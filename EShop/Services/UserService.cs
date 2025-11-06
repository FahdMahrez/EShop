using BCrypt.Net;
using EShop.Configurations;
using EShop.Data;
using EShop.Dto;
using EShop.Dto.UserModel;
using EShop.Repositories;
using EShop.Repositories.Interface;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace EShop.Services
{
    public class UserService(IUserRepository userRepository, ITokenService tokenService, IUserRoleRepository userRoleRepository) : IUserService
    {
        public async Task<BaseResponse<UserDto>> CreateAsync(UserDto userDto)
        {
            try
            {
                Log.Information("Attempting to create a new user with email: {Email}", userDto.Email);
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
                if (!Regex.IsMatch(userDto.Email, emailPattern))
                {
                    Log.Warning("Invalid email format for user creation: {Email}", userDto.Email);
                    return BaseResponse<UserDto>.FailResponse("Invalid email. Only @gmail.com addresses are allowed.");
                }

                var existing = await userRepository.GetByEmailAsync(userDto.Email);
                if (existing != null)
                {
                    Log.Warning("Attempt to register already existing email: {Email}", userDto.Email);
                    return BaseResponse<UserDto>.FailResponse("Email already registered.");
                }

                string passwordHash = string.Empty;
                if (!string.IsNullOrWhiteSpace(userDto.Password))
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    OtherName = userDto.OtherName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    Address = userDto.Address,
                    Gender = Enum.TryParse<Gender>(userDto.Gender, true, out var gender) ? gender : Gender.Unknown,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow
                };

                var success = await userRepository.CreateAsync(user, CancellationToken.None);
                if (!success)
                {
                    Log.Warning("Failed to create user with email: {Email}", userDto.Email);
                    return BaseResponse<UserDto>.FailResponse("Failed to create user.");
                }
                userDto.Id = user.Id;
                Log.Information("User created successfully with Id: {UserId}", user.Id);
                return BaseResponse<UserDto>.SuccessResponse(userDto, "User created successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating user with email: {Email}", userDto.Email);
                return BaseResponse<UserDto>.FailResponse("An error occurred while creating user.");
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                Log.Information("Attempting to delete user with Id: {UserId}", id);
                var user = await userRepository.GetByIdAsync(id, CancellationToken.None);
                if (user == null)
                {
                    Log.Warning("User with Id: {UserId} not found for deletion", id);
                    return BaseResponse<bool>.FailResponse("User not found.");
                }
                var success = await userRepository.DeleteAsync(user, CancellationToken.None);
                if (!success)
                {
                    Log.Warning("Failed to delete user with Id: {UserId}", id);
                    return BaseResponse<bool>.FailResponse("Failed to delete user.");
                }
                Log.Information("User deleted successfully with Id: {UserId}", id);
                return BaseResponse<bool>.SuccessResponse(true, "User deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting user with Id: {UserId}", id);
                return BaseResponse<bool>.FailResponse("An error occurred while deleting user.");
            }
        }

        public async Task<BaseResponse<IEnumerable<UserDto>>> GetAllAsync()
        {
            try
            {
                Log.Information("Retrieving all users...");
                var users = await userRepository.GetAllAsync(CancellationToken.None);
                if (users == null || !users.Any())
                {
                    Log.Warning("No users found in the database");
                    return BaseResponse<IEnumerable<UserDto>>.FailResponse("No users found.");
                }
                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    OtherName = u.OtherName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Gender = u.Gender.ToString()
                });
                Log.Information("Successfully retrieved {UserCount} users", users.Count());
                return BaseResponse<IEnumerable<UserDto>>.SuccessResponse(userDtos, "Users retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching all users");
                return BaseResponse<IEnumerable<UserDto>>.FailResponse("An error occurred while retrieving users.");
            }
        }

        public async Task<BaseResponse<UserDto>> UpdateAsync(Guid id, UserDto userDto)
        {
            try
            {
                Log.Information("Updating user with Id: {UserId}", id);

                var emailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
                if (!Regex.IsMatch(userDto.Email, emailPattern))
                {
                    Log.Warning("Invalid email format for user update: {Email}", userDto.Email);
                    return BaseResponse<UserDto>.FailResponse("Invalid email. Only @gmail.com addresses are allowed.");
                }
                var existingUser = await userRepository.GetByIdAsync(id, CancellationToken.None);
                if (existingUser == null)
                {
                    Log.Warning("User with Id: {UserId} not found for update", id);
                    return BaseResponse<UserDto>.FailResponse("User not found.");
                }
                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.OtherName = userDto.OtherName;
                existingUser.Email = userDto.Email;
                existingUser.PhoneNumber = userDto.PhoneNumber;
                existingUser.Address = userDto.Address;
                existingUser.Gender = Enum.TryParse<Gender>(userDto.Gender, true, out var gender) ? gender : existingUser.Gender;

                var success = await userRepository.UpdateAsync(existingUser, CancellationToken.None);
                if (!success)
                    return BaseResponse<UserDto>.FailResponse("Failed to update user.");

                return BaseResponse<UserDto>.SuccessResponse(userDto, "User updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating user.");
                return BaseResponse<UserDto>.FailResponse("An error occurred while updating user.");
            }
        }

        async Task<BaseResponse<IEnumerable<UserDto>>> IUserService.GetAllAsync()
        {
            try
            {
                var users = await userRepository.GetAllAsync(CancellationToken.None);
                if (users == null || !users.Any())
                    return BaseResponse<IEnumerable<UserDto>>.FailResponse("No users found.");

                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    OtherName = u.OtherName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Gender = u.Gender.ToString()
                });

                return BaseResponse<IEnumerable<UserDto>>.SuccessResponse(userDtos, "Users retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching users.");
                return BaseResponse<IEnumerable<UserDto>>.FailResponse("An error occurred while retrieving users.");
            }
        }

        async Task<BaseResponse<UserDto>> IUserService.UpdateAsync(Guid id, UserDto userDto)
        {
            try
            {
                Log.Information("Updating user with Id: {UserId}", id);
                var existingUser = await userRepository.GetByIdAsync(id, CancellationToken.None);
                if (existingUser == null)
                {
                    Log.Warning("User with Id: {UserId} not found for update", id);
                    return BaseResponse<UserDto>.FailResponse("User not found.");
                }
                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.OtherName = userDto.OtherName;
                existingUser.Email = userDto.Email;
                existingUser.PhoneNumber = userDto.PhoneNumber;
                existingUser.Address = userDto.Address;
                existingUser.Gender = Enum.TryParse<Gender>(userDto.Gender, true, out var gender) ? gender : existingUser.Gender;

                var success = await userRepository.UpdateAsync(existingUser, CancellationToken.None);
                if (!success)
                {
                    Log.Warning("Failed to update user with Id: {UserId}", id);
                    return BaseResponse<UserDto>.FailResponse("Failed to update user.");
                }
                Log.Information("User with Id: {UserId} updated successfully", id);
                return BaseResponse<UserDto>.SuccessResponse(userDto, "User updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating user with Id: {UserId}", id);
                return BaseResponse<UserDto>.FailResponse("An error occurred while updating user.");
            }
        }
       
        public async Task<BaseResponse<User?>> RegisterUserAsync(string email, string password)
        {
            try
            {
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
                if (!Regex.IsMatch(email, emailPattern))
                    return BaseResponse<User?>.FailResponse("Invalid email. Only @gmail.com addresses are allowed.");

                var existing = await userRepository.GetByEmailAsync(email);
                if (existing != null)
                    return BaseResponse<User?>.FailResponse("Email already registered.");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    CreatedAt = DateTime.UtcNow
                };

                var success = await userRepository.CreateAsync(user, CancellationToken.None);
                return success
                    ? BaseResponse<User?>.SuccessResponse(user, "User registered successfully.")
                    : BaseResponse<User?>.FailResponse("Failed to register user.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error registering user {Email}", email);
                return BaseResponse<User?>.FailResponse("An error occurred while registering user.");
            }
        }

        public async Task<BaseResponse<User?>> ValidateUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            try
            {
                var user = await userRepository.GetByEmailAsync(email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    Log.Warning("Invalid login attempt for {Email}", email);
                    return BaseResponse<User?>.FailResponse("Invalid credentials.");
                }

                Log.Information("User validated: {Email}", email);
                return BaseResponse<User?>.SuccessResponse(user, "Login successful.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error validating user {Email}", email);
                return BaseResponse<User?>.FailResponse("Error during validation.");
            }
        }

        public async Task<BaseResponse<User?>> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                var user = await userRepository.GetByRefreshTokenAsync(refreshToken, CancellationToken.None);
                return user == null
                    ? BaseResponse<User?>.FailResponse("Invalid refresh token.")
                    : BaseResponse<User?>.SuccessResponse(user, "User retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching user by refresh token");
                return BaseResponse<User?>.FailResponse("An error occurred while retrieving refresh token.");
            }
        }

        public async Task<BaseResponse<bool>>UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryDate, CancellationToken cancellationToken)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId, CancellationToken.None);
                if (user == null)
                    return BaseResponse<bool>.FailResponse("User not found.");

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = expiryDate;

                var success = await userRepository.UpdateAsync(user, CancellationToken.None);
                return success
                    ? BaseResponse<bool>.SuccessResponse(true, "Refresh token updated successfully.")
                    : BaseResponse<bool>.FailResponse("Failed to update refresh token.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating refresh token for user {UserId}", userId);
                return BaseResponse<bool>.FailResponse("An error occurred while updating refresh token.");
            }
        }

        public async Task<BaseResponse<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Fetching user with Id: {UserId}", id);
                var user = await userRepository.GetByIdAsync(id, CancellationToken.None);
                if (user == null)
                {
                    Log.Warning("User with Id: {UserId} not found", id);
                    return BaseResponse<UserDto>.FailResponse("User not found.");
                }
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    OtherName = user.OtherName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Gender = user.Gender.ToString()
                };
                Log.Information("Successfully retrieved user with Id: {UserId}", id);
                return BaseResponse<UserDto>.SuccessResponse(userDto, "User retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching user with Id: {UserId}", id);
                return BaseResponse<UserDto>.FailResponse("An error occurred while retrieving user.");
            }
        }

        public async Task<BaseResponse<User?>> LoginUserAsync(LoginRequest request)
        {
            try
            {
                Log.Information("Attempting login for {Email}", request.Email);

                var user = await userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                    return new BaseResponse<User?> { Success = false, Message = "Invalid email or password" };

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return new BaseResponse<User?> { Success = false, Message = "Invalid email or password" };

                var roles = await userRoleRepository.GetRolesByUserIdAsync(user.Id, CancellationToken.None);
                var accessToken = tokenService.GenerateToken(user, roles.Select(r => r.RoleName));
                var refreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                await userRepository.UpdateAsync(user, CancellationToken.None);

                Log.Information("User {Email} logged in successfully", user.Email);

                return new BaseResponse<User?>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while logging in {Email}", request.Email);
                return BaseResponse<User?>.FailResponse("An error occurred while logging in.");
            }
        }
    }
}
