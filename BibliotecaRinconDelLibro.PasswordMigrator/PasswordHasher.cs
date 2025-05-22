namespace Biblioteca_Login
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                // Se recomienda usar un "salt" para mayor seguridad, aquí un ejemplo simple
                // En una aplicación real, el salt debería ser único por usuario y almacenado.
                // string salt = "tuSaltSecretoUnico"; // ¡NO USAR UN SALT FIJO EN PRODUCCIÓN!
                // byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password); // Simplificado sin salt visible para este ejemplo
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hashedPassword) == 0;
        }
    }
}
