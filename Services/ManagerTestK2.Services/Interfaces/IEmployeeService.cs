using ManagerTestK2.Domain.Entities;
using ManagerTestK2.Domain.Pagination;
using ManagerTestK2.Services.Commands;
using ManagerTestK2.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTestK2.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployeeAsync(CreateEmployeeCommand command, Employee currentEmployee);
        Task<EmployeeDTO> GetEmployeeById(int idEmployee);
        Task<PagedList<EmployeeDTO>> GetAllEmployees(int pageNumber, int pageSize);
        Task<EmployeeDTO> UpdateEmployee(EmployeeDTO employeeDTO);
        Task<EmployeeDTO> DeleteEmployee(int idEmployee);
    }
}
