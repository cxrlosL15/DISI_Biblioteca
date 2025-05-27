using BibliotecaRinconDelLibro.Models; // Correcto
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Aseg�rate de tener este para List
using System.Linq;
using System.Threading.Tasks;

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
        // --- CORRECCI�N APLICADA AQU� ---
        public BibliotecaRinconDelLibro.Models.Multa Multa { get; set; } = new BibliotecaRinconDelLibro.Models.Multa();

        public SelectList? PrestamosList { get; set; }
        public SelectList? TiposMultaList { get; set; }

        // ELIMINASTE CORRECTAMENTE EL OnGet() S�NCRONO
        /* public void OnGet() 
        { ... } */

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListasDesplegablesAsync();
            // Es buena pr�ctica inicializar Multa aqu� tambi�n si no lo hiciste en la declaraci�n de la propiedad
            // y si necesitas que tenga valores por defecto antes de que se muestre el formulario.
            // Multa = new BibliotecaRinconDelLibro.Models.Multa(); // Ya est� inicializada en la declaraci�n.
            return Page();
        }

        private async Task CargarListasDesplegablesAsync()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.IdClientesNavigation)
                .Include(p => p.IdLibroNavigation)
                .Select(p => new {
                    p.IdPrestamo,
                    DisplayText = $"ID: {p.IdPrestamo} (Cliente: {(p.IdClientesNavigation != null ? p.IdClientesNavigation.Nombre : "")} {(p.IdClientesNavigation != null ? p.IdClientesNavigation.ApellidoP : "")}, Libro: {(p.IdLibroNavigation != null ? p.IdLibroNavigation.Titulo : "")})"
                })
                .ToListAsync();
            PrestamosList = new SelectList(prestamos, "IdPrestamo", "DisplayText", Multa?.IdPrestamo);

            // Aseg�rate que tu DbSet para TipoMulta se llame "TipoMultas" o "TipoMulta" en BibliotecaContext.cs
            // El error anterior mencionaba "TipoMultum", que es como EF Core a veces singulariza.
            // Si tu DbSet se llama TipoMultums, usa _context.TipoMultums
            var tipoMultas = await _context.TipoMulta // o _context.TipoMultas o _context.TipoMultums
                .OrderBy(t => t.Tipo)
                .Select(t => new {
                    Id = t.IdTipomulta,
                    Texto = $"{t.Tipo} (Monto: {(t.MontoBase ?? 0).ToString("C")})" // Asumiendo MontoBase en TipoMulta
                })
                .ToListAsync();
            TiposMultaList = new SelectList(tipoMultas, "Id", "Texto", Multa?.IdTipomulta);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Aseg�rate que tu DbSet se llame TipoMulta o TipoMultas
            // Si la entidad se llama TipoMultum, el DbSet podr�a ser _context.TipoMultums
            var tipoMultaSeleccionado = await _context.TipoMulta.FindAsync(Multa.IdTipomulta);

            if (tipoMultaSeleccionado != null)
            {
                if (tipoMultaSeleccionado.Tipo == "Libro no regresado")
                {
                    var prestamoConLibro = await _context.Prestamos
                        .Include(p => p.IdLibroNavigation)
                        .FirstOrDefaultAsync(p => p.IdPrestamo == Multa.IdPrestamo);

                    if (prestamoConLibro?.IdLibroNavigation != null)
                    {
                        // Asumiendo que Libro tiene ValorReposicion
                        Multa.Monto = prestamoConLibro.IdLibroNavigation.ValorReposicion;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudo encontrar el libro asociado al pr�stamo para calcular el monto.");
                        await CargarListasDesplegablesAsync();
                        return Page();
                    }
                }
                else
                {
                    // Aseg�rate que TipoMulta (o TipoMultum) tenga MontoBase o PrecioMulta
                    Multa.Monto = tipoMultaSeleccionado.MontoBase;
                }
            }
            else
            {
                ModelState.AddModelError("Multa.IdTipomulta", "El tipo de multa seleccionado no es v�lido.");
                await CargarListasDesplegablesAsync();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await CargarListasDesplegablesAsync();
                return Page();
            }

            _context.Multas.Add(Multa); // Aqu� Multa es this.Multa, ya es del tipo correcto
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}