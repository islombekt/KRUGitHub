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
        public DbSet<Category1> Category1s { get; set; }
        public DbSet<Category11> Category11s { get; set; }
        public DbSet<Category111> Category111s { get; set; }
        public DbSet<FinanceReport> FinanceReports { get; set; }
        public DbSet<Task_Empl> Task_Empls { get; set; }
        public DbSet<Task_Report> Task_Reports { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Empl_Selected_Option> Empl_Selected_Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
    }
}
