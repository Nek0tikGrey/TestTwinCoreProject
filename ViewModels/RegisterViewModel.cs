using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestTwinCoreProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Дата рождения")]
        public DateTime DateBirthday { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
        [Required]
        [ScaffoldColumn(true)]
        public string InviteCode { get; set; }
        [Required]
        [ScaffoldColumn(true)]
        public string CaptchaCodeGen { get; set; }
        [Required]
        [Compare("CaptchaCodeGen", ErrorMessage = "Код введен не правильно")]
        [Display(Name = "Ведите код с изображения")]
        public string CaptchaCode { get; set; }
    }
}
