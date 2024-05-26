using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class ChangePassword : ReactiveValidationObject
    {
        public ChangePassword()
        {
            this.ValidationRule(m => m.OldPassword,
                pass => !string.IsNullOrWhiteSpace(pass),
            "Пароль обязателен к заполнению.");
            this.ValidationRule(
            m => m.NewPassword,
                pass => !string.IsNullOrWhiteSpace(pass) && pass.Length > 6 && pass.Length < 20,
                "Пароль обязателен к заполнению. Длина должна быть больше 6 символов и меньше 20.");
        }
        private string _newPassword = null!;
        private string _oldPassword = null!;
        public int UserId { get; set; }
        public string OldPassword
        {
            get => _oldPassword;
            set => this.RaiseAndSetIfChanged(ref _oldPassword, value);
        }
        public string NewPassword 
        {
            get => _newPassword;
            set => this.RaiseAndSetIfChanged(ref _newPassword, value);
        }

    }
}
