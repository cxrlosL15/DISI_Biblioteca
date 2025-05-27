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
    public class DetailsModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DetailsModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public BibliotecaRinconDelLibro.Models.Prestamo Prestamo { get; set; } = default!;
        public EncabezadoTicket? EncabezadoTicket { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EncabezadoTicket = await _context.EncabezadoTickets
                .Include(e => e.IdDireccionNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync();



            var prestamo = await _context.Prestamos
                 .Include(p => p.IdClientesNavigation)
                 .Include(p => p.IdLibroNavigation)
                 .Include(p => p.IdEstadoLibroNavigation)
                 .Include(p => p.IdUsuarioNavigation)
                 .FirstOrDefaultAsync(m => m.IdPrestamo == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            Prestamo = prestamo;
            /*
            TotalMultasPendientes = await _context.Multas
               .Where(m => m.IdPrestamo == id && m.Pagado != true) // Filtra por el ID del préstamo actual y que no esté pagada
               .SumAsync(m => m.Monto ?? 0); // Suma los montos (trata null como 0)

            // Opcional: Calcular la suma de multas pagadas
            TotalMultasPagadas = await _context.Multas
                .Where(m => m.IdPrestamo == id && m.Pagado == true)
                .SumAsync(m => m.Monto ?? 0);
            */
            return Page();
        }

    }
}
