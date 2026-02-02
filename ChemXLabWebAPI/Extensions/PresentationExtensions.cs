using ChemXLabWebAPI.DataHandler.Validation;
using Microsoft.AspNetCore.Mvc;

namespace ChemXLabWebAPI.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring presentation layer services and behaviors.
    /// </summary>
    public static class PresentationExtensions
    {
        /// <summary>
        /// Configures global validation error handling to return a standardized API response format.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration properties.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGlobalValidation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var response = ValidationHandler.Handle(context.ModelState);

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}