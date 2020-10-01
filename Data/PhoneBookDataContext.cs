using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Practice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practice.Data
{
    public class PhoneBookDataContext : DbContext
    {
        public DbSet<Phonebook> Contacts { get; set; }
    }   

}
