using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TasteOfHome.Data;
using TasteOfHome.Models;
using TasteOfHome.Services;

namespace TasteOfHome.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;

        public RegisterModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string Message { get; set; } = "";
        public string Error { get; set; } = "";

        public async Task<IActionResult> OnPostAsync()
        {
            var email = (Email ?? "").Trim().ToLowerInvariant();
            var password = Password ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Error = "Email and password are required.";
                Message = "";
                return Page();
            }

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(email))
            {
                Error = "Please enter a valid email address.";
                Message = "";
                return Page();
            }

            if (password.Length < 6)
            {
                Error = "Password must be at least 6 characters.";
                Message = "";
                return Page();
            }

            var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
            if (exists)
            {
                Error = "An account with that email already exists.";
                Message = "";
                return Page();
            }

            var user = new AppUser
            {
                Email = email,
                Name = "",
                PasswordHash = PasswordHasher.Hash(password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            

            FakeUsers.Users.Add((email, password));

            Message = "Account created successfully. Please log in.";
            Error = "";

            return Page();
        }
    }
}