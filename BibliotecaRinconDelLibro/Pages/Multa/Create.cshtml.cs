using BibliotecaRinconDelLibro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            TiposMultaList = new SelectList(_context.TipoMulta, "IdTipomulta", "Descripcion","Precio_Multa");
        }
        private async Task CargarListasDesplegablesAsync()
        {
            // Cargar pr�stamos (quiz�s quieras mostrar m�s info que solo el ID)
            // Podr�as necesitar filtrar pr�stamos que puedan tener multas, o pr�stamos activos, etc.
            // Este es un ejemplo general:
            var prestamos = await _context.Prestamos
                .Select(p => new {
                    p.IdPrestamo,
                    DisplayText = $"Pr�stamo ID: {p.IdPrestamo} - Cliente: {p.IdClientesNavigation.Nombre} {p.IdClientesNavigation.ApellidoP} - Libro: {p.IdLibroNavigation.Titulo}"
                })
                .ToListAsync();
            PrestamosList = new SelectList(prestamos, "IdPrestamo", "DisplayText", Multa?.IdPrestamo);

            var tipoMultas = await _context.TipoMulta.OrderBy(t => t.Tipo).ToListAsync();
            TiposMultaList = new SelectList(tipoMultas, "IdTipoMulta", "Tipo", Multa?.IdTipomulta);

            // Carga cualquier otra lista que necesites para tus dropdowns
            // Ejemplo: ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreCompleto");
        }
        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListasDesplegablesAsync();
            // Inicializa el objeto Multa si es necesario para valores por defecto
            
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var tipoMultaSeleccionado = await _context.TipoMulta.FindAsync(Multa.IdTipomulta); // Asumiendo que Multa.IdTipomulta es el ID del tipo seleccionado

            if (tipoMultaSeleccionado != null)
            {
                // Excepci�n: si es por "Libro no regresado", el monto es el precio de reposici�n del libro
                if (tipoMultaSeleccionado.Tipo == "Libro no regresado") // O como hayas llamado a este tipo espec�fico
                {
                    var prestamoConLibro = await _context.Prestamos
                        .Include(p => p.IdLibroNavigation) // Aseg�rate de tener esta navegaci�n en Prestamo.cs
                        .FirstOrDefaultAsync(p => p.IdPrestamo == Multa.IdPrestamo);

                    if (prestamoConLibro?.IdLibroNavigation != null)
                    {
                        Multa.Monto = prestamoConLibro.IdLibroNavigation.ValorReposicion; // Asumiendo que Libro tiene ValorReposicion
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudo encontrar el libro asociado al pr�stamo para calcular el monto.");
                        // Repopular dropdowns y retornar Page()
                        await CargarListasDesplegablesAsync(); // M�todo para cargar SelectLists
                        return Page();
                    }
                }
                else // Para otros tipos de multa, usar el precio definido en TipoMulta
                {
                    Multa.Monto = tipoMultaSeleccionado.MontoBase; // Asumiendo que TipoMulta tiene Precio_Multa
                }
            }
            else
            {
                ModelState.AddModelError("Multa.IdTipomulta", "El tipo de multa seleccionado no es v�lido.");
                await CargarListasDesplegablesAsync();
                return Page();
            }
            if (!ModelState.IsValid) { 
                await CargarListasDesplegablesAsync();
                return Page();
                                     }

            _context.Multas.Add(Multa);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }
    }
}
