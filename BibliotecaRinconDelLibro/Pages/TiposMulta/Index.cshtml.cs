using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.TipoMulta
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public IndexModel(BibliotecaContext context)
        {
            _context = context;
        }

        public List<TipoMultum> TipoMultaList { get; set; } = new();

        public async Task OnGetAsync()
        {
            TipoMultaList = await _context.TipoMulta.ToListAsync();
        }
    }
}
