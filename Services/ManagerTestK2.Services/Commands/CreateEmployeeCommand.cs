using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTestK2.Services.Commands
{
    public class CreateEmployeeCommand
    {
        public string FirstName { get; set; }        // Nome
        public string LastName { get; set; }         // Sobrenome
        public string Email { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Caso o funcionário tenha um gestor, informe o número do documento do gestor.
        /// </summary>
        public string ManagerDocumentNumber { get; set; }
        /// <summary>
        /// Função desejada para o novo funcionário ("Employee", "Leader" ou "Director")
        /// </summary>
        public string Role { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();
    }
}
