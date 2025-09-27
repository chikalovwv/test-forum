using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите username или email.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}