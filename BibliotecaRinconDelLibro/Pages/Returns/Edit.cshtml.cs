using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Returns
{
    public class EditModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public EditModel(BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Devolucione Devolucion { get; set; }

        public List<SelectListItem> DescripcionEstado { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Buen estado" },
            new SelectListItem { Value = "2", Text = "Dañado" }
        };

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Devolucion = await _context.Devoluciones.FindAsync(id);

            if (Devolucion == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Devolucion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DevolucionExists(Devolucion.IdDevoluciones))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Index");
        }

        private bool DevolucionExists(int id)
        {
            return _context.Devoluciones.Any(e => e.IdDevoluciones == id);
        }
    }
}
