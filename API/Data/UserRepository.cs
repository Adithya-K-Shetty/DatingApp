using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            //The .AsQueryable() method is called on the Users entity set
            //It converts the Users entity set to an IQueryable<T>
            //interface, which allows you to construct 
            //and execute queries against the data source using LINQ
             var query = _context.Users.AsQueryable(); //AsNoTracking :- entity frame work wont keep track of returned items as we dont process over them in user control

             query = query.Where(u => u.UserName != userParams.CurrentUsername);
             query = query.Where(u => u.Gender == userParams.Gender);
            
            //older age
             var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
            //younger age
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u=>u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                //gives newest users first
                "created" => query.OrderByDescending(u => u.Created),
                //bydefault it will ordered based on lastActive
                _ => query.OrderByDescending(u => u.LastActive)
            };

             return await PagedList<MemberDto>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),userParams.PageNumber,userParams.PageSize);
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