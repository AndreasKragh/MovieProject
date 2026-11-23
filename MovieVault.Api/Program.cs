using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data; 
using Scalar.AspNetCore;
using Serilog;
using MovieVault.Api.Middleware;

using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<MovieVaultContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//AUTHENTICATION
builder.Services.AddIdentity<ApiUser, IdentityRole>(options =>
{
options.Password.RequireDigit = true; 
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<MovieVaultContext>();    


builder.Services.AddAuthentication(options => {
options.DefaultAuthenticateScheme =
options.DefaultChallengeScheme =
options.DefaultForbidScheme =
options.DefaultScheme =
options.DefaultSignInScheme =
options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidIssuer = builder.Configuration["JWT:Issuer"],
ValidateAudience = true,
ValidAudience = builder.Configuration["JWT:Audience"],
ValidateIssuerSigningKey = true,
IssuerSigningKey = new SymmetricSecurityKey(
System.Text.Encoding.UTF8.GetBytes(
builder.Configuration["JWT:SigningKey"]))
};
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        return Task.CompletedTask;
    });
});

var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
