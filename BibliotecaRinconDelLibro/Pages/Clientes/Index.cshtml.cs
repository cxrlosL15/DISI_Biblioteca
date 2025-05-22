using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public IndexModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public IList<Cliente> Cliente { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public async Task OnGetAsync()
        {
            var query = _context.Clientes
                .Include(c => c.IdDireccionNavigation)
                .Where(c => c.EstadoCliente == 1) // Solo clientes activos
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(c =>
                    c.Nombre!.Contains(SearchString) ||
                    c.ApellidoP!.Contains(SearchString) ||
                    c.ApellidoM!.Contains(SearchString) ||
                    c.Correo!.Contains(SearchString));
            }

            Cliente = await query.ToListAsync();
        }
    }
}
