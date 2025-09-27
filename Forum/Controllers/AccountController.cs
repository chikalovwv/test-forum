using System;
using System.IO;
using System.Threading.Tasks;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            DateTime now = DateTime.UtcNow;
            int age = now.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth > now.AddYears(-age))
            {
                age = age - 1;
            }
            if (age < 18)
            {
                ModelState.AddModelError(string.Empty, "Регистрация разрешена с 18 лет.");
                return View(model);
            }

            ApplicationUser existingByName = await _userManager.FindByNameAsync(model.UserName);
            if (existingByName != null)
            {
                ModelState.AddModelError(string.Empty, "Имя пользователя занято.");
                return View(model);
            }

            ApplicationUser existingByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email уже используется.");
                return View(model);
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                AvatarPath = "/uploads/avatars/default.png"
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded == false)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, "Ошибка: " + error.Description);
                }
                return View(model);
            }

            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                string uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                if (Directory.Exists(uploadsPath) == false)
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Avatar.FileName);
                string filePath = Path.Combine(uploadsPath, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(stream);
                }
                user.AvatarPath = "/uploads/avatars/" + fileName;
                await _userManager.UpdateAsync(user);
            }

            await _userManager.AddToRoleAsync(user, "user");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Topics");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            ApplicationUser? userByName = await _userManager.FindByNameAsync(model.Login);ApplicationUser? userByEmail = null;
            if (userByName == null)
            {
                userByEmail = await _userManager.FindByEmailAsync(model.Login);
            }

            ApplicationUser? user = userByName ?? userByEmail;
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "Ваш аккаунт заблокирован.");
                return View(model);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (signInResult.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }

            return RedirectToAction("Index", "Topics");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Topics");
        }
    }
}