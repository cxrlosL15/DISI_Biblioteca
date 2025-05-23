using BibliotecaRinconDelLibro.Models;          // ← 1) agrega tu namespace de entidades
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca_Login.Pages                // ← 2) deja tu namespace tal cual
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BibliotecaContext _context;   // 3) inyectamos el DbContext

        public IndexModel(ILogger<IndexModel> logger, BibliotecaContext context)
        {
            _logger = logger;
            _context = context;
        }

        // 4) Colección que la vista recorrerá
        public IList<Libro> Libros { get; private set; } = new List<Libro>();

        // 5) Carga de datos
        public async Task OnGetAsync()
        {
            Libros = await _context.Libros
                .Include(l => l.IdDisponibilidadNavigation)
                .Include(l => l.IdGenerosNavigation)
                .Include(l => l.IdCategoriasNavigation)
                .Include(l => l.IdImgLibrosNavigation)
                .ToListAsync();
        }
    }
}