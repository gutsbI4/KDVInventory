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
    public class UserEdit : ReactiveValidationObject
    {
        private string _login = "";
        private string _password = "";
        private int _roleId;
        private string? _image;
        private bool _isArchive;
        private bool _isActive;
        private Employee? _employee;

        public UserEdit()
        {
            this.ValidationRule(m => m.Login,
                login => !String.IsNullOrWhiteSpace(login) && login.Length > 4 && login.Length <=15,
                "Логин не должен быть пустым! Длина от 5 до 15");
            this.ValidationRule(m => m.Password,
                pass => !String.IsNullOrWhiteSpace(pass) && pass.Length > 6 && pass.Length < 20,
                "Пароль обязателен к заполнению. Длина должна быть больше 6 символов и меньше 20.");
            this.ValidationRule(m => m.RoleId,
                role => role != null || role != 0,
                "Выберите роль!");
        }

        public int UserId { get; set; }

        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public int RoleId
        {
            get => _roleId;
            set => this.RaiseAndSetIfChanged(ref _roleId, value);
        }

        public string? Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public bool IsArchive
        {
            get => _isArchive;
            set => this.RaiseAndSetIfChanged(ref _isArchive, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }
        public Employee? Employee
        {
            get => _employee;
            set => this.RaiseAndSetIfChanged(ref _employee, value);
        }
    }
}
