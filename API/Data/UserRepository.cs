using API.Entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
            
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