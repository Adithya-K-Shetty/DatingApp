using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using AutoMapper.Execution;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
           return await _context.Users
                .Where(x => x.UserName == username)  //configuration take config from mapper automapperprofile
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) //here we are projecting it into a member dto
                .SingleOrDefaultAsync(); //it expcts at most one user to match the above lambda expression else return null if no matching  
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            //returns the single element from the data source
            //that matches the condition
            //lambda expression checks for the desired user
            //based on the username
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            //we should explicitly specify 
            //that even data from related enity
            //has to be fetched
            //specifically it is called as eager loading
            return await _context.Users
            .Include(p =>p.Photos)//this causes object cycle between user and photos
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            //checks whether the number of changes
            //made greater than 0
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            //informing entity frame work tracker
            //an entity has been updated
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}