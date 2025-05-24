using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Multas
{
    public class CreateModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public CreateModel(BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BibliotecaRinconDelLibro.Models.Multa Multa { get; set; }

        public SelectList PrestamosList { get; set; }
        public SelectList TiposMultaList { get; set; }

        public void OnGet()
        {
            PrestamosList = new SelectList(_context.Prestamos, "IdPrestamo", "IdPrestamo");
            TiposMultaList = new SelectList(_context.TipoMulta, "IdTipomulta", "Descripcion");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Multas.Add(Multa);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
