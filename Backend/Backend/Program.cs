using Application;
using Application.Commands;
using Application.Commands.PostApplications;
using Application.Commands.Posts;
using Application.Commands.Users;
using Application.Email;
using Application.ExceptionLogging;
using Application.Queries;
using Application.Queries.PostApplications;
using Application.Queries.Posts;
using ASPLAB2.API.JWT;
using ASPLAB2.API.Middleware;
using Backend;
using Backend.JWT;
using Data.Access;
using Implementation;
using Implementation.Commands;
using Implementation.Commands.PostApplications;
using Implementation.Commands.Posts;
using Implementation.Commands.Users;
using Implementation.Emails;
using Implementation.ExceptionLogging;
using Implementation.Queries.Auth;
using Implementation.Queries.Posts;
using Implementation.Queries.PostsApplication;
using Implementation.Queries.Users;
using Implementation.UseCases.Commands;
using Implementation.Validations;
using Implementation.Validators.Posts;
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
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IRegisterUserCommand, EfRegisterUserCommand>();
builder.Services.AddTransient<RegisterUserValidation>();
builder.Services.AddTransient<ILoginQuery, EfLoginQuery>();
builder.Services.AddTransient<IPostsQuery, EfPostsQuery>();
builder.Services.AddTransient<IPostQuery, EfPostQuery>();
builder.Services.AddTransient<ICreatePostCommand, EfCreatePostCommand>();
builder.Services.AddTransient<CreatePostValidation>();
builder.Services.AddTransient<IDeletePostCommand, EfDeletePostCommand>();
builder.Services.AddTransient<IUpdatePostCommand, EfUpdatePostCommand>();

builder.Services.AddTransient<IPostsApplicationQuery, PostsApplicationQuery>();
builder.Services.AddTransient<IPostApplicationQuery, EfPostApplicationQuery>();
builder.Services.AddTransient<IUpdatePostApplicationCommand, EfUpdatePostApplicationCommand>();
builder.Services.AddTransient<IDeletePostApplicationCommand, EfDeletePostApplicationCommand>();
// treba da dodam interface za upload fajla loklano
// Da li cu koristiti Transient, Singleton ili Scoped?
// - Singleton pravi instancu objekta za ceo tok rada aplikacije
// - Transient pravi novu isntancu svaki put kada se zatrazi iz DI container-a
// - Scoped pravi novu instancu svaki put kada stigne novi requesjt, tako da ce instanca vaziti za rokt trajanja request-a

// Da li za x klasu koristiti Singleton?
// - Genericko pitanje koje se postavlja je "Da li klasa x sadrzi polja(fields) i ako sadrzi da li mi pravi problem ako bi se ona menjala sa svakim novim request-om?

builder.Services.AddTransient<IUploadPostFileToCommand, EfUploadPostFileToLocal>();
builder.Services.AddTransient<IUsersQuery, EfUsersQuery>();
builder.Services.AddTransient<IUserQuery, EfUserQuery>();
builder.Services.AddTransient<IDeleteUserCommand, EfDeleteUserCommand>();
builder.Services.AddTransient<IGetUserPostsQuery, EfGetUserPosts>();
builder.Services.AddTransient<IExceptionLogger, ConsoleLogging>();
builder.Services.AddTransient<IActivateAccountCommand, EfActivateAccountCommand>();
builder.Services.AddTransient<EmailTemplateComposer>();
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>(x =>
{
    return new SmtpEmailSender(appSettings.EmailSettings.FromEmail, appSettings.EmailSettings.AppPassword);
});
builder.Services.AddTransient<JwtHandler>();
builder.Services.AddScoped<UseCaseHandler>();


builder.Services.AddScoped<Backend.Authorization.AdminAccessFilter>();
builder.Services.AddScoped<IApplicationUser>(container =>
{
    var principal = container.GetRequiredService<IHttpContextAccessor>().HttpContext?.User;
    if (principal?.Identity?.IsAuthenticated != true ||
        !long.TryParse(principal.FindFirst("Id")?.Value, out var id))
        return new UnauthorizedUser();

    var context = container.GetRequiredService<ApplicationDbContext>();
    var user = context.Users.Include(x => x.Role).ThenInclude(x => x.RoleUseCases)
        .ThenInclude(x => x.UseCases).SingleOrDefault(x => x.Id == id);
    if (user == null || user.ActivatedAt == null) return new UnauthorizedUser();

    return new JwtUser
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        AllowedUseCases = user.Role.RoleUseCases
            .Where(x => x.DeletedAt == null && x.UseCases.DeletedAt == null)
            .Select(x => x.UseCases.UseCaseId)
            .Union(new UnauthorizedUser().AllowedUseCases).ToList()
    };
});
builder.Services.AddAuthentication(options =>
{

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{

    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.Events = new JwtBearerEvents
    {
        OnTokenValidated = async tokenContext =>
        {
            if (!long.TryParse(tokenContext.Principal?.FindFirst("Id")?.Value, out var id))
            {
                tokenContext.Fail("Invalid user identifier.");
                return;
            }
            var context = tokenContext.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            if (!await context.Users.AnyAsync(x => x.Id == id && x.ActivatedAt != null))
                tokenContext.Fail("This account is no longer active.");
        }
    };
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
app.UseStaticFiles(); // usluzuje fajlove iz wwwroot foldera
app.MapControllers();

app.Run();
