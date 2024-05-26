using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class Employee : ReactiveValidationObject
    {
        private string? _surname = "";
        private string _name = "";
        private string? _middleName = "";
        private string? _phoneNumber;

        public Employee()
        {
            this.ValidationRule(m => m.Name,
                name => !String.IsNullOrWhiteSpace(name),
                "Укажите имя пользователя.");
            this.ValidationRule(m => m.Name,
                name => name?.Length < 15,
                "Слишком длинное имя.");
            this.ValidationRule(m => m.Surname,
                name => name?.Length < 20,
                "Слишком длинная фамилия.");
            this.ValidationRule(m => m.MiddleName,
                name => name?.Length < 20,
                "Слишком длинное отчество.");

        }
        public int EmployeeId { get; set; }

        public string? Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string? MiddleName
        {
            get => _middleName;
            set => this.RaiseAndSetIfChanged(ref _middleName, value);
        }

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
        }
    }
}
