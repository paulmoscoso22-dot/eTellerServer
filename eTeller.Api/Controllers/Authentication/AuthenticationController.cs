using eTeller.Application.Features.User.DTOs;
using eTeller.Application.Features.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eTeller.Api.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate user with credentials
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = "INVALID_REQUEST",
                        Message = "Username and password are required."
                    });
                }

                // Get client IP address
                request.IpAddress = GetClientIpAddress();

                var result = await _authenticationService.LoginAsync(request);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed login attempt for user: {UserId} from IP: {IpAddress}", 
                        request.UserId, request.IpAddress);
                    return Unauthorized(result);
                }

                _logger.LogInformation("Successful login for user: {UserId} from IP: {IpAddress}", 
                    request.UserId, request.IpAddress);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {UserId}", request.UserId);
                return StatusCode(500, new LoginResponseDto
                {
                    IsSuccessful = false,
                    ResultCode = "ERROR",
                    Message = "An error occurred during login. Please try again later."
                });
            }
        }

        /// <summary>
        /// Force login (disconnect existing session)
        /// </summary>
        [HttpPost("force-login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> ForceLogin([FromBody] LoginRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new LoginResponseDto
                    {
                        IsSuccessful = false,
                        ResultCode = "INVALID_REQUEST",
                        Message = "Username and password are required."
                    });
                }

                request.IpAddress = GetClientIpAddress();

                var result = await _authenticationService.ForceLoginAsync(request);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed force login attempt for user: {UserId} from IP: {IpAddress}", 
                        request.UserId, request.IpAddress);
                    return Unauthorized(result);
                }

                _logger.LogInformation("Successful force login for user: {UserId} from IP: {IpAddress}", 
                    request.UserId, request.IpAddress);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during force login for user: {UserId}", request.UserId);
                return StatusCode(500, new LoginResponseDto
                {
                    IsSuccessful = false,
                    ResultCode = "ERROR",
                    Message = "An error occurred during login. Please try again later."
                });
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ChangePasswordResponseDto>> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            try
            {
                // Get current user from token
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                // Ensure user can only change their own password (or admin can change any)
                if (request.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var result = await _authenticationService.ChangePasswordAsync(request);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed password change for user: {UserId}", request.UserId);
                    return BadRequest(result);
                }

                _logger.LogInformation("Successful password change for user: {UserId}", request.UserId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user: {UserId}", request.UserId);
                return StatusCode(500, new ChangePasswordResponseDto
                {
                    IsSuccessful = false,
                    Message = "An error occurred during password change. Please try again later."
                });
            }
        }

        /// <summary>
        /// Validate user credentials without creating a session
        /// </summary>
        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<ActionResult<ValidateUserResponseDto>> ValidateUser([FromBody] ValidateUserRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ValidateUserResponseDto
                    {
                        IsValid = false,
                        Message = "Username and password are required."
                    });
                }

                var result = await _authenticationService.ValidateUserAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user validation for user: {UserId}", request.UserId);
                return StatusCode(500, new ValidateUserResponseDto
                {
                    IsValid = false,
                    Message = "An error occurred during validation. Please try again later."
                });
            }
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(userId))
                {
                    var success = await _authenticationService.LogoutAsync(userId);
                    
                    if (success)
                    {
                        _logger.LogInformation("User logged out: {UserId}", userId);
                        return Ok(new { message = "Logged out successfully." });
                    }
                }

                return BadRequest(new { message = "Logout failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }

        /// <summary>
        /// Get client IP address from request
        /// Handles X-Forwarded-For header for proxy scenarios
        /// </summary>
        private string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Check for proxy headers
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }
            else if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = Request.Headers["X-Real-IP"].FirstOrDefault();
            }

            // Handle IPv6 loopback
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress ?? "Unknown";
        }
    }
}
