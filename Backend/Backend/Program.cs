using Backend;
using Data.Access;
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



builder.Services.AddSingleton(appSettings);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{

    options.UseSqlServer(appSettings.ConnectionStringSQL).UseLazyLoadingProxies();

}
);


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
