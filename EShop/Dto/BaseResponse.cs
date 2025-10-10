namespace EShop.Dto
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public static BaseResponse<T> SuccessResponse(T data, string message = "Operation Sucessful")
        {
            return new BaseResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }
        public static BaseResponse<T> FailResponse(string message)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Data = default,
                Message = message
            };
        }
    }
}
