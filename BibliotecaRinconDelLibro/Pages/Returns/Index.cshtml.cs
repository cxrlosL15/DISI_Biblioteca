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
    public class IndexModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public IndexModel(BibliotecaContext context)
        {
            _context = context;
        }

        public IList<Devolucione> DevolucionList { get; set; } 

        public async Task OnGetAsync()
        {
            DevolucionList = await _context.Devoluciones
                 .Include(d => d.IdPrestamoNavigation)
                 .ThenInclude(p => p.IdClientesNavigation)
                 .Include(d => d.IdEstadoRegresoNavigation)// <-- Esta es la propiedad correcta
                 .ToListAsync();
        }
    }
}
