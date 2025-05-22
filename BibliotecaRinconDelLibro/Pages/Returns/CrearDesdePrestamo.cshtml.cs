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
                Mensaje = "Por favor, ingresa un ID de préstamo válido.";
                return Page();
            }

            var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.IdPrestamo == IdPrestamo);
            if (prestamo == null)
            {
                Mensaje = $"No se encontró el préstamo con ID {IdPrestamo}.";
                return Page();
            }

            bool yaDevuelto = await _context.Devoluciones.AnyAsync(d => d.IdPrestamo == IdPrestamo);
            if (yaDevuelto)
            {
                Mensaje = "Este préstamo ya tiene una devolución registrada.";
                return Page();
            }

            var devolucion = new Devolucione
            {
                IdPrestamo = IdPrestamo,
                FechaDevolucion = DateTime.Now
            };

            _context.Devoluciones.Add(devolucion);
            await _context.SaveChangesAsync();

            Mensaje = $"Devolución registrada correctamente para préstamo ID {IdPrestamo}.";

            return RedirectToPage("./Index");
        }
    }
}
