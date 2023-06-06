using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository,ILikesRepository likesRepository){
            _likesRepository = likesRepository;
            _userRepository = userRepository;

        }
        [HttpPost("{username}")] //to like a user
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId(); //user who likes the other user
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser ==null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId,likedUser.Id);

            if(userLike != null) return BadRequest("You already like this user");

             userLike = new UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
             };

             sourceUser.LikedUsers.Add(userLike);

             if(await _userRepository.SaveAllAsync()) return Ok();

             return BadRequest("Failed To Like User");
        }

        [HttpGet] //to get users whom the current user liked
        public async Task<ActionResult<PagedList<LikeDto>>>  GetUserLikes([FromQuery]LikesParams likesParams){
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUsersLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);
        }
    }
}