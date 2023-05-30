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
    }
}