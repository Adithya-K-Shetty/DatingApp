using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);

        Task<AppUser> GetUserWithLikes(int userId);


        //GetUsersLikes return list of liked or liked by users
        //which depends on the predicate
        Task<IEnumerable<LikeDto>> GetUsersLikes(string predicate,int userId);
    }
}