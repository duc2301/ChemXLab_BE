using Application.DTOs.RequestDTOs.Sepay;
using Application.Interfaces.IServices;
using Application.Services;
using ChemXLabWebAPI.DataHandler.Exceptions;
using ChemXLabWebAPI.Extensions;
using ChemXLabWebAPI.Hubs;
using Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer(); ;

builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddGlobalValidation(builder.Configuration);
builder.Services.AuthenticationServices(builder);
builder.Services.SwaggerServices(builder);

builder.Services.Configure<SePaySettings>(
    builder.Configuration.GetSection("SePay"));


builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ILLMService, GeminiLLMService>();

builder.Services.AddSingleton<IConversationMemoryService, ConversationMemoryService>();

builder.Services.AddScoped<IChemistryToolkit, ChemistryToolkit>();

builder.Services.AddScoped<IAIChemistryAgent, AIChemistryAgent>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<PaymentHub>("/PaymentHub");

app.MapControllers();

app.Run();
