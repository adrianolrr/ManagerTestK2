using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTestK2.Domain.Entities
{
    public class Phone
    {
        public Guid Id { get; private set; }
        public string Number { get; private set; }

        public Phone(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Número de telefone é obrigatório.");
            Id = Guid.NewGuid();
            Number = number;
        }
    }
}
