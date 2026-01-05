using Application.DTOs.ApiResponseDTO;
using ChemXLabWebAPI.DataHandler.ExceptionMidleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChemXLabWebAPI.DataHandler.Exceptions
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiExceptionResponse ex)
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    statusCode = ex.StatusCode,
                    message = ex.Message,
                    data = (object?)null
                })
                {
                    StatusCode = ex.StatusCode
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
