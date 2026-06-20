using Application.Commands;
using Application.ExceptionLogging;
using Application.Queries;
using ASPLAB2.API.JWT;
using ASPLAB2.API.Middleware;
using Backend;
using Data.Access;
using Implementation.Commands;
using Implementation.ExceptionLogging;
using Implementation.Queries.Auth;
using Implementation.Queries.Posts;
using Implementation.Queries.Users;
using Implementation.Validations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

AppSettings appSettings = new AppSettings();

// Sta radi bind metoda?
// - Mapira podatke iz konfiguracije u appSettings objekat

// Zasto se uopste koristi bind-ovnje zasto ne bih citao vrednosti sa builder.Configuration["AppSettings:ApplicationName"];?
// - Zbog DI

builder.Configuration.Bind(appSettings);

// Koji lifetime-ovi postoje za DI container?
// Lifetimeovi postavljaju pitanje "Koliko dugo zivi instanca objekta iz DI container-a?"
// - Scoped -> pravi se 1 instanca objekta na nivou request-a
// - Singleton -> pravi se 1 instanca na nivou cele aplikacije
// - Transient -> pravi se 1 isntanca svaki put kada se zatrazi iz DI container-a


// Treba da registrujem IRegisterUserCommand i RegisterUserValidation
// Treba da DI container-u kazem "Kada se trazi IRegisterUserCommand prosledi klasu EfRegisterUserCommand" i RegiseterUserValidation da registrujem
// Kako to?



builder.Services.AddSingleton(appSettings);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{

    options.UseSqlServer(appSettings.ConnectionStringSQL).UseLazyLoadingProxies();

}
);

// ISPOD IDU REGISTRACIJE U DI CONTAINER
// 

// Koju klasu registrujem u DI Container?
// - IRegisterUserCommand

// Koji lifetime ce klasa imati u DI container-u?
// - JA bih isao sa Scoped -> 1 instanca na nivou request-a ili transient svaki put nova, ma transient
// - Mozda bih cak isao i Singleton jer mi treba interfejs, ali prosledice istu klasu koja je vezana za taj interfejs i onda ako se ta klasa koja je vezana za interfejs prosledjuje u vise metoda onda ce sve one mutirati istu klasu, zato ne sme ni na nivou request-a (Scoped) vec mora biti Transient

builder.Services.AddTransient<IRegisterUserCommand, EfRegisterUserCommand>();
builder.Services.AddTransient<RegisterUserValidation>();
builder.Services.AddTransient<ILoginQuery, EfLoginQuery>();
builder.Services.AddTransient<IPostsQuery, EfPostsQuery>();
builder.Services.AddTransient<IPostQuery, EfPostQuery>();
builder.Services.AddTransient<IUsersQuery, EfUsersQuery>();
builder.Services.AddTransient<IUserQuery, EfUserQuery>();
builder.Services.AddTransient<IExceptionLogger, ConsoleLogging>();
builder.Services.AddTransient<JwtHandler>();







builder.Services.AddAuthentication(options =>
{

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{

    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = appSettings.JwtSettings.Issuer,
        ValidateIssuer = true,
        ValidAudience = "Any",
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtSettings.SecretKey)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
