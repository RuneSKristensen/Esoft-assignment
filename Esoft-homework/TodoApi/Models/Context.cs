using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EsoftApi.Models
{
    public class Context : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Database.db"); //use/create sqlite database
        }

         public DbSet<Person> Family { get; set; } //with person model
         public DbSet<Relation> Relations { get; set; } //and relation model
    }
}
