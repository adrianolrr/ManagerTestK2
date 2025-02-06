using ManagerTestK2.Domain.Entities;
using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Extensions;
using ManagerTestK2.Models;
using ManagerTestK2.Services.Commands;
using ManagerTestK2.Services.DTO;
using ManagerTestK2.Services.Interfaces;
using ManagerTestK2.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTestK2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand command)
        {
          var currentEmployee = new Employee("Admin", "Employee", "admin@example.com", 1231232, DateTime.Today.AddYears(-30), EmployeeRole.Director);
            currentEmployee.SetPassword("Admin@123");

            try
            {
                var employee = await _employeeService.CreateEmployeeAsync(command, currentEmployee);
                return CreatedAtAction(nameof(Employee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("Employees")]
        public async Task<ActionResult<List<EmployeeDTO>>> Get([FromQuery] PaginationParams paginationParams)
        {
            var employees = await _employeeService.GetAllEmployees(paginationParams.PageNumber, paginationParams.PageSize);

            if (employees == null)
                return NotFound("Empregados não encontrados.");

            Response.AddPaginationHeader(new PaginationHeader(employees.CurrentPage, employees.PageSize, employees.TotalCount, employees.TotalPages));
            return Ok(new
            {
                Total = employees.TotalCount,
                data = employees
            });
        }

        [HttpGet("{idEmployee:int}")]
        public async Task<ActionResult<EmployeeDTO>> Get(int idEmployee)
        {

            var employee = await _employeeService.GetEmployeeById(idEmployee);
            if (employee == null)
                return NotFound("Empregado não encontrado.");

            return Ok(employee);
        }

        [HttpPut]
        public async Task<ActionResult> Update(EmployeeDTO employeeDTO)
        {
            var employee = await _employeeService.GetEmployeeById(employeeDTO.Id);

            if (employeeDTO == null)
                return BadRequest("Dados inválidos");

            var employeeUpdated = await _employeeService.UpdateEmployee(employeeDTO);

            if (employeeUpdated == null)
                return BadRequest("Erro ao atualizar usuário.");

            return Ok("Empregado atualizado com sucesso.");
        }

        [HttpDelete("{idEmployee:int}")]
        public async Task<ActionResult> Delete(int idEmployee)
        {
            var employee = await _employeeService.GetEmployeeById(idEmployee);

            await _employeeService.DeleteEmployee(idEmployee);

            if (employee == null)
                return BadRequest("Erro ao excluir usuário.");

            return Ok("Empregado excluído com sucesso");
        }
    }
}
