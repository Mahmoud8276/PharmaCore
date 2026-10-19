using Pharmacy.System.Core.Dtos.AccountDtos;
using Pharmacy.System.Core.Dtos.AppUserDtos;
using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.Emails;
using Pharmacy.System.Services.Helpers;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{

    public class AccountService : IAccountService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<AccountService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        private async Task<string> GenerateConfirmEmailUrlAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = _configuration["Storage:BaseUrl"];
            var confirmEmailUrl = $"{baseUrl}/api/v1/account/confirm-email?userId={user.Id}&token={encodedToken}";
            return confirmEmailUrl;
        }

        private async Task<string> GenerateForgetPasswordUrlAsync(AppUser appUser, string clientUrl)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var forgetPasswordUrl = $"{clientUrl}?userId={appUser.Id}&token={encodedToken}";
            return forgetPasswordUrl;
        }
        private async Task RemoveUserImageAsync(string? imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                await FileHelper.DeleteFileAsync(imageName, "UserImages");
            }
        }




        public async Task<Response> RegisterUserAsync(RegisterDto dto)
        {
            if (await UserExistsAsync(dto.Email))
                return Response.Fail("User Already Exists");

            var appUser = new AppUser()
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber
            };

            if(dto.UserImage != null) 
            {
                var imageName = await FileHelper.UploadFileAsync(dto.UserImage, "UserImages");
                appUser.Image = imageName;
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(appUser, dto.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    await RemoveUserImageAsync(appUser.Image);
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    return Response.Fail(
                        message: "User registration failed",
                        details: $"{errors}");
                }

                result = await _userManager.AddToRoleAsync(appUser, AppUserRoles.User.ToString());
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    await RemoveUserImageAsync(appUser.Image);
                    var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                    return Response.InternalServerError(
                        message: "Failed to assign role to user",
                        details: $"{errors}");
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                await RemoveUserImageAsync(appUser.Image);
                throw;
            }

            try
            {
                await _emailService.SendAsync(new ConfirmAccountEmail(
                    to: appUser.Email,
                    recipientName: $"{appUser.FirstName} {appUser.LastName}",
                    firstName: appUser.FirstName,
                    emailConfirmationLink: await GenerateConfirmEmailUrlAsync(appUser)));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Registration succeeded but confirmation email failed to send for {Email}", appUser.Email);
                return Response.Success(message: "Registered successfully, but we couldn't send your confirmation email — please use \"resend confirmation\" to try again.");
            }

            return Response.Success(message: "registered successfully, please check your email to confirm your account");
        }

        public async Task<Response<LoginDetailsDto>> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.Users
                                        .Include(u=>u.RefreshTokens)
                                        .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                return Response<LoginDetailsDto>.Unauthorized("Invalid Email Or Password!");

            if(!user.EmailConfirmed)
                return Response<LoginDetailsDto>.Unauthorized("Email is not confirmed!");


            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if(!signInResult.Succeeded)
                return Response<LoginDetailsDto>.Unauthorized("Invalid Email Or Password!");

            RefreshToken refreshToken; 
            if(user.RefreshTokens.Any(token => token.IsActive))
            {
                refreshToken = user.RefreshTokens.First(token => token.IsActive);
            }
            else
            {
                refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            var userDetails = user.Adapt<UserDetailsDto>();
            userDetails.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return Response<LoginDetailsDto>.Success(new LoginDetailsDto()
            {
                User = userDetails,
                AccessToken = await _tokenService.GenerateAccessTokenAsync(user),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            });
        }

        public async Task<Response> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response.NotFound("User not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail(
                    message: "Email confirmation failed", 
                    details: $"{errors}");
            }

            return Response.Success(message:"Email confirmed successfully");
        }

        public async Task<Response> SendConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || user.EmailConfirmed)
                return Response.Success(message: "Check your inbox, please!");

            try
            {
                await _emailService.SendAsync(new ConfirmAccountEmail(
                    to: user.Email,
                    recipientName: $"{user.FirstName} {user.LastName}",
                    firstName: user.FirstName,
                    emailConfirmationLink: await GenerateConfirmEmailUrlAsync(user)));
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resend confirmation email for {Email}", user.Email);
            }
            
            return Response.Success(message: "Check your inbox, please!");
        }

        public async Task<Response> ForgetPasswordAsync(ForgetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
                return Response.Success(message: "Check your inbox");

            try
            {
                await _emailService.SendAsync(new ForgetPasswordEmail(
                    to: user.Email,
                    recipientName: user.FirstName,
                    resetLink: await GenerateForgetPasswordUrlAsync(user, dto.ClientUrl)
                ));
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send forget password email for {Email}", user.Email);
            }

            return Response.Success(message: "check your inbox");
        }

        public async Task<Response> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return Response.NotFound("User not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail(
                    message: "Password reset failed", 
                    details: $"{errors}");
            }

            return Response.Success(message: "Password has been reset successfully!");
        }

        public async Task<Response<LoginDetailsDto>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            
            if(user == null)
                return Response<LoginDetailsDto>.Fail("Invalid token");

            var token = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if(!token.IsActive)
                return Response<LoginDetailsDto>.Fail("Invalid token");

            token.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var userDetails = user.Adapt<UserDetailsDto>();
            userDetails.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return Response<LoginDetailsDto>.Success(new LoginDetailsDto()
            {
                User = userDetails,
                AccessToken = await _tokenService.GenerateAccessTokenAsync(user),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            });
        }

        public async Task<Response> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
                return Response.Fail("Invalid token");

            var token = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if (!token.IsActive)
                return Response.Fail("Invalid token");

            token.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Response.Success(message: "Refresh token revoked successfully");
        }

        public async Task<Response> GetUserDataAsync()
        {
            var userId = _httpContextAccessor.HttpContext.GetRequiredUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) 
                return Response.NotFound(message: "User does not exist!");

            var userDetails = user.Adapt<UserDetailsDto>();
            userDetails.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return Response.Success(data: userDetails);
        }

        public async Task<Response> UpdateUserDataAsync(UpdateUserDto dto)
        {
            var userId = _httpContextAccessor.HttpContext.GetRequiredUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response.NotFound(message: "User does not exist!");

            var previousImage = user.Image;

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            string? newlyUploadedImage = null;
            if (dto.RemoveImage)
            {
                user.Image = null;
            }
            else if (dto.UserImage != null)
            {
                newlyUploadedImage = await FileHelper.UploadFileAsync(dto.UserImage, "UserImages");
                user.Image = newlyUploadedImage;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (newlyUploadedImage != null)
                    await RemoveUserImageAsync(newlyUploadedImage);

                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail(
                    message: "Failed to update profile",
                    details: errors);
            }

            if ((dto.RemoveImage || newlyUploadedImage != null) && !string.IsNullOrEmpty(previousImage))
                await RemoveUserImageAsync(previousImage);

            var userDetails = user.Adapt<UserDetailsDto>();
            userDetails.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return Response.Success(userDetails);
        }

        public async Task<Response> DeleteUserDataAsync()
        {
            var userId = _httpContextAccessor.HttpContext.GetRequiredUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response.NotFound(message: "User does not exist!");

            var previousImage = user.Image;

            var result = await _userManager.DeleteAsync(user);
            if(!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail(
                    message: "Failed to delete user",
                    details: errors);
            }

            await RemoveUserImageAsync(previousImage);

            return Response.Success(message: "User deleted successfully");
        }

        public async Task<Response> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userId = _httpContextAccessor.HttpContext.GetRequiredUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                return Response.NotFound(message: "User does not exist!");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return Response.Fail(
                    message: "Failed to change user password",
                    details: errors);
            }

            return Response.Success(message: "User password has been changed successfully");
        }
    }
}
