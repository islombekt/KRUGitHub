using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using KRU.Data;
using KRU.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace KRU.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
       private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
    
        public RegisterModel(

            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender
            , ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression("^[a-zA-Z0-9]+[.]?[a-z0-9]{3,20}@ung.uz$", ErrorMessage = "Электрон почта нотўғри кўрсатилган!")]
            [EmailAddress]
            [Display(Name = "Почта")]
            public string Email { get; set; }

            [Required]
            [StringLength(15, ErrorMessage = "{0} камида {2} ва кўпи билан {1} хонадан ташкил топиши керак!", MinimumLength = 6)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,15}$", ErrorMessage = "Пароль камида битта бош ҳарф, кичик ҳарф, рақам ва символдан ташкил топиши керак!")]

            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            [RegularExpression("^[9,7][0-9]{8}$", ErrorMessage = "Тел. рақами нотўғри кўрсатилган!")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Телефон номер")]
            public string PhoneNumber { get; set; }
            [Required]
            public string FName { get; set; }
            [Required]
            public string LName { get; set; }
            [Required]
            public string SName { get; set; }
           
            public string Role { get; set; }
           
            public int AddressId { get; set; }
            [Required]
            public string Position { get;  set; }
            
            public int DepartmentId { get; set; }
            public int? ManagerId { get; set; }

            public IEnumerable<SelectListItem> BuildingList { get; set; }
            public IEnumerable<SelectListItem> DepartmentList { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }
            public IEnumerable<SelectListItem> ManagerList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            var userList = _db.User.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            }
            Input = new InputModel() {
                BuildingList = _db.Addresses.Select(i => new SelectListItem
                {
                    Text = i.Building,
                    Value = i.AddressId.ToString()
                }),
                DepartmentList = _db.Departments.ToList().Select(i => new SelectListItem
                {
                    Text = i.DepartmentName,
                    Value = i.DepartmentId.ToString()
                }),
                RoleList = _db.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                    Selected = false
                }),
                ManagerList = userList.Where(u => u.Role == "Manager").Select(i => new SelectListItem
                {
                    Text = i.FName + " " + i.LName,
                    Value = i.Id.ToString()
                }),

            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
           var userList = _db.User.ToList();
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
               
                var user = new Users { 
                    UserName = Input.Email, 
                    Email = Input.Email, 
                    FName = Input.FName, 
                    LName = Input.LName, 
                    SName = Input.SName, 
                    Role = Input.Role,
                    PhoneNumber = Input.PhoneNumber,
                    AddressId = Input.AddressId,
                    DepartmentId = Input.DepartmentId,
                    Position = Input.Position,
                    
                    EmailConfirmed = true
                };

                // *********************
                

                // ****************************88
                var manager = new Models.Manager
                {
                    UserId = user.Id
                };
                var employee = new Models.Employee
                {
                    UserId = user.Id,

                    ManagerId = Input.ManagerId,
                };

                // Manager is creating new user
                if (User.IsInRole(SD.Role_Manager))
                {
                    var ManId = _db.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
                    var DepId = _db.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
                    user.DepartmentId = DepId;
                    user.Role = SD.Role_Employee;
                    employee.ManagerId = ManId;

                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var comfirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, comfirmationLink);
                    _logger.LogInformation("User created a new account with password.");
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Manager))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Manager));

                    }
                    if (user.Role == null)
                    {
                        //await _userManager.AddToRoleAsync(user, SD.Role_Requester);
                        user.Role = SD.Role_Admin;
                        
                    }

                    else 
                    {
                        if (user.Role == SD.Role_Employee)
                        {
                            _db.Employees.Add(employee);

                        }
                        if(user.Role == SD.Role_Manager)
                        {
                            _db.Managers.Add(manager);
                        }
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }
                    if (user.Role == SD.Role_Admin)
                    {
                        
                        //Console.WriteLine(requester.RequestmakerId + " and " + requester.UserId);
                        await _userManager.AddToRoleAsync(user, user.Role);
                        user.Role = null;
                    }

                   

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else if(user.Role == SD.Role_Admin)
                        {
                            //admin is registering new user
                            return RedirectToAction("Index", "Users", new { Area = "Admin" });
                        }
                        else if(user.Role == SD.Role_Manager)
                        {
                            return RedirectToAction("Index", "Users", new { Area = "Admin" });
                        }
                        else if(user.Role == SD.Role_Employee)
                        {
                            return RedirectToAction("Index", "Employees", new { Area = "Manager" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Input = new InputModel()
            {
                BuildingList = _db.Addresses.Select(i => new SelectListItem
                {
                    Text = i.Building,
                    Value = i.AddressId.ToString()
                }),
                DepartmentList = _db.Departments.ToList().Select(i => new SelectListItem
                {
                    Text = i.DepartmentName,
                    Value = i.DepartmentId.ToString()
                }),
                RoleList = _db.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                    Selected = false
                }),
                ManagerList = userList.Where(u => u.Role == "Manager").Select(i => new SelectListItem
                {
                    Text = i.FName + " " + i.LName,
                    Value = i.Id.ToString()
                }),

            };


            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
