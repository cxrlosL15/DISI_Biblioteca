using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibliotecaRinconDelLibro.Models;

public class MultasModel : PageModel
{
    private readonly BibliotecaContext _context;

    public MultasModel(BibliotecaContext context)
    {
        _context = context;
    }

    public List<MultaViewModel> Multas { get; set; }

    public async Task OnGetAsync()
    {
        Multas = await _context.Multas
            .Include(m => m.Prestamo)
                .ThenInclude(p => p.Cliente)
            .Include(m => m.Prestamo)
                .ThenInclude(p => p.Libro)
            .Include(m => m.TipoMulta)
            .Select(m => new MultaViewModel
            {
                IdMulta = m.IdMulta,
                Cliente = m.Prestamo.Cliente.Nombre + " " + m.Prestamo.Cliente.ApellidoP,
                Libro = m.Prestamo.Libro.Titulo,
                TipoMulta = m.TipoMulta.Tipo,
                Monto = m.Monto,
                Descripcion = m.Descripcion,
                Pagado = m.Pagado
            })
            .ToListAsync();
    }
}
