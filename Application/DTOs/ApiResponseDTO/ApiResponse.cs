using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ApiResponseDTO
{
    public class ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public object? Result { get; set; }
        public object? Errors { get; set; }

        public static ApiResponse Success(string message = "Success", object? result = null)
            => new()
            {
                Message = message,
                IsSuccess = true,
                Result = result
            };

        public static ApiResponse Fail(string message, object? error = null)
            => new()
            {
                Message = message,
                IsSuccess = false,
                Errors = error
            };
    }
}
