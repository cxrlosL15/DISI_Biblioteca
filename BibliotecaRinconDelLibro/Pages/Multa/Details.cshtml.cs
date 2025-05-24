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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Multa = await _context.Multas
                .Include(m => m.IdPrestamoNavigation)
                .Include(m => m.IdTipomultaNavigation)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (Multa == null)
                return NotFound();

            return Page();
        }
    }
}
