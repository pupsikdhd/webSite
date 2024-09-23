using System.ComponentModel.DataAnnotations;

namespace tutorialAsp.NetCore.Data.Models.ViewModel
{
    public class LoginViewModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
