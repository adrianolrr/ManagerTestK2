using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Models;
using ManagerTestK2.Services.DTO;
using ManagerTestK2.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTestK2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticate _authenticateService;
        private readonly IUserService _userService;

        public LoginController(IAuthenticate authenticateService, IUserService userService)
        {
            _authenticateService = authenticateService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserToken>> Register([FromBody] UserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                    return BadRequest("Dados inválido.");

                var emailExists = await _authenticateService.UserExists(userDTO.Email);

                if (emailExists == true)
                    return BadRequest("Email ja cadastrado.");

                var user = await _userService.AddUser(userDTO);
                if (user == null)
                    return BadRequest("Erro ao cadastrar usuario.");

                var token = _authenticateService.GenerateToken(user.Id, user.Email);
                return new UserToken
                {
                    Token = token,
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login(LoginModel loginModel)
        {
            try
            {
                var userExists = await _authenticateService.UserExists(loginModel.Email);
                if (!userExists)
                    return Unauthorized("Usuario não encontrado.");

                var result = await _authenticateService.AuthenticateAsync(loginModel.Email, loginModel.Password);
                if (!result)
                    return Unauthorized("Email ou senha invalido(s).");

                var user = await _authenticateService.GetUserByEmail(loginModel.Email);

                var token = _authenticateService.GenerateToken(user.Id, user.Email);

                return new UserToken { Token = token, };
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
