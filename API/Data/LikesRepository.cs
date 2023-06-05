using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context){
            _context = context;

        }

        //get a like which the sourceId and targetId matches
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId,targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUsersLikes(LikesParams likesParams)
        {

          //AsQueryAble represent that the query is not yet been executed
           var users = _context.Users.OrderBy(u=>u.UserName).AsQueryable();
           var likes = _context.Likes.AsQueryable();

           if(likesParams.Predicate == "liked"){
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.TargetUser);
           }

            if(likesParams.Predicate == "likedBy"){
            likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
           }


            //manual mapping
            var likedUsers =  users.Select(user =>  new LikeDto{
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber,likesParams.PageSize);

        }

        public async  Task<AppUser> GetUserWithLikes(int userId)
        {
            //used to check whether a user is already been liked by other user
          return await _context.Users
          .Include(x => x.LikedUsers)
          .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}