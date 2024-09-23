using System.ComponentModel.DataAnnotations;

namespace tutorialAsp.NetCore.Data.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Не указан userName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    }
}
