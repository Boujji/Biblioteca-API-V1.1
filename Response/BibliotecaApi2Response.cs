namespace BibliotecaApi2.Responses
{
    public class BibliotecaApi2Response<T>
    {
        public bool Success { get ; set; }
        public string Message { get ; set; } = string.Empty;
        public string? ErrorCode { get ; set; }
        public T? Data { get ; set; }

        public static BibliotecaApi2Response<T> Ok (T data, string message = "Operacion exitosa")
        {
            return new BibliotecaApi2Response<T>
            {
                Success = true,
                Message = message,
                ErrorCode = null,
                Data = data
            };
        }

        public static BibliotecaApi2Response<T> Fail(string message, string? errorCode = null)
        {
            return new BibliotecaApi2Response<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = default,
            };
        }
    }
}