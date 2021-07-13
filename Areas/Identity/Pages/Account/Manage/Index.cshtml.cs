using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KRU.Data;
using KRU.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KRU.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public int? DepartmentId { get; set; }
            public string Position { get; set; }
            public string FName { get; set; }
            public string LName { get; set; }
            public string SName { get; set; }
            public IEnumerable<SelectListItem> DepartmentList { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var department = _db.User.Include(r => r.Department).FirstOrDefault(u => u.Id == user.Id).DepartmentId;
            var position = _db.User.FirstOrDefault(u => u.Id == user.Id).Position;
            var FName = _db.User.FirstOrDefault(u => u.Id == user.Id).FName;
            var LName = _db.User.FirstOrDefault(u => u.Id == user.Id).LName;
            var SName = _db.User.FirstOrDefault(u => u.Id == user.Id).SName;
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FName = FName,
                LName = LName,
                SName = SName,
                Position = position,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            var user_ = new Users
            {
              
                Position = Input.Position,
                FName = Input.FName,
                LName = Input.LName,
                SName = Input.SName,
                PhoneNumber = Input.PhoneNumber
            };
            
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var objUs = _db.User.FirstOrDefault(u => u.Id == user.Id);
            var position = objUs.Position;

            if(Input.Position != position)
            {
                objUs.Position = Input.Position;
                _db.SaveChanges();
            }
            var FName = objUs.FName;
            var SName = objUs.SName;
            var LName = objUs.LName;

            if (Input.FName != FName)
            {
                objUs.FName = Input.FName;
                _db.SaveChanges();
            }
            if (Input.SName != SName)
            {
                objUs.SName = Input.SName;
                _db.SaveChanges();
            }
            if (Input.LName != LName)
            {
                objUs.LName = Input.LName;
                _db.SaveChanges();
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Профиль янгиланди";
            return RedirectToPage();
        }
    }
}
