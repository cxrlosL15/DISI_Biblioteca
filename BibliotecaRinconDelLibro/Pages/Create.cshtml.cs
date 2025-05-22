using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages
{
    public class CreateModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public CreateModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["IdPrestamo"] = new SelectList(_context.Prestamos, "IdPrestamo", "IdPrestamo");
            return Page();
        }

        [BindProperty]
        public Disponibilidad Disponibilidad { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Disponibilidads.Add(Disponibilidad);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
