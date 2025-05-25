using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models = BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Multas
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public IndexModel(BibliotecaContext context)
        {
            _context = context;
        }

        public List<Models.Multa> MultaList { get; set; } = new List<Models.Multa>();

        public async Task OnGetAsync()
        {
            MultaList = await _context.Multas
                .Include(m => m.IdPrestamoNavigation)
                .Include(m => m.IdTipomultaNavigation)

                .ToListAsync();
        }
    }
}
