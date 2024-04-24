using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoExample.Services;
using System.Text;
using WeatherApp.Context.DBSettings;
using WeatherApp.Context.User;
using WeatherApp.Context.WeatherLocation;
using WeatherApp.Helpers;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:3000", "http://192.168.1.45:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.Configure<MongoDBSettings>(_configuration.GetSection("MongoDB"));
            services.Configure<JWTSettings>(_configuration.GetSection("JWTSettings"));

            services.AddScoped<IMongoDBService, MongoDBService>();
            services.AddScoped<IWeatherLocationService, WeatherLocationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSignalR();

            string jwtSecretKey = _configuration.GetValue<string>("JWTSettings:SecretKey") ?? "";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
                // Add more policies as needed
            });

            // Add Swagger generation
            services.AddSwaggerGen(swagger =>
            {
                // Configure Swagger document
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = ".NET 8 Web API"
                });

                // Add JWT Authentication support to Swagger
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                // Add JWT Authorization requirement
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigin");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Add your JWT middleware here if it's a custom middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalrHub>("/signalrHub");
            });

            app.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Resource not found.");
            });
        }
    }
}
