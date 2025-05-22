using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient; // Para SQL Server
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Biblioteca_Login.Pages.Admin
{
    [Authorize(Roles = "Administrador")] // Asegura que solo usuarios con el rol "Administrador" puedan acceder.
    public class GestionUsuariosModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public GestionUsuariosModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<UsuarioViewModel> Usuarios { get; set; } = new List<UsuarioViewModel>();

        [BindProperty]
        public NuevoUsuarioInputModel NuevoUsuario { get; set; } = new NuevoUsuarioInputModel();

        public List<SelectListItem> RolesDisponibles { get; set; } = new List<SelectListItem>();

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class UsuarioViewModel
        {
            public int UsuarioID { get; set; }
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public string TipoRol { get; set; }
        }

        public class NuevoUsuarioInputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(255)]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
            [StringLength(255)]
            public string ApellidoP { get; set; }

            [StringLength(255)]
            public string ApellidoM { get; set; } // Opcional

            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
            [StringLength(255)]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.,#\-_])[A-Za-z\d@$!%*?&.,#\-_]{8,}$",
             ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un caracter especial (@$!%*?&.,#-_).")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un rol.")]
            [Display(Name = "Rol del Usuario")]
            public int IdRol { get; set; }
        }

        private async Task CargarRolesDisponiblesAsync()
        {
            RolesDisponibles.Clear();
            string connectionString = _configuration.GetConnectionString("BibliotecaConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT id_rol, tipo FROM Rol ORDER BY tipo";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RolesDisponibles.Add(new SelectListItem
                            {
                                Value = reader["id_rol"].ToString(),
                                Text = reader["tipo"].ToString()
                            });
                        }
                    }
                }
            }
        }

        private async Task CargarUsuariosAsync()
        {
            Usuarios.Clear();
            string connectionString = _configuration.GetConnectionString("BibliotecaConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = @"
                SELECT u.id_usuario, u.nombre, u.apellidoP, u.apellidoM, u.correo, r.tipo AS TipoRol
                FROM Usuarios u
                INNER JOIN Rol r ON u.id_rol = r.id_rol
                ORDER BY u.nombre, u.apellidoP";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Usuarios.Add(new UsuarioViewModel
                            {
                                UsuarioID = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                NombreCompleto = $"{reader["nombre"]} {reader["apellidoP"]} {reader["apellidoM"]}".Trim(),
                                Email = reader["correo"].ToString(),
                                TipoRol = reader["TipoRol"].ToString()
                            });
                        }
                    }
                }
            }
        }

        public async Task OnGetAsync()
        {
            await CargarUsuariosAsync();
            await CargarRolesDisponiblesAsync();
        }

        public async Task<IActionResult> OnPostCrearUsuarioAsync()
        {
            await CargarRolesDisponiblesAsync(); // Necesario si hay error de modelo para repoblar el dropdown

            // Validación de Emojis para los campos de texto
            if (ContainsEmojis(NuevoUsuario.Nombre) || ContainsEmojis(NuevoUsuario.ApellidoP) || ContainsEmojis(NuevoUsuario.ApellidoM) || ContainsEmojis(NuevoUsuario.Email) || ContainsEmojis(NuevoUsuario.Password))
            {
                ModelState.AddModelError(string.Empty, "Los campos no deben contener emojis u otros símbolos especiales no permitidos.");
            }


            if (!ModelState.IsValid)
            {
                await CargarUsuariosAsync(); // Recargar lista de usuarios
                return Page();
            }

            if (await EmailExisteAsync(NuevoUsuario.Email))
            {
                ModelState.AddModelError("NuevoUsuario.Email", "Este correo electrónico ya está registrado.");
                await CargarUsuariosAsync();
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"
                    INSERT INTO Usuarios (nombre, apellidoP, apellidoM, correo, contrasena, id_rol)
                    VALUES (@Nombre, @ApellidoP, @ApellidoM, @Correo, @PasswordHash, @IdRol)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", NuevoUsuario.Nombre);
                        command.Parameters.AddWithValue("@ApellidoP", NuevoUsuario.ApellidoP);
                        command.Parameters.AddWithValue("@ApellidoM", (object)NuevoUsuario.ApellidoM ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Correo", NuevoUsuario.Email);
                        command.Parameters.AddWithValue("@PasswordHash", PasswordHasher.HashPassword(NuevoUsuario.Password));
                        command.Parameters.AddWithValue("@IdRol", NuevoUsuario.IdRol);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            SuccessMessage = $"Usuario '{NuevoUsuario.Email}' creado exitosamente.";
                        }
                        else
                        {
                            ErrorMessage = "No se pudo crear el usuario. Verifique los datos.";
                        }
                    }
                }
            }
            catch (SqlException dbEx)
            {
                ErrorMessage = $"Error de base de datos: {dbEx.Message}.";
                // TODO: Log dbEx.ToString()
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error inesperado: {ex.Message}.";
                // TODO: Log ex.ToString()
            }

            // Limpiar el modelo para el próximo intento o recarga.
            //ModelState.Clear(); // Esto limpia los errores de validación también, lo cual puede no ser deseado si hubo error.
            NuevoUsuario = new NuevoUsuarioInputModel(); // Reinicia el objeto para el formulario.

            // Volver a cargar los usuarios para mostrar la lista actualizada
            await CargarUsuariosAsync();
            // Los mensajes de TempData se mostrarán después de la recarga de la página.
            return Page(); // Vuelve a mostrar la página, los TempData se encargarán de los mensajes.
                           // Si prefieres un RedirectToAction (PRG pattern): return RedirectToPage();
        }

        private async Task<bool> EmailExisteAsync(string email)
        {
            string connectionString = _configuration.GetConnectionString("BibliotecaConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT COUNT(1) FROM Usuarios WHERE correo = @Correo";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Correo", email);
                    int count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }
        private bool ContainsEmojis(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            Regex rgx = new Regex(@"\p{Cs}|\p{So}|\p{Sk}|\p{Sc}|\p{Sm}");
            return rgx.IsMatch(input);
        }
    }
}
