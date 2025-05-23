using System;
using System.Threading.Tasks;
using BibliotecaRinconDelLibro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Pages.Returns
{
    public class CrearDesdePrestamoModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public CrearDesdePrestamoModel(BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int IdPrestamo { get; set; }

        public string? Mensaje { get; set; }

        public void OnGet()
        {
            // Por si quieres pasar un IdPrestamo por query string en el futuro
        }

        public async Task<IActionResult> OnPostAsync()
        {
             if (IdPrestamo <= 0)
            {
                TempData["Mensaje"] = "Por favor, ingresa un ID de prestamo valido.";
                return Page();
            }

            var prestamo = await _context.Prestamos
                .Include(p => p.IdClientesNavigation)
                .FirstOrDefaultAsync(p => p.IdPrestamo == IdPrestamo);

            if (prestamo == null)
            {
                TempData["Mensaje"] = $"No se encontro el prestamo con ID {IdPrestamo}.";
                return Page();
            }

            bool yaDevuelto = await _context.Devoluciones.AnyAsync(d => d.IdPrestamo == IdPrestamo);
            if (yaDevuelto)
            {
                TempData["Mensaje"] = "Este prestamo ya tiene una devolucion registrada.";
                return Page();
            }

            var devolucion = new Devolucione
            {
                IdPrestamo = IdPrestamo,
                FechaDevolucion = DateTime.Now
            };

            _context.Devoluciones.Add(devolucion);

            // Cambia el estado del préstamo a "Devuelto" 
            prestamo.IdEstadoPrestamo = "Devuelto";

            await _context.SaveChangesAsync();

            // Verifica si se pasó la fecha de devolución
            if (DateTime.Now.Date > prestamo.FechaDevolucion.Date)
            {
                TempData["Mensaje"] = "Devolución registrada. El préstamo se devolvió tarde. Se debe generar una multa.";
            }
            else
            {
                TempData["Mensaje"] = $"Devolución registrada correctamente para préstamo ID {IdPrestamo}.";
            }

            return RedirectToPage("./CrearDesdePrestamo"); // o "../Index" si prefieres volver al listado
        }
    }
}
