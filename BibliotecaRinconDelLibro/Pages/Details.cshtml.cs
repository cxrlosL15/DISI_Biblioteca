using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DetailsModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public Disponibilidad Disponibilidad { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*Prueba de cambios comentarios*/
            var disponibilidad = await _context.Disponibilidads.FirstOrDefaultAsync(m => m.IdDisponibilidad == id);
            if (disponibilidad == null)
            {
                return NotFound();
            }
            else
            {
                Disponibilidad = disponibilidad;
            }
            return Page();
        }
    }
}
