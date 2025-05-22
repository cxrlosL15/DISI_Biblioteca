using BibliotecaRinconDelLibro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Esto protegería todas las páginas en la carpeta Admin
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole");
    // Puedes añadir autorización a páginas específicas si es necesario:
    // options.Conventions.AuthorizePage("/Admin/GestionUsuarios", "RequireAdminRole");
});
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection")));
// Configuración de Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Página a la que redirige si no está autenticado
        options.LogoutPath = "/Logout"; // Página para cerrar sesión (crearemos una simple)
        options.AccessDeniedPath = "/AccessDenied"; // Página si no tiene permisos
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Tiempo de expiración de la cookie
        options.SlidingExpiration = true; // Renueva la cookie si hay actividad

    });

// Opcional: Configuración de autorización para roles
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

app.UseAuthentication(); // Esencial: habilita la autenticación
app.UseAuthorization();

app.MapRazorPages();

app.Run();
