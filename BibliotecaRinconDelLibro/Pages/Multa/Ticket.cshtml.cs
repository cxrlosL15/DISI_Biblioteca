using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Multa
{
    public class TicketModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public TicketModel(BibliotecaContext context)
        {
            _context = context;
        }

        public BibliotecaRinconDelLibro.Models.Multa Multa { get; set; } = default!;

        public EncabezadoTicket? EncabezadoTicket { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            EncabezadoTicket = await _context.EncabezadoTickets
               .Include(e => e.IdDireccionNavigation)
               .Include(e => e.IdUsuarioNavigation)
               .FirstOrDefaultAsync();

             Multa = await _context.Multas
                .Include(m => m.IdPrestamoNavigation).ThenInclude(p => p.IdClientesNavigation)
                .Include(m => m.IdTipomultaNavigation)
                .Include(m => m.TicketMulta)
                    .ThenInclude(tm => tm.IdEncabezadoTicketNavigation)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (Multa == null)
            {
                return NotFound();
            }

            EncabezadoTicket = Multa.TicketMulta.FirstOrDefault()?.IdEncabezadoTicketNavigation;

            return Page();
        }
    }
}