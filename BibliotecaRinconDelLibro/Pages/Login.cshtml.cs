using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient; // Para SQL Server
using Microsoft.Extensions.Configuration; // Para leer appsettings.json
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic; // Para List<Claim>
using System.Text.RegularExpressions; // Para validaci�n de emojis
using System.Threading.Tasks; // Para Task

namespace Biblioteca_Login.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El campo de correo electr�nico es obligatorio.")]
            [EmailAddress(ErrorMessage = "El formato del correo electr�nico no es v�lido. Debe ser como 'ejemplo@dominio.com'.")]
            [RegularExpression(@"^[^<>%$'""]*$", ErrorMessage = "El correo electr�nico contiene caracteres no permitidos.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "El campo de contrase�a es obligatorio.")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^[^<>%$'""]*$", ErrorMessage = "La contrase�a contiene caracteres no permitidos.")]
            public string Password { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            // Si ya est� autenticado y trata de ir a Login, redirigir (opcional)
            if (User.Identity.IsAuthenticated)
            {
                // Considera a d�nde redirigir, quiz�s a una p�gina de inicio o dashboard.
                // Response.Redirect(ReturnUrl); 
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (ContainsEmojis(Input.Email) || ContainsEmojis(Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Los campos no deben contener emojis.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("BibliotecaConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"
                    SELECT u.id_usuario, u.correo, u.contrasena, u.nombre, u.apellidoP, r.tipo AS TipoRol
                    FROM Usuarios u
                    INNER JOIN Rol r ON u.id_rol = r.id_rol
                    WHERE u.correo = @Correo";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Correo", Input.Email);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string storedHash = reader["contrasena"].ToString();
                                int userId = Convert.ToInt32(reader["id_usuario"]);
                                string nombreUsuario = reader["nombre"].ToString();
                                string apellidoPaterno = reader["apellidoP"].ToString();
                                string nombreCompleto = $"{nombreUsuario} {apellidoPaterno}".Trim();
                                string tipoRol = reader["TipoRol"].ToString();

                                if (PasswordHasher.VerifyPassword(Input.Password, storedHash))
                                {
                                    var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, reader["correo"].ToString()), // Usar el correo de la BD
                                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                                    new Claim(ClaimTypes.GivenName, nombreCompleto), // Nombre para mostrar
                                    new Claim(ClaimTypes.Role, tipoRol) // El rol como "Administrador" o "Bibliotecario"
                                };

                                    var claimsIdentity = new ClaimsIdentity(
                                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                    var authProperties = new AuthenticationProperties
                                    {
                                        IsPersistent = true, // Recordar al usuario (opcional)
                                                             // ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Duraci�n de la cookie (opcional)
                                    };

                                    await HttpContext.SignInAsync(
                                        CookieAuthenticationDefaults.AuthenticationScheme,
                                        new ClaimsPrincipal(claimsIdentity),
                                        authProperties);

                                    if (tipoRol.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                                    {
                                        return LocalRedirect(Url.Content("~/Admin/GestionUsuarios"));
                                    }
                                    // Redirigir a una p�gina principal o dashboard para otros roles
                                    return LocalRedirect(Url.Content("~/Index")); // Asumiendo que tienes una p�gina Index
                                }
                                else
                                {
                                    ErrorMessage = "Correo o contrase�a incorrectos.";
                                    return Page();
                                }
                            }
                            else
                            {
                                ErrorMessage = "Correo o contrase�a incorrectos. Verifique sus datos.";
                                return Page();
                            }
                        }
                    }
                }
            }
            catch (SqlException dbEx)
            {
                // TODO: Log dbEx.ToString() para un diagn�stico detallado
                ErrorMessage = "Error de conexi�n con la base de datos. Por favor, intente m�s tarde.";
                return Page();
            }
            catch (Exception ex)
            {
                // TODO: Log ex.ToString()
                ErrorMessage = "Ocurri� un error inesperado. Por favor, intente m�s tarde.";
                return Page();
            }
        }

        private bool ContainsEmojis(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            // Regex para detectar algunos rangos comunes de emojis y s�mbolos que podr�an no ser deseados.
            Regex rgx = new Regex(@"\p{Cs}|\p{So}|\p{Sk}|\p{Sc}|\p{Sm}");
            return rgx.IsMatch(input);
        }
    }
}
