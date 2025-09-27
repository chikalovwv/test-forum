using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Forum.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя пользователя.")]
        [Display(Name = "UserName")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите email.")]
        [EmailAddress(ErrorMessage = "Неверный формат email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите дату рождения.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public IFormFile? Avatar { get; set; }

        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}