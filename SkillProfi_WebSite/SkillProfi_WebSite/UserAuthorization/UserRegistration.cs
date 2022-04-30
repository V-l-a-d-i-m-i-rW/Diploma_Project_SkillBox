using System.ComponentModel.DataAnnotations;

namespace SkillProfi_WebSite.UserAuthorization
{
    public class UserRegistration
    {
        [Required, MaxLength(20)]
        [Display(Name = "Логин")]
        public string LoginUser { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string ConfirmPassword { get; set; }
    }
}
