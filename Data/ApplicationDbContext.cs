using KRU.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KRU.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Users> User { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Objects> Objects { get; set; }
        public DbSet<Task_Type> Task_Types { get; set; }
        public DbSet<FileHistory> FileHistory { get; set; }
        public DbSet<Task_File> Task_Files { get; set; }


    }
}
