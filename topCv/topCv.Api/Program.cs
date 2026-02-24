using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using topCv.Application.Common;
using topCv.Application.Interfaces.Auth;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Services.Auth;
using topCv.Application.Services.Commons;
using topCv.Domain.Common;
using topCv.Infrastructure.Persistence;
using topCv.Infrastructure.Repositories;
using topCv.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// DI
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<IAppDbContext, AppDbContext>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHashService, Sha256HashService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<IResumeFileService, ResumeFileService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<ISavedJobService, SavedJobService>();
builder.Services.AddScoped<IFollowCompanyService, FollowCompanyService>();
builder.Services.AddScoped<IEmployerDashboardService, EmployerDashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IJobMatchService, JobMatchService>();
builder.Services.AddScoped<IWardService, WardService>();

//
builder.Services.AddHttpClient<ProvinceWardSeedService>();
builder.Services.AddScoped<ProvinceWardSeedService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CandidateOnly", p =>
        p.RequireRole(AppRoles.Candidate));

    options.AddPolicy("EmployerOnly", p =>
        p.RequireRole(AppRoles.Employer));

    options.AddPolicy("AdminOnly", p =>
        p.RequireRole(AppRoles.Admin));
});

//Test Connect Db
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("Jwt"));

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSetting>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromSeconds(10)
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token dạng: Bearer {your_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();
//
using (var scope = app.Services.CreateScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<ProvinceWardSeedService>();
    await seed.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();