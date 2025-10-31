using EShop.Data;
using EShop.Dto;
using EShop.Dto.UserModel;
using Microsoft.AspNetCore.Identity.Data;
using System.Threading.Tasks;

namespace EShop.Services.Interface
{
    public interface IUserService
    {
        Task<BaseResponse<User?>> RegisterUserAsync(string email, string password);
        Task<BaseResponse<User?>> LoginUserAsync(LoginRequest request);
        Task<BaseResponse<User?>> ValidateUserAsync(string email, string password, CancellationToken cancellationToken);
        Task<BaseResponse<User?>> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<BaseResponse<bool>>UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryDate, CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<UserDto>>> GetAllAsync();
        Task<BaseResponse<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse<UserDto>> CreateAsync(UserDto userDto);
        Task<BaseResponse<UserDto>> UpdateAsync(Guid id, UserDto userDto);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
