using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Multa
{
    public class DetailsModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public DetailsModel(BibliotecaContext context)
        {
            _context = context;
        }

        public BibliotecaRinconDelLibro.Models.Multa Multa { get; set; }
       
        public double TotalMultasPendientes { get; set; } // Para mostrar la suma en la vista
        public double TotalMultasPagadas { get; set; }   // Opcional: para mostrar lo pagado

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Multa = await _context.Multas
                .Include(m => m.IdPrestamoNavigation).ThenInclude(p => p.IdClientesNavigation)
                .Include(m => m.IdTipomultaNavigation)
                .FirstOrDefaultAsync(m => m.IdMulta == id);
                

            if (Multa == null)
                return NotFound();

            return Page();
        }
    }
}
