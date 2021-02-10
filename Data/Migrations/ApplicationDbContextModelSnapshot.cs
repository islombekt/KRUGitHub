﻿// <auto-generated />
using System;
using KRU.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KRU.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("KRU.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Building")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AddressId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("KRU.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("DepartmentName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("KRU.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("EmployeeState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<double>("Score")
                        .HasColumnType("float");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("EmployeeId");

                    b.HasIndex("ManagerId");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("KRU.Models.FileHistory", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FileEnd")
                        .HasColumnType("datetime2");

                    b.Property<bool>("FileFinished")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FileStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaskTypeId")
                        .HasColumnType("int");

                    b.HasKey("FileId");

                    b.HasIndex("TaskTypeId");

                    b.ToTable("FileHistory");
                });

            modelBuilder.Entity("KRU.Models.Manager", b =>
                {
                    b.Property<int>("ManagerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ManagerId");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("KRU.Models.Objects", b =>
                {
                    b.Property<int>("ObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("ObjectName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ObjectId");

                    b.HasIndex("AddressId");

                    b.ToTable("Objects");
                });

            modelBuilder.Entity("KRU.Models.Plan", b =>
                {
                    b.Property<int>("PlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<bool>("HaveSeen")
                        .HasColumnType("bit");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<int?>("ObjectId")
                        .HasColumnType("int");

                    b.Property<string>("PlanDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PlanEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PlanStart")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("PlanId");

                    b.HasIndex("AddressId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ManagerId");

                    b.HasIndex("ObjectId");

                    b.HasIndex("TaskId");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("KRU.Models.Report", b =>
                {
                    b.Property<int>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<bool>("HaveSeen")
                        .HasColumnType("bit");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<int?>("ObjectId")
                        .HasColumnType("int");

                    b.Property<string>("ReportComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReportDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReportDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ReportScore")
                        .HasColumnType("float");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("ReportId");

                    b.HasIndex("AddressId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ManagerId");

                    b.HasIndex("ObjectId");

                    b.HasIndex("TaskId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("KRU.Models.Task_File", b =>
                {
                    b.Property<int>("Task_FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("FileId")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("Task_FileId");

                    b.HasIndex("FileId");

                    b.HasIndex("TaskId");

                    b.ToTable("Task_Files");
                });

            modelBuilder.Entity("KRU.Models.Task_Type", b =>
                {
                    b.Property<int>("TaskTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("NameType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskTypeID");

                    b.ToTable("Task_Types");
                });

            modelBuilder.Entity("KRU.Models.Tasks", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("File")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Finished")
                        .HasColumnType("bit");

                    b.Property<string>("SumGain")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SumLost")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TaskEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TaskStarted")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TaskTypeId")
                        .HasColumnType("int");

                    b.HasKey("TaskId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("TaskTypeId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("KRU.Models.Users", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EnteredToWork")
                        .HasColumnType("datetime2");

                    b.Property<string>("FName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FiredFromWork")
                        .HasColumnType("datetime2");

                    b.Property<string>("LName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("AddressId");

                    b.HasIndex("DepartmentId");

                    b.HasDiscriminator().HasValue("Users");
                });

            modelBuilder.Entity("KRU.Models.Employee", b =>
                {
                    b.HasOne("KRU.Models.Manager", "Manager")
                        .WithMany("Employees")
                        .HasForeignKey("ManagerId");

                    b.HasOne("KRU.Models.Users", "User")
                        .WithOne("Employee")
                        .HasForeignKey("KRU.Models.Employee", "UserId");

                    b.Navigation("Manager");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KRU.Models.FileHistory", b =>
                {
                    b.HasOne("KRU.Models.Task_Type", "Task_Type")
                        .WithMany("FileHistory")
                        .HasForeignKey("TaskTypeId");

                    b.Navigation("Task_Type");
                });

            modelBuilder.Entity("KRU.Models.Manager", b =>
                {
                    b.HasOne("KRU.Models.Users", "User")
                        .WithOne("Managers")
                        .HasForeignKey("KRU.Models.Manager", "UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KRU.Models.Objects", b =>
                {
                    b.HasOne("KRU.Models.Address", "Address")
                        .WithMany("Objects")
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("KRU.Models.Plan", b =>
                {
                    b.HasOne("KRU.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("KRU.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("KRU.Models.Manager", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId");

                    b.HasOne("KRU.Models.Objects", "Objects")
                        .WithMany()
                        .HasForeignKey("ObjectId");

                    b.HasOne("KRU.Models.Tasks", "Tasks")
                        .WithMany()
                        .HasForeignKey("TaskId");

                    b.Navigation("Address");

                    b.Navigation("Employee");

                    b.Navigation("Manager");

                    b.Navigation("Objects");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("KRU.Models.Report", b =>
                {
                    b.HasOne("KRU.Models.Address", "Address")
                        .WithMany("Reports")
                        .HasForeignKey("AddressId");

                    b.HasOne("KRU.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("KRU.Models.Manager", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId");

                    b.HasOne("KRU.Models.Objects", "Objects")
                        .WithMany("Reports")
                        .HasForeignKey("ObjectId");

                    b.HasOne("KRU.Models.Tasks", "Tasks")
                        .WithMany("Reports")
                        .HasForeignKey("TaskId");

                    b.Navigation("Address");

                    b.Navigation("Employee");

                    b.Navigation("Manager");

                    b.Navigation("Objects");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("KRU.Models.Task_File", b =>
                {
                    b.HasOne("KRU.Models.FileHistory", "FileHistory")
                        .WithMany("Task_Files")
                        .HasForeignKey("FileId");

                    b.HasOne("KRU.Models.Tasks", "Tasks")
                        .WithMany("Task_Files")
                        .HasForeignKey("TaskId");

                    b.Navigation("FileHistory");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("KRU.Models.Tasks", b =>
                {
                    b.HasOne("KRU.Models.Department", "Department")
                        .WithMany("Tasks")
                        .HasForeignKey("DepartmentId");

                    b.HasOne("KRU.Models.Task_Type", "Task_Type")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskTypeId");

                    b.Navigation("Department");

                    b.Navigation("Task_Type");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KRU.Models.Users", b =>
                {
                    b.HasOne("KRU.Models.Address", "Address")
                        .WithMany("Users")
                        .HasForeignKey("AddressId");

                    b.HasOne("KRU.Models.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId");

                    b.Navigation("Address");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("KRU.Models.Address", b =>
                {
                    b.Navigation("Objects");

                    b.Navigation("Reports");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("KRU.Models.Department", b =>
                {
                    b.Navigation("Tasks");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("KRU.Models.FileHistory", b =>
                {
                    b.Navigation("Task_Files");
                });

            modelBuilder.Entity("KRU.Models.Manager", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("KRU.Models.Objects", b =>
                {
                    b.Navigation("Reports");
                });

            modelBuilder.Entity("KRU.Models.Task_Type", b =>
                {
                    b.Navigation("FileHistory");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("KRU.Models.Tasks", b =>
                {
                    b.Navigation("Reports");

                    b.Navigation("Task_Files");
                });

            modelBuilder.Entity("KRU.Models.Users", b =>
                {
                    b.Navigation("Employee");

                    b.Navigation("Managers");
                });
#pragma warning restore 612, 618
        }
    }
}
