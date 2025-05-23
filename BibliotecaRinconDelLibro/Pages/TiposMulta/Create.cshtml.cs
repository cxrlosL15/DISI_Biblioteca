using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.TipoMulta
{
    public class CreateModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public CreateModel(BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TipoMultum TipoMulta { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.TipoMulta.Add(TipoMulta);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
