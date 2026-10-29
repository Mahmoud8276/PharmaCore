using Asp.Versioning;
using Pharmacy.System.Api.Responses;
using Pharmacy.System.Core.Dtos.AccountDtos;
using Pharmacy.System.Core.Dtos.AppUserDtos;
using Pharmacy.System.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/v1/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            return FromResponse(
                await _accountService.RegisterUserAsync(dto));
        }


        // POST api/v1/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _accountService.LoginUserAsync(dto);
            if(result.IsSuccess)
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
            
            return FromResponse(result);
        }


        // GET api/v1/account/confirm-email
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            return FromResponse(
                await _accountService.ConfirmEmailAsync(userId, token));
        }

        // POST api/v1/account/confirm-email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            return FromResponse(
                await _accountService.SendConfirmEmailAsync(dto));
        }

        // POST api/v1/account/forget-password
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            return FromResponse(
                await _accountService.ForgetPasswordAsync(dto));
        }

        // POST api/v1/account/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            return FromResponse(
                await _accountService.ResetPasswordAsync(dto));
        }

        // GET api/v1/account/refresh-token
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return FromResponse(ApiResponse.Fail("Refresh token is required."));

            var result = await _accountService.RefreshTokenAsync(refreshToken);
            if (result.IsSuccess)
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
            
            return FromResponse(result);
        }

        // POST api/v1/account/revoke-token
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
        {
            var refreshToken = dto.RefreshToken ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return FromResponse(
                    ApiResponse.Fail("Refresh token is required."));

            return FromResponse(
                await _accountService.RevokeRefreshTokenAsync(refreshToken));
        }


        // GET api/v1/account/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserData()
        {
            return FromResponse(
                await _accountService.GetUserDataAsync());
        }

        // POST api/v1/account/change-password
        [Authorize]
        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            return FromResponse(
                await _accountService.ChangePasswordAsync(dto));
        }

        // PUT api/v1/account/me
        [Authorize]
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateUserData([FromForm] UpdateUserDto dto)
        {
            return FromResponse(
                await _accountService.UpdateUserDataAsync(dto));
        }

        // DELETE api/v1/account/me
        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUserData()
        {
            return FromResponse(
                await _accountService.DeleteUserDataAsync());
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
