using System.ComponentModel.DataAnnotations;

namespace tutorialAsp.NetCore.Data.Models.ViewModel.AccountManage
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Не указан старый пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Укажите новый пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Подтверждение нового пароля не совпадает")]
        public string ConfirmNewPassword { get; set; }
    }
}
