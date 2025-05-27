using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Clientes
{
    public class DetailsModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DetailsModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public Cliente Cliente { get; set; } = default!;

        public double TotalMultasPendientesDelCliente { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }
            
            Cliente = await _context.Clientes
        .Include(c => c.IdDireccionNavigation) // <--- Esta línea es clave
        .Include(c => c.Prestamos)
            .ThenInclude(p => p.IdLibroNavigation)
        .Include(c => c.Prestamos)
            .ThenInclude(p => p.Multa)
                .ThenInclude(m => m.IdTipomultaNavigation)
        .FirstOrDefaultAsync(m => m.IdClientes == id) ?? throw new InvalidOperationException("Cliente no encontrado.");

           


            if (Cliente == null)
            {
                return NotFound();
            }
            
            else
            {
                Cliente = Cliente;
            }
            TotalMultasPendientesDelCliente = await _context.Multas
                .Where(m => m.IdPrestamoNavigation.IdClientes == id && m.Pagado != true)
                .SumAsync(m => m.Monto ?? 0);
            return Page();
        }
    }
}
