using ManagerTestK2.Domain.Pagination;
using ManagerTestK2.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTestK2.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(int idUser);
        Task<PagedList<UserDTO>> GetAllUsers(int pageNumber, int pageSize);
        Task<UserDTO> AddUser(UserDTO userDTO);
        Task<UserDTO> UpdateUser(UserDTO userDTO);
        Task<UserDTO> DeleteUser(int idUser);
    }
}
