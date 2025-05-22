using BibliotecaRinconDelLibro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Esto proteger�a todas las p�ginas en la carpeta Admin
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole");
    // Puedes a�adir autorizaci�n a p�ginas espec�ficas si es necesario:
    // options.Conventions.AuthorizePage("/Admin/GestionUsuarios", "RequireAdminRole");
});
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection")));
// Configuraci�n de Autenticaci�n por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // P�gina a la que redirige si no est� autenticado
        options.LogoutPath = "/Logout"; // P�gina para cerrar sesi�n (crearemos una simple)
        options.AccessDeniedPath = "/AccessDenied"; // P�gina si no tiene permisos
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Tiempo de expiraci�n de la cookie
        options.SlidingExpiration = true; // Renueva la cookie si hay actividad

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
