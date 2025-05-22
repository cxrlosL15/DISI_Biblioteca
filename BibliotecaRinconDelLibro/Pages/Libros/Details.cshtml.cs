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
    public class DetailsModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DetailsModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public Libro Libro { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.IdCategoriasNavigation)
                .Include(l => l.IdGenerosNavigation)
                .Include(l => l.IdEditorialNavigation)
                .Include(l => l.IdUbicacionNavigation)
                .Include(l => l.IdDisponibilidadNavigation)
                .Include(l => l.IdImgLibrosNavigation)
                .Include(l => l.LibroAutores)!.ThenInclude(la => la.Autor)
                .FirstOrDefaultAsync(m => m.IdLibro == id);

            if (libro == null)
            {
                return NotFound();
            }
            else
            {
                Libro = libro;
            }
            return Page();
        }
    }
}
