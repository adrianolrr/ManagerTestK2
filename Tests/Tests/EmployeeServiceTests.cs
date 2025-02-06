using ManagerTestK2.Domain.Entities;
using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Domain.Pagination;
using ManagerTestK2.Infrastrucure.Repository.Interfaces;
using ManagerTestK2.Services.Commands;
using ManagerTestK2.Services.Services;
using Xunit;

namespace Tests
{
    public class EmployeeServiceTests
    {
        private readonly EmployeeService _employeeService;
        //private readonly FakeEmployeeRepository _employeeRepository;
        private readonly Employee _currentEmployee;

        public EmployeeServiceTests()
        {
            //_employeeRepository = new FakeEmployeeRepository();
            //_employeeService = new EmployeeService(_employeeRepository);
            // Para os testes, usamos um usuário diretor (nível mais alto)
            _currentEmployee = new Employee("Admin", "User", "admin@example.com", 1232131, DateTime.Today.AddYears(-30), EmployeeRole.Director);
        }

        [Fact]
        public async Task CreateEmployee_Should_CreateEmployee_When_ValidData()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DocumentNumber = "1321312",
                DateOfBirth = DateTime.Today.AddYears(-25),
                Password = "StrongPass@123",
                Role = "Employee",
                PhoneNumbers = new List<string> { "123456789" }
            };

            // Act
            var employee = await _employeeService.CreateEmployeeAsync(command, _currentEmployee);

            // Assert
            Assert.NotNull(employee);
            Assert.Equal("John", employee.FirstName);
        }

        [Fact]
        public async Task CreateEmployee_Should_ThrowException_When_Minor()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                DocumentNumber = "13123",
                DateOfBirth = DateTime.Today.AddYears(-15),
                Password = "Pass@123",
                Role = "Employee",
                PhoneNumbers = new List<string> { "987654321" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _employeeService.CreateEmployeeAsync(command, _currentEmployee));
        }

        [Fact]
        public async Task CreateEmployee_Should_ThrowException_When_DocumentExists()
        {
            // Arrange
            var command1 = new CreateEmployeeCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DocumentNumber = "123123",
                DateOfBirth = DateTime.Today.AddYears(-25),
                Password = "StrongPass@123",
                Role = "Employee",
                PhoneNumbers = new List<string> { "123456789" }
            };
            await _employeeService.CreateEmployeeAsync(command1, _currentEmployee);

            var command2 = new CreateEmployeeCommand
            {
                FirstName = "Jake",
                LastName = "Doe",
                Email = "jake.doe@example.com",
                DocumentNumber = "123213",
                DateOfBirth = DateTime.Today.AddYears(-30),
                Password = "AnotherPass@123",
                Role = "Employee",
                PhoneNumbers = new List<string> { "111222333" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _employeeService.CreateEmployeeAsync(command2, _currentEmployee));
        }

        [Fact]
        public async Task CreateEmployee_Should_ThrowException_When_CreatingHigherRole()
        {
            // Arrange: usuário líder tenta criar um diretor
            var leader = new Employee("Leader", "User", "leader@example.com", 565756, DateTime.Today.AddYears(-35), EmployeeRole.Leader);
            var command = new CreateEmployeeCommand
            {
                FirstName = "Mark",
                LastName = "Smith",
                Email = "mark.smith@example.com",
                DocumentNumber = "13123",
                DateOfBirth = DateTime.Today.AddYears(-28),
                Password = "Pass@123",
                Role = "Director", // Função superior à do líder
                PhoneNumbers = new List<string> { "222333444" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _employeeService.CreateEmployeeAsync(command, leader));
        }
    }

}
