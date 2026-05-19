using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VisionArt.Api.Configuration;
using VisionArt.Api.Context;
using VisionArt.Api.Helpers;
using VisionArt.Api.IServices;
using VisionArt.Api.Models;
using VisionArt.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ArtDbContext>(
    optionsBuilder =>
        optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("ArtVisionAppDB"))
);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<ArtDbContext>()
    .AddDefaultTokenProviders();

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Key))
    };
});

builder.Services.AddOptionsWithValidateOnStart<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddOptionsWithValidateOnStart<CorsOptions>()
    .Bind(builder.Configuration.GetSection(CorsOptions.SectionName));
builder.Services.AddOptionsWithValidateOnStart<PythonApiOptions>()
    .Bind(builder.Configuration.GetSection(PythonApiOptions.SectionName));
builder.Services.AddOptionsWithValidateOnStart<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName));

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var corsOptions = builder.Configuration
            .GetSection(CorsOptions.SectionName)
            .Get<CorsOptions>();

        var origins = corsOptions?.AllowedOrigins ?? ["http://localhost:5137"];
        policy.WithOrigins(origins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDataImportService, DataImportService>();
builder.Services.AddScoped<IArtService, ArtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtTokenGenerator>();

var pythonApiOptions = builder.Configuration
    .GetSection(PythonApiOptions.SectionName)
    .Get<PythonApiOptions>() ?? new PythonApiOptions();

builder.Services.AddHttpClient("PythonApi", client =>
{
    client.BaseAddress = new Uri(pythonApiOptions.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Art API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
