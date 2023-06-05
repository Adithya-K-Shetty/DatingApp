using API.Data;
using API.Helpers;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {

        //this keyword represents
        //AddApplicationServices method is an extension method for IServiceCollection
        //we can call the method as an instance of IServiceCollection
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(opt => 
            {
                opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddCors();

            //service will be there unil is controller is created and disposed
            //new instance of TokenService will be created for each HTTP request 
            //and will be shared within that request
            services.AddScoped<ITokenService, TokenService>();
            //service will be scoped to the http request
            services.AddScoped<IUserRepository,UserRepository>();

            //object-to-object mapping library.
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // binds the configuration section named "CloudinarySettings" 
            //from the configuration file
            //to the CloudinarySettings class
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddScoped<IPhotoService,PhotoService>();

            services.AddScoped<LogUserActivity>();

            services.AddScoped<ILikesRepository,LikesRepository>();

            return services;
        }
    }
}