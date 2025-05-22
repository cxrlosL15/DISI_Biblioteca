using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;  

namespace BibliotecaRinconDelLibro.Pages.Prestamo
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public IndexModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public IList<BibliotecaRinconDelLibro.Models.Prestamo> Prestamo { get; set; } = default!;


        public async Task OnGetAsync()
        {
            Prestamo = await _context.Prestamos
                .Include(p => p.IdClientesNavigation)
                .Include(p => p.IdEstadoLibroNavigation)
                .Include(p => p.IdLibroNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .ToListAsync();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Buscar el préstamo e incluir el libro
            var prestamo = await _context.Prestamos
                .Include(p => p.IdLibroNavigation) // Necesario para obtener el IdLibro
                .FirstOrDefaultAsync(p => p.IdPrestamo == id);

            if (prestamo != null)
            {
                //  Eliminar devoluciones relacionadas
                var devoluciones = await _context.Devoluciones
                    .Where(d => d.IdPrestamo == id)
                    .ToListAsync();

                _context.Devoluciones.RemoveRange(devoluciones);

                //  Buscar disponibilidad del libro y actualizar
                var disponibilidad = await _context.Disponibilidads
                    .FirstOrDefaultAsync(d => d.IdLibro == prestamo.IdLibro);

                if (disponibilidad != null)
                {
                    disponibilidad.TotalDespuesPrestamos = (disponibilidad.TotalDespuesPrestamos ?? 0) + 1;
                    disponibilidad.CopiasPrestadas = (disponibilidad.CopiasPrestadas ?? 1) - 1;

                    // Validación para evitar negativos
                    if (disponibilidad.CopiasPrestadas < 0)
                        disponibilidad.CopiasPrestadas = 0;
                }

                // Eliminar el préstamo
                _context.Prestamos.Remove(prestamo);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


    }
}
