using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Returns
{
    public class DeleteModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;
        public DeleteModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Devolucione Devolucion { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Devolucion = await _context.Devoluciones.FindAsync(id);

            if (Devolucion == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var devolucion = await _context.Devoluciones.FindAsync(id);

            if (devolucion == null)
            {
                return NotFound();
            }

            _context.Devoluciones.Remove(devolucion);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
