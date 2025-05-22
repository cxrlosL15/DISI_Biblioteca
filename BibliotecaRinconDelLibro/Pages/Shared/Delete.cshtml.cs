using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Shared
{
    public class DeleteModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DeleteModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EstadoLibro EstadoLibro { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadolibro = await _context.EstadoLibros.FirstOrDefaultAsync(m => m.IdEstadoLibro == id);

            if (estadolibro == null)
            {
                return NotFound();
            }
            else
            {
                EstadoLibro = estadolibro;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadolibro = await _context.EstadoLibros.FindAsync(id);
            if (estadolibro != null)
            {
                EstadoLibro = estadolibro;
                _context.EstadoLibros.Remove(EstadoLibro);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
