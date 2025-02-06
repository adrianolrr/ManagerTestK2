using AutoMapper;
using ManagerTestK2.Domain.Entities;
using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Domain.Pagination;
using ManagerTestK2.Extensions;
using ManagerTestK2.Services.Commands;
using ManagerTestK2.Services.DTO;
using ManagerTestK2.Services.Interfaces;

namespace ManagerTestK2.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IMapper _mapper;
        public EmployeeService(IGenericRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> CreateEmployeeAsync(CreateEmployeeCommand command, Employee currentEmployee)
        {
            // Validação dos campos obrigatórios
            if (string.IsNullOrWhiteSpace(command.FirstName) || string.IsNullOrWhiteSpace(command.LastName))
                throw new ArgumentException("Nome e sobrenome são obrigatórios.");
            if (string.IsNullOrWhiteSpace(command.Email))
                throw new ArgumentException("E-mail é obrigatório.");
            if (FormatCharacters.RemoveNonNumber(command.DocumentNumber) > 0)
                throw new ArgumentException("Número do documento é obrigatório.");
            if (command.DateOfBirth == default)
                throw new ArgumentException("Data de nascimento inválida.");
            if (string.IsNullOrWhiteSpace(command.Password))
                throw new ArgumentException("Senha é obrigatória.");

            var documentNumber = FormatCharacters.RemoveNonNumber(command.DocumentNumber);
            // Verifica se a pessoa é maior de idade
            var tempEmployee = new Employee(command.FirstName, command.LastName, command.Email, documentNumber, command.DateOfBirth, EmployeeRole.Employee);
            if (!tempEmployee.IsAdult())
                throw new InvalidOperationException("O funcionário deve ser maior de idade.");

            // Verifica se já existe um funcionário com o mesmo documento
            var existingEmployee = await _employeeRepository.GetById(documentNumber);
            if (existingEmployee != null)
                throw new InvalidOperationException("Já existe um funcionário com esse número de documento.");

            // Converte a string para o enum EmployeeRole
            if (!Enum.TryParse<EmployeeRole>(command.Role, true, out var role))
                throw new ArgumentException("Função inválida.");

            // Valida a hierarquia de permissões:
            // Um usuário só pode criar um funcionário cuja função seja menor ou igual à dele.
            if (!CanCreate(currentEmployee.Role, role))
                throw new UnauthorizedAccessException("Você não pode criar um funcionário com uma função superior à sua.");

            var employee = new Employee(command.FirstName, command.LastName, command.Email, documentNumber, command.DateOfBirth, role);
            employee.SetPassword(command.Password);

            // Adiciona os telefones (deve haver pelo menos um)
            if (command.PhoneNumbers == null || command.PhoneNumbers.Count == 0)
                throw new ArgumentException("Deve haver pelo menos um telefone.");

            foreach (var phone in command.PhoneNumbers)
            {
                employee.AddPhone(new Phone(phone));
            }

            // Configura o gestor, se informado
            if (!string.IsNullOrWhiteSpace(command.ManagerDocumentNumber))
            {
                var manager = await _employeeRepository.GetById(documentNumber);
                if (manager == null)
                    throw new InvalidOperationException("Gestor não encontrado.");
                employee.SetManager(manager);
            }

            // Persiste o novo funcionário
            await _employeeRepository.Create(employee);

            return employee;
        }

        /// <summary>
        /// Determina se o usuário atual pode criar um funcionário com a função desejada.
        /// Regras:
        /// - Um funcionário (Employee) só pode criar outros Employee.
        /// - Um líder (Leader) pode criar Employee ou Leader, mas não Director.
        /// - Um diretor (Director) pode criar qualquer.
        /// </summary>
        private bool CanCreate(EmployeeRole currentRole, EmployeeRole newRole)
        {
            return (int)currentRole >= (int)newRole;
        }


        public async Task<PagedList<EmployeeDTO>> GetAllEmployees(int pageNumber, int pageSize)
        {
            var employees = await _employeeRepository.GetAllPagination(pageNumber, pageSize);
            var employeesDTO = _mapper.Map<List<EmployeeDTO>>(employees);

            return new PagedList<EmployeeDTO>(employeesDTO, pageNumber, pageSize, employees.TotalCount);
        }

        public async Task<EmployeeDTO> GetEmployeeById(int idEmployee)
        {
            var employee = await _employeeRepository.GetById(idEmployee);
            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<EmployeeDTO> UpdateEmployee(EmployeeDTO employeeDTO)
        {
            var employee = await _employeeRepository.GetById(employeeDTO.Id);

            if (employee == null)
                return null;

            var employeeUpdate = await _employeeRepository.Update(employee);
            return _mapper.Map<EmployeeDTO>(employeeUpdate);
        }

        public async Task<EmployeeDTO> DeleteEmployee(int idEmployee)
        {
            var employee = await _employeeRepository.GetById(idEmployee);

            if (employee == null)
                return null;

            var employeeDeleted = await _employeeRepository.Remove(employee);
            return _mapper.Map<EmployeeDTO>(employeeDeleted);
        }
    }
}
