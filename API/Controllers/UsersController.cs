using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController //Inheritence
    {
        
        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _uow;
        
        public UsersController(IUnitOfWork uow,IMapper mapper,IPhotoService photoService)  // dependency injection
        {
            _uow = uow;
            _mapper = mapper;
            _photoService = photoService;
            
        }
        

        [HttpGet]
        //[FromQuery] is used to provide a hint for the
        //API to look for the query parameters
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());
            
            userParams.CurrentUsername = User.GetUsername();

            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _uow.UserRepository.GetMembersAsync(userParams);

            //sending pagination response 
            //via the response header
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            //mapping of returened users and dto takes place
           
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
           return await _uow.UserRepository.GetMemberAsync(username);
            
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            //When we retrieve the user the entity frame work starts tracking that particular user
            var user = await _uow.UserRepository.GetUserByUsernameAsync(username);

            if(user == null) return NotFound();

            _mapper.Map(memberUpdateDto,user);

            if(await _uow.Complete()) return NoContent(); //http status code 204 which says everything is ok but nothing to return

            return BadRequest("Failed To Update User");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            //entity frame work tracks the user
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0) photo.IsMain = true;

            //tracks the user in memory
            user.Photos.Add(photo);

            if(await _uow.Complete())
            {
                //returns 201 created response
                //along with location detail about where to find the newly created resource
                //first parameter :- client will be redirected to the GetUser Method
                //second parameter :- represents route values that will be used to construct the URL of the action method 
                //third parameter :- represent the data returned in response body
                return CreatedAtAction(nameof(GetUser),new {username = user.UserName},_mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem Adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]

        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null)  return NotFound();

            //checking whether the photo passed is already a main photo
            if(photo.IsMain) return BadRequest("this is already your main photo");
            
            //if the above condition fails
            //get the current main photo 
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if(currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _uow.Complete()) return NoContent();

            return BadRequest("Problem Setting Main Photo");

        }

        [HttpDelete("delete-photo/{photoId}")]

        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());  

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("U cannot delete your main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletionResultAsync(photo.PublicId);

                if(result.Error != null) return BadRequest(result.Error.Message);

            }
            //removing data from the database
            user.Photos.Remove(photo);

            if(await _uow.Complete()) return Ok();

            return BadRequest("Problem Deleting Photo");
        }
    }
}