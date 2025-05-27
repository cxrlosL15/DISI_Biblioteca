using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaRinconDelLibro.Pages.Libros
{
    public class EditModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(BibliotecaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [BindProperty]
        public Libro Libro { get; set; } = default!;
        [BindProperty]
        public IFormFile? Imagen { get; set; }
        [BindProperty]
        public Disponibilidad NuevaDisponibilidad { get; set; } = new Disponibilidad();
        [BindProperty]
        public string SelectedOperation { get; set; }
        [BindProperty]
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "Por favor indica cuántos libros quieres agregar o retirar.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Quantity { get; set; }
        [BindProperty]
        public List<int> AutoresSeleccionados { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null){ return NotFound();}

            var libro =  await _context.Libros
                .Include(l => l.IdImgLibrosNavigation)
                .Include(l => l.IdDisponibilidadNavigation)
                .FirstOrDefaultAsync(m => m.IdLibro == id);

            if (libro == null){ return NotFound();}

            Libro = libro;

            AutoresSeleccionados = await _context.LibroAutores
                .Where(la => la.IdLibro == libro.IdLibro)
                .Select(la => la.IdAutor)
                .ToListAsync();

            // Inicializamos NuevaDisponibilidad con el TotalLibros actual
            if (libro.IdDisponibilidad.HasValue && libro.IdDisponibilidadNavigation != null)
            {
                NuevaDisponibilidad = new Disponibilidad
                {
                    IdDisponibilidad = libro.IdDisponibilidad.Value,
                    TotalLibros = libro.IdDisponibilidadNavigation.TotalLibros,
                    CopiasPrestadas = libro.IdDisponibilidadNavigation.CopiasPrestadas ?? 0
                };
            }   

            CargarCombos();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove(nameof(SelectedOperation));
            if (string.IsNullOrEmpty(SelectedOperation))
            {
                ModelState.Remove(nameof(Quantity));
            }


            var libroDb = await _context.Libros
              .Include(l => l.IdImgLibrosNavigation)
              .FirstOrDefaultAsync(l => l.IdLibro == Libro.IdLibro);
            if (libroDb == null) return NotFound();

            ModelState.Remove("Libro.IdImgLibros");

            if (Imagen == null)
            {
                Libro.IdImgLibros = libroDb.IdImgLibros;
            }
            

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return Page();
            }

            // 1) Cargo lo que ya existe en BD
          
            var dispDb = await _context.Disponibilidads
                .FirstOrDefaultAsync(d => d.IdDisponibilidad == NuevaDisponibilidad.IdDisponibilidad);

            if (dispDb == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontró la disponibilidad.");
                CargarCombos();
                return Page();
            }

            var originalAutores = await _context.LibroAutores
                .Where(la => la.IdLibro == Libro.IdLibro)
                .Select(la => la.IdAutor)
                .ToListAsync();

            // 2) Detectores de cambio
            bool imageChanged = Imagen != null;
            bool libroChanged = false;
            bool dispChanged = false;
            bool autoresChanged = false;

            // ——— Imagen ———
            if (Imagen != null)
            {
                // 1) Guardar el archivo en disco
                string carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "ImagenLibro");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string nuevoNombreArchivo = Guid.NewGuid() + Path.GetExtension(Imagen.FileName);
                string rutaFisicaNueva = Path.Combine(carpeta, nuevoNombreArchivo);
                using (var stream = new FileStream(rutaFisicaNueva, FileMode.Create))
                {
                    await Imagen.CopyToAsync(stream);
                }

                string rutaRelativaNueva = "/ImagenLibro/" + nuevoNombreArchivo;

                // 2) (Opcional) Borrar físicamente la imagen antigua
                if (!string.IsNullOrEmpty(libroDb.IdImgLibrosNavigation?.ImgLibros))
                {
                    string rutaFisicaVieja = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        libroDb.IdImgLibrosNavigation.ImgLibros.TrimStart('/')
                            .Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(rutaFisicaVieja))
                        System.IO.File.Delete(rutaFisicaVieja);
                }

                // 3) Actualizar la entidad ImgLibro existente, o crear una nueva
                var imgDb = libroDb.IdImgLibrosNavigation;
                if (imgDb != null)
                {
                    imgDb.ImgLibros = rutaRelativaNueva;
                    _context.Update(imgDb);
                }
                else
                {
                    var nuevaImagen = new ImgLibro { ImgLibros = rutaRelativaNueva };
                    _context.Add(nuevaImagen);
                    // enlazamos el libro al nuevo registro
                    libroDb.IdImgLibros = nuevaImagen.IdImgLibros;
                }

                imageChanged = true;
            }



            // ——— Propiedades del libro (mapéo manual) ———
            libroDb.Titulo = Libro.Titulo;
            libroDb.IdCategorias = Libro.IdCategorias;
            libroDb.IdEditorial = Libro.IdEditorial;
            libroDb.IdGeneros = Libro.IdGeneros;
            libroDb.FechaDeImpresion = Libro.FechaDeImpresion;
            libroDb.ValorReposicion = Libro.ValorReposicion;
            libroDb.IdUbicacion = Libro.IdUbicacion;
            // (NO asignas libroDb.IdImgLibros aquí; la dejas intacta)

            _context.Update(libroDb);
            libroChanged = true;



            // ——————— GESTIÓN DE DISPONIBILIDAD ———————
            // Solo si el usuario realmente eligió una operación:
            if (!string.IsNullOrEmpty(SelectedOperation) && Quantity > 0)
            {
                int tot = dispDb.TotalLibros ?? 0;
                int pre = dispDb.CopiasPrestadas ?? 0;

                if (SelectedOperation == "Agregar")
                {
                    dispDb.TotalLibros = tot + Quantity;
                    dispDb.TotalDespuesPrestamos = (dispDb.TotalDespuesPrestamos ?? 0) + Quantity;
                    dispChanged = true;
                }
                else if (SelectedOperation == "Retirar")
                {
                    int maxRetirar = tot - pre;
                    if (Quantity > maxRetirar)
                    {
                        ModelState.AddModelError("Quantity", $"Solo puedes retirar hasta {maxRetirar} libros.");
                        CargarCombos();
                        return Page();
                    }
                    dispDb.TotalLibros = tot - Quantity;
                    dispDb.TotalDespuesPrestamos = (dispDb.TotalDespuesPrestamos ?? 0) - Quantity;
                    dispChanged = true;
                }
            }
                
            

            // ——— Autores ———
            autoresChanged = !AutoresSeleccionados
                .OrderBy(x => x)
                .SequenceEqual(originalAutores.OrderBy(x => x));
            if (autoresChanged)
            {
                var actuales = _context.LibroAutores
                    .Where(la => la.IdLibro == Libro.IdLibro);
                _context.LibroAutores.RemoveRange(actuales);
                foreach (var idAutor in AutoresSeleccionados)
                {
                    _context.LibroAutores.Add(new LibroAutor
                    {
                        IdLibro = Libro.IdLibro,
                        IdAutor = idAutor
                    });
                }
            }

            // 3) Si no hubo cambio en **ninguna** parte, salimos
            if (!imageChanged && !libroChanged && !dispChanged && !autoresChanged)
            {
                // no toca _context.SaveChangesAsync()
                return RedirectToPage("./Index");
            }

            // 4) Todo dentro de una transacción
            using (var trx = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Actualizamos disponibilidad si cambió
                    if (dispChanged)
                        _context.Update(dispDb);

                    await _context.SaveChangesAsync();
                    await trx.CommitAsync();
                }
                catch
                {
                    await trx.RollbackAsync();
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }


        private void CargarCombos()
        {
            ViewData["IdCategorias"] = new SelectList(_context.Categorias, "IdCategorias", "Categoria1");
            ViewData["IdDisponibilidad"] = new SelectList(_context.Disponibilidads, "IdDisponibilidad", "IdDisponibilidad");
            ViewData["IdEditorial"] = new SelectList(_context.Editorials, "IdEditorial", "Nombre");
            ViewData["IdGeneros"] = new SelectList(_context.Generos, "IdGeneros", "Generos");
            ViewData["IdUbicacion"] = new SelectList(_context.Ubicaciones.Select(u => new { u.IdUbicacion, Descripcion = "Pasillo " + u.Pasillo + " - Estante " + u.Estante }), "IdUbicacion", "Descripcion");
            ViewData["IdAutores"] = new SelectList(_context.Autores.Select(a => new { a.IdAutores, NombreCompleto = a.Nombres + " " + a.ApellidoP + " " + a.ApellidoM }), "IdAutores", "NombreCompleto");
        }
    }
}
