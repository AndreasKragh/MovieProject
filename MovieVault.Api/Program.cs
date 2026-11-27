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
using MovieVault.Api.Constants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MovieVault.Api.Authorization;

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


//AUTHORIZATION
// Claims-based policies (slide 50-52): hver policy kræver et role-claim med bestemte værdier.
// Role-claimet kommer fra tokenet, som AccountController.Login har lagt det i.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy => policy
        .RequireClaim(ClaimTypes.Role, RoleNames.Admin));

    options.AddPolicy(Policies.ActorDirectorOrAdmin, policy => policy
        .RequireClaim(ClaimTypes.Role, RoleNames.Actor, RoleNames.Director, RoleNames.Admin));

    options.AddPolicy(Policies.ActorOrAdmin, policy => policy
        .RequireClaim(ClaimTypes.Role, RoleNames.Actor, RoleNames.Admin));

    // Resource-based policies (slide 62): består af et requirement, som en handler afgør
    options.AddPolicy(Policies.CanViewMovie, policy => policy
        .AddRequirements(new CanViewMovieRequirement()));

    options.AddPolicy(Policies.CanViewActorPerformance, policy => policy
        .AddRequirements(new CanViewActorPerformanceRequirement()));
});

// Handlerne registreres i DI (slide 59/64). Scoped, fordi MovieAccessHandler bruger DbContext.
builder.Services.AddScoped<IAuthorizationHandler, MovieAccessHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ActorPerformanceHandler>();


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

        // Sig til OpenAPI/Scalar, at Bearer-skemaet skal BRUGES på requests.
        // Uden dette er skemaet kun defineret, og Scalar sender ikke Authorization-headeren.
        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            }
        ];
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






// Seed roller og demo-brugere (slide 39)
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<ApiUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var context = serviceProvider.GetRequiredService<MovieVaultContext>();
    await UserSeeder.SeedUsersAsync(userManager, roleManager, context, app.Configuration);
}

app.Run();
