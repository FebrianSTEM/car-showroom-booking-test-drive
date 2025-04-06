using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using user_service.Application.Models.Requests;
using user_service.Application.Models.Responses;
using user_service.Application.Services.Interfaces;
using user_service.Domain.Common;
using user_service.Domain.Entities;
using user_service.Domain.Enums;
using user_service.Infrastructure.Repositories;

namespace user_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        protected HttpResult HttpResults;
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(201, Type = typeof(StandardResponse<UserResponse>))]
        public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest req)
        {
            try
            {
                var result = await _accountService.RegisterAsync(req);
                string message = $"User {req.Email} successfuly registered";
                HttpResults = new StandardResponse<UserResponse>(Enums.StatusCode.Created, Enums.StatusMessage.Success, result, message);
                return StatusCode((int)StatusCodes.Status201Created, HttpResults);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<UserResponse>(Enums.StatusCode.BadRequest, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status400BadRequest, HttpResults);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<UserResponse>(Enums.StatusCode.Conflict, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status409Conflict, HttpResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<AuthResponse>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(StandardResponse<AuthResponse>))]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
        {
            try
            {
                var result = await _accountService.LoginAsync(req);
                string message = $"User {req.Email} successfuly logged in";
                HttpResults = new StandardResponse<AuthResponse>(Enums.StatusCode.OK, Enums.StatusMessage.Success, result, message);
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<AuthResponse>(Enums.StatusCode.Unauthorized, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status401Unauthorized, HttpResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<AuthResponse>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(200, Type = typeof(StandardResponse<UserResponse>))]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    HttpResults = new StandardResponse<bool?>(Enums.StatusCode.Unauthorized, Enums.StatusMessage.Error, null, "unauthorized !");
                    return StatusCode((int)StatusCodes.Status401Unauthorized, HttpResults);
                }
                var result = await _accountService.GetUserByIdAsync(userGuid);
                HttpResults = new StandardResponse<UserResponse>(Enums.StatusCode.OK, Enums.StatusMessage.Success, result, "JWT Confirmed !");
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<UserResponse>(Enums.StatusCode.NotFound, Enums.StatusMessage.Error, null, "User not found !");
                return StatusCode((int)StatusCodes.Status404NotFound, HttpResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(200, Type = typeof(StandardResponse<bool>))]
        public async Task<ActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            try
            {
                await _accountService.ConfirmEmailAsync(userId, token);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.OK, Enums.StatusMessage.Success, null, "Email confirmed successfully");
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.NotFound, Enums.StatusMessage.Error, null, "User not Found");
                return StatusCode((int)StatusCodes.Status404NotFound, HttpResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpPost("confirm-phone")]
        [ProducesResponseType(200, Type = typeof(StandardResponse<bool>))]
        public async Task<ActionResult> ConfirmPhoneNumber([FromQuery] Guid userId, [FromQuery] string token)
        {
            try
            {
                await _accountService.ConfirmPhoneNumberAsync(userId, token);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.OK, Enums.StatusMessage.Success, null, "Phone number confirmed successfully");
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.NotFound, Enums.StatusMessage.Error, null, "User not Found");
                return StatusCode((int)StatusCodes.Status404NotFound, HttpResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpPost("add-to-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(StandardResponse<bool>))]
        public async Task<ActionResult> AddToRole([FromBody] UserRoleRequest userRoleDto)
        {
            try
            {
                await _accountService.AddUserToRoleAsync(userRoleDto.UserId, userRoleDto.RoleName);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.OK, Enums.StatusMessage.Success, null, $"User added to role '{userRoleDto.RoleName}' successfully");
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.NotFound, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status404NotFound, HttpResults);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpPost("remove-from-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(StandardResponse<bool>))]
        public async Task<ActionResult> RemoveFromRole([FromBody] UserRoleRequest userRoleDto)
        {
            try
            {
                await _accountService.RemoveUserFromRoleAsync(userRoleDto.UserId, userRoleDto.RoleName);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.OK, Enums.StatusMessage.Success, null, $"User removed from role '{userRoleDto.RoleName}' successfully");
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.NotFound, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status404NotFound, HttpResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpGet("roles")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(StandardResponse<IList<string>>))]
        public async Task<ActionResult<IList<string>>> GetUserRoles()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    return Unauthorized();

                var roles = await _accountService.GetUserRolesAsync(userGuid);
                return Ok(roles);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.NotFound, Enums.StatusMessage.Error, null, "User not found");
                return StatusCode((int)StatusCodes.Status404NotFound, HttpResults);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                HttpResults = new StandardResponse<bool?>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }
    }
}
