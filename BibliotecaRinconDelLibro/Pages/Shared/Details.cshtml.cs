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
    public class DetailsModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DetailsModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

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
    }
}
