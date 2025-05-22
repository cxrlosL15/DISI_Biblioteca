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
    public class DeleteModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DeleteModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Disponibilidad Disponibilidad { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disponibilidad = await _context.Disponibilidads.FindAsync(id);
            if (disponibilidad != null)
            {
                Disponibilidad = disponibilidad;
                _context.Disponibilidads.Remove(Disponibilidad);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
