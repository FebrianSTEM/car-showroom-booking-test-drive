using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.Application.Models.Responses;
using user_service.Application.Services.Interfaces;
using user_service.Domain.Common;
using user_service.Domain.Enums;

namespace user_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        protected HttpResult HttpResults;
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StandardResponse<IEnumerable<RoleResponse>>))]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                HttpResults = new StandardResponse<IEnumerable<RoleResponse>>(Enums.StatusCode.OK, Enums.StatusMessage.Success, roles, "");
                return StatusCode((int)StatusCodes.Status200OK, HttpResults);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roles");
                HttpResults = new StandardResponse<IEnumerable<RoleResponse>>(Enums.StatusCode.InternalServerErrorException, Enums.StatusMessage.Error, null, ex.Message);
                return StatusCode((int)StatusCodes.Status500InternalServerError, HttpResults);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(201, Type = typeof(StandardResponse<bool>))]
        public async Task<ActionResult> CreateRole([FromBody] RoleResponse roleDto)
        {
            try
            {
                await _roleService.CreateRoleAsync(roleDto.Name);
                HttpResults = new StandardResponse<bool>(Enums.StatusCode.Created, Enums.StatusMessage.Success, true, "");
                return StatusCode((int)StatusCodes.Status201Created, HttpResults);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while creating role");
                HttpResults = new StandardResponse<bool>(Enums.StatusCode.BadRequest, Enums.StatusMessage.Success, true, ex.Message);
                return StatusCode((int)StatusCodes.Status400BadRequest, HttpResults);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error occurred while creating role");
                HttpResults = new StandardResponse<bool>(Enums.StatusCode.Conflict, Enums.StatusMessage.Success, true, ex.Message);
                return StatusCode((int)StatusCodes.Status409Conflict, HttpResults);
            }
        }
    }
}
