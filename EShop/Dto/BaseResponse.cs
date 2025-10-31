using EShop.Data;

namespace EShop.Dto
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
        public static BaseResponse<T> SuccessResponse(T data, string message = "Operation Sucessful", User? user = null)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                User = user
            };
        }
        public static BaseResponse<T> FailResponse(string message, User? user = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                User = user
            };
        }
    }
}
