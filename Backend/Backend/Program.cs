using Backend;
using Data.Access;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

AppSettings config = new AppSettings();

// Sta radi bind metoda?
// - Mapira podatke iz konfiguracije u appSettings objekat

// Zasto se uopste koristi bind-ovnje zasto ne bih citao vrednosti sa builder.Configuration["AppSettings:ApplicationName"];?
// - Zbog DI

builder.Configuration.Bind(config);

// Koji lifetime-ovi postoje za DI container?
// Lifetimeovi postavljaju pitanje "Koliko dugo zivi instanca objekta iz DI container-a?"
// - Scoped -> pravi se 1 instanca objekta na nivou request-a
// - Singleton -> pravi se 1 instanca na nivou cele aplikacije
// - Transient -> pravi se 1 isntanca svaki put kada se zatrazi iz DI container-a



builder.Services.AddSingleton(config);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{

    options.UseSqlServer(config.ConnectionStringSQL).UseLazyLoadingProxies();

}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
