using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{

    //DbContext class is a part of Entity Framework Core 
    //and provides an abstraction for interacting with the database
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //class is used in Entity Framework to represent a collection of entities
        //It provides a set of methods and properties to query, insert, update, and delete data from the corresponding table
        //entity in application code represent table in database
        //he DbSet provides methods to query the database using LINQ 
        public DbSet<AppUser> Users {get;set;}
        public DbSet<UserLike> Likes {get;set;}

        public DbSet<Message> Messages { get; set; }


        //a method defined as vitual in a class
        //can be overrided

        /*--  implementation of a many-to-many relationship between the AppUser class
         and itself through the UserLike class --*/
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId,k.TargetUserId}); //represent primary key used in this table
            

            //this is for user liking many users
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

             builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);


             builder.Entity<Message>()
             .HasOne(u => u.Recipient)
             .WithMany(m => m.MessagesReceived)
             .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
             .HasOne(u => u.Sender)
             .WithMany(m => m.MessagesSent)
             .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}