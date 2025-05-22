using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Libros
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public IndexModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public IList<Libro> Libro { get;set; } = default!;

        public async Task OnGetAsync()
        {
            // Base de la consulta: incluye relaciones necesarias
            var query = _context.Libros
                .Include(l => l.IdCategoriasNavigation)
                .Include(l => l.IdDisponibilidadNavigation)
                .Include(l => l.IdEditorialNavigation)
                .Include(l => l.IdGenerosNavigation)
                .Include(l => l.LibroAutores)
                    .ThenInclude(la => la.Autor)
                .Where(l => !l.BorradoLogico) // <- Solo no borrados
                .AsQueryable();

            // Si hay texto de búsqueda, filtramos por título o por nombre de autor
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(l =>
                    l.Titulo!.Contains(SearchString) ||
                    l.LibroAutores.Any(la =>
                        (la.Autor.Nombres + " " + la.Autor.ApellidoP + " " + la.Autor.ApellidoM)
                        .Contains(SearchString)
                    )
                );
            }
            Libro = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                libro.BorradoLogico = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
