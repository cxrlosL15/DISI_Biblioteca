using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BibliotecaRinconDelLibro.Models;
using Microsoft.AspNetCore.Hosting;

namespace BibliotecaRinconDelLibro.Pages.Libros
{
    public class CreateModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult OnGet()
        {
            CargarCombos();
            return Page();
        }

        [BindProperty]
        public Libro Libro { get; set; } = default!;

        [BindProperty]
        public Disponibilidad NuevaDisponibilidad { get; set; } = new Disponibilidad();

        [BindProperty]
        public IFormFile? Imagen { get; set; }

        [BindProperty]
        public List<int> AutoresSeleccionados { get; set; } = new List<int>();


        public async Task<IActionResult> OnPostAsync()
        {
            if (Imagen == null)
            {
                ModelState.AddModelError("Imagen", "La imagen del libro es obligatoria.");
                CargarCombos(); return Page();
            }
            if (AutoresSeleccionados == null || !AutoresSeleccionados.Any())
            {
                ModelState.AddModelError("AutoresSeleccionados", "Debe seleccionar al menos un autor.");
                CargarCombos(); return Page();
            }
            if (AutoresSeleccionados.Count != AutoresSeleccionados.Distinct().Count())
            {
                ModelState.AddModelError("AutoresSeleccionados", "No se permiten autores duplicados.");
                CargarCombos(); return Page();
            }

            ModelState.Remove("Libro.IdImgLibros");
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return Page();
            }

            string rutaRelativa = string.Empty;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Guardar imagen
                string carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "ImagenLibro");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(Imagen.FileName);
                string rutaFisica = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    await Imagen.CopyToAsync(stream);
                }

                rutaRelativa = "/ImagenLibro/" + nombreArchivo;
                var img = new ImgLibro { ImgLibros = rutaRelativa };
                _context.ImgLibros.Add(img);
                await _context.SaveChangesAsync();
                Libro.IdImgLibros = img.IdImgLibros;

                // Guardar libro primero
                _context.Libros.Add(Libro);
                await _context.SaveChangesAsync();

                // Inicializar y guardar disponibilidad una sola vez
                NuevaDisponibilidad.IdLibro = Libro.IdLibro;
                NuevaDisponibilidad.CopiasPrestadas = 0;
                NuevaDisponibilidad.TotalDespuesPrestamos = NuevaDisponibilidad.TotalLibros;

                _context.Disponibilidads.Add(NuevaDisponibilidad);
                await _context.SaveChangesAsync();

                // Vincular disponibilidad al libro
                Libro.IdDisponibilidad = NuevaDisponibilidad.IdDisponibilidad;
                _context.Update(Libro);
                await _context.SaveChangesAsync();

                // Guardar autores
                foreach (var idAutor in AutoresSeleccionados)
                {
                    var libroAutor = new LibroAutor
                    {
                        IdLibro = Libro.IdLibro,
                        IdAutor = idAutor
                    };
                    _context.LibroAutores.Add(libroAutor);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al guardar el libro: {mensajeError}");

                if (!string.IsNullOrEmpty(rutaRelativa))
                {
                    string rutaFisica = Path.Combine(_webHostEnvironment.WebRootPath, rutaRelativa.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(rutaFisica))
                        System.IO.File.Delete(rutaFisica);
                }

                CargarCombos();
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private void CargarCombos()
        {
            ViewData["IdCategorias"] = new SelectList(_context.Categorias, "IdCategorias", "Categoria1");
            ViewData["IdDisponibilidad"] = new SelectList(_context.Disponibilidads, "IdDisponibilidad", "IdDisponibilidad");
            ViewData["IdEditorial"] = new SelectList(_context.Editorials, "IdEditorial", "Nombre");
            ViewData["IdGeneros"] = new SelectList(_context.Generos, "IdGeneros", "Generos");
            ViewData["IdImgLibros"] = new SelectList(_context.ImgLibros, "IdImgLibros", "IdImgLibros");
            ViewData["IdUbicacion"] = new SelectList(_context.Ubicaciones.Select(u => new { u.IdUbicacion, Descripcion = "Pasillo " + u.Pasillo + " - Estante " + u.Estante }), "IdUbicacion", "Descripcion");
            ViewData["IdAutores"] = new SelectList(_context.Autores.Select(a => new {a.IdAutores, NombreCompleto = a.Nombres + " " + a.ApellidoP + " " + a.ApellidoM }), "IdAutores", "NombreCompleto");
        }
    }
}
