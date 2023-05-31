using API.DTOs;
using API.Entities;

namespace API.interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        
        Task<bool> SaveAllAsync();

        //this fetches all the data
        //in which some of them are not displayed
        //at the user end
        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        //optimizing the getting of list of users
        //by just getting what data is required as specified at the DTO
        Task<IEnumerable<MemberDto>> GetMembersAsync();

        Task<MemberDto> GetMemberAsync(string username);
    }
}