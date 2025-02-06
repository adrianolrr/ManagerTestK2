using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Numerics;
using ManagerTestK2.Domain.Utils;

namespace ManagerTestK2.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }  // Nome
        public string LastName { get; private set; }   // Sobrenome
        public string Email { get; private set; }
        public long DocumentNumber { get; private set; } // Único e obrigatório
        public DateTime DateOfBirth { get; private set; }
        public EmployeeRole Role { get; private set; }
        public Guid? ManagerId { get; private set; }
        public Employee Manager { get; private set; }
        public ICollection<Phone> Phones { get; private set; } = new List<Phone>();
        public string PasswordHash { get; private set; }
        public string PasswordSalt { get; private set; }

        public Employee(string firstName, string lastName, string email, long documentNumber, DateTime dateOfBirth, EmployeeRole role)
        {
            Id = Guid.NewGuid();
            SetName(firstName, lastName);
            Email = email ?? throw new ArgumentException("E-mail é obrigatório.");
            DocumentNumber = documentNumber;
            DateOfBirth = dateOfBirth;
            Role = role;
        }

        public void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Sobrenome é obrigatório.");
            FirstName = firstName;
            LastName = lastName;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail é obrigatório.");
            Email = email;
        }

        public void SetManager(Employee manager)
        {
            Manager = manager;
            ManagerId = manager?.Id;
        }

        public void SetPassword(string password)
        {
            // Utiliza uma função de hash com salt (exemplo usando PBKDF2)
            var (hash, salt) = PasswordHasher.HashPassword(password);
            PasswordHash = hash;
            PasswordSalt = salt;
        }

        public void AddPhone(Phone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));
            Phones.Add(phone);
        }

        /// <summary>
        /// Verifica se o funcionário é maior de idade (18 anos ou mais)
        /// </summary>
        public bool IsAdult()
        {
            var age = DateTime.Today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}
