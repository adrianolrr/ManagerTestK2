using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Extensions;
using ManagerTestK2.Models;
using ManagerTestK2.Services.DTO;
using ManagerTestK2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTestK2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticate _authenticateService;

        public UserController(IUserService userService, IAuthenticate authenticate)
        {
            _userService = userService;
            _authenticateService = authenticate;
        }

        [HttpGet("Users")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationParams paginationParams)
        {
            try
            {
                var users = await _userService.GetAllUsers(paginationParams.PageNumber, paginationParams.PageSize);

                if (users == null)
                    return NotFound("Usuários não encontrados.");

                Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
                return Ok(new
                {
                    Total = users.TotalCount,
                    data = users
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpGet("{idUser:int}")]
        public async Task<ActionResult<UserDTO>> Get(int idUser)
        {
            try
            {
                var user = await _userService.GetUserById(idUser);
                if (user == null)
                    return NotFound("Usuário não encontrado.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(UserDTO userDTO)
        {
            try
            {
                var userIdClaim = int.Parse(User.FindFirst("id").Value);
                var userClaim = await _userService.GetUserById(userIdClaim);

                if (!userClaim.IsAdmin)
                    return Unauthorized("Você não tem permissão para atualizar este usuário.");


                if (userDTO == null)
                    return BadRequest("Dados inválidos");

                var userUpdated = await _userService.UpdateUser(userDTO);

                if (userUpdated == null)
                    return BadRequest("Erro ao atualizar usuário.");

                return Ok("Usuário atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }

        }

        [HttpDelete("{idUser:int}")]
        public async Task<ActionResult> Delete(int idUser)
        {
            try
            {
                var userIdClaim = int.Parse(User.FindFirst("id").Value);
                var userClaim = await _userService.GetUserById(userIdClaim);

                if (!userClaim.IsAdmin)
                    return Unauthorized("Você não tem permissão para excluir este usuário.");


                var user = await _userService.DeleteUser(idUser);

                if (user == null)
                    return BadRequest("Erro ao excluir usuário.");

                return Ok("Usuário excluído com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }
    }
}
