using System.ComponentModel.DataAnnotations;


namespace SkillProfi_WebSite.UserAuthorization
{
    
    public class UserLogin
    {
        [Required, MaxLength(20)]
        [Display(Name = "Логин")]
        public string LoginUser { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
