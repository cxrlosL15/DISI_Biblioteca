using BibliotecaRinconDelLibro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Esto proteger�a todas las p�ginas en la carpeta Admin
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole");
    options.Conventions.AuthorizeFolder("/"); // Protege todas las p�ginas por defecto
    options.Conventions.AllowAnonymousToPage("/Login"); // Permite acceso an�nimo al Login
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
    // Puedes a�adir autorizaci�n a p�ginas espec�ficas si es necesario:
    // options.Conventions.AuthorizePage("/Admin/GestionUsuarios", "RequireAdminRole");
});
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection")));
// Configuraci�n de Autenticaci�n por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // P�gina de login
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";

    });

// Opcional: Configuraci�n de autorizaci�n para roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrador"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Esencial: habilita la autenticaci�n
app.UseAuthorization();

app.MapRazorPages();

app.Run();
