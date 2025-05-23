using Biblioteca_Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient; // Para SQL Server
using System.Threading.Tasks;

namespace BibliotecaRinconDelLibro.PasswordMigrator
{
    internal class Program
    {//Hola vlads
        // --- CONFIGURACIÓN ---
        // ¡¡¡REEMPLAZA CON TU CADENA DE CONEXIÓN REAL!!!
        // Usa la cadena de conexión con Autenticación de Windows si así es como te conectas.
        private const string ConnectionString = "Server=ELISE\\MSSQLSERVER02;Database=BibliotecaRinconDelLibro;Trusted_Connection=True;TrustServerCertificate=True;";
        // O si usas usuario y contraseña SQL:
        // private const string ConnectionString = "Server=tu_servidor_sql;Database=BibliotecaRinconDelLibro;User ID=tu_usuario;Password=tu_contraseña;TrustServerCertificate=True;";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando la migración de contraseñas...");
            Console.WriteLine("IMPORTANTE: Asegúrate de haber respaldado tu base de datos antes de continuar.");
            Console.Write("¿Estás seguro de que deseas continuar? (s/N): ");
            string confirmation = Console.ReadLine();

            if (!confirmation.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Migración cancelada por el usuario.");
                return;
            }

            List<UserToMigrate> usersToMigrate = new List<UserToMigrate>();

            try
            {
                // --- PASO 1: Obtener todos los usuarios con sus contraseñas (actualmente en texto plano) ---
                Console.WriteLine("Obteniendo usuarios de la base de datos...");
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    // Seleccionamos id_usuario y contrasena.
                    // Podrías añadir un WHERE clause aquí si solo quisieras migrar un subconjunto,
                    // o si tuvieras una columna como 'ContrasenaYaHasheada' para evitar reprocesar.
                    // Por ahora, asumimos que todas las contraseñas en la columna 'contrasena' son texto plano.
                    string selectSql = "SELECT id_usuario, contrasena, correo FROM Usuarios";
                    using (SqlCommand command = new SqlCommand(selectSql, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                usersToMigrate.Add(new UserToMigrate
                                {
                                    Id = Convert.ToInt32(reader["id_usuario"]),
                                    PlainTextPassword = reader["contrasena"].ToString(), // Asumimos que esto es texto plano
                                    Email = reader["correo"].ToString()
                                });
                            }
                        }
                    }
                }
                Console.WriteLine($"Se encontraron {usersToMigrate.Count} usuarios para procesar.");

                // --- PASO 2: Hashear cada contraseña y actualizar la base de datos ---
                int migratedCount = 0;
                foreach (var user in usersToMigrate)
                {
                    if (string.IsNullOrEmpty(user.PlainTextPassword))
                    {
                        Console.WriteLine($"Usuario ID {user.Id} ({user.Email}) no tiene contraseña o está vacía. Omitiendo.");
                        continue;
                    }

                    // Aquí es donde verificas si la contraseña YA PARECE UN HASH
                    // Un hash Base64 de SHA256 tiene 44 caracteres.
                    // Y no suele contener espacios ni muchos caracteres especiales que una contraseña plana podría tener.
                    // Esta es una heurística simple, ¡ajusta si es necesario!
                    if (user.PlainTextPassword.Length == 44 && IsBase64String(user.PlainTextPassword))
                    {
                        Console.WriteLine($"Usuario ID {user.Id} ({user.Email}) su contraseña ya parece estar hasheada. Omitiendo.");
                        continue;
                    }


                    string hashedPassword = PasswordHasher.HashPassword(user.PlainTextPassword);

                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        await connection.OpenAsync();
                        string updateSql = "UPDATE Usuarios SET contrasena = @HashedPassword WHERE id_usuario = @UserId";
                        using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                            updateCommand.Parameters.AddWithValue("@UserId", user.Id);
                            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine($"Contraseña para Usuario ID {user.Id} ({user.Email}) hasheada y actualizada correctamente.");
                                migratedCount++;
                            }
                            else
                            {
                                Console.WriteLine($"ADVERTENCIA: No se actualizó la contraseña para Usuario ID {user.Id} ({user.Email}). ¿Existe el usuario?");
                            }
                        }
                    }
                }
                Console.WriteLine($"\nMigración completada. {migratedCount} contraseñas fueron procesadas y actualizadas.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n¡ERROR DURANTE LA MIGRACIÓN!");
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                Console.WriteLine("\nPOR FAVOR, REVISA LOS MENSAJES DE ERROR. SI ALGO FALLÓ, CONSIDERA RESTAURAR DESDE TU RESPALDO.");
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir.");
            Console.ReadKey();
        }

        // Clase auxiliar para almacenar datos del usuario
        private class UserToMigrate
        {
            public int Id { get; set; }
            public string PlainTextPassword { get; set; }
            public string Email { get; set; } // Para un output más amigable
        }

        // Función auxiliar simple para verificar si una cadena podría ser Base64
        // Esto es una heurística y no una validación completa de Base64, pero útil aquí.
        private static bool IsBase64String(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length % 4 != 0 || s.Contains(" ") || s.Contains("\t") || s.Contains("\r") || s.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
 
