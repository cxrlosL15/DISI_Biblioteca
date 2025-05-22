using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Libro
{
    [Key]
    [Column("id_Libro")]
    public int IdLibro { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [Display(Name = "Título")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Titulo { get; set; }

    /*
    [Column("id_Autores")]
    public int? IdAutores { get; set; }
    */

    [Required(ErrorMessage = "Seleccione un género")]
    [Column("id_Generos")]
    public int? IdGeneros { get; set; }

    [Display(Name = "¿?")]
    [Column("ID_Disponibilidad")]
    public int? IdDisponibilidad { get; set; }

    [Required(ErrorMessage = "Seleccione una editorial")]
    [Column("ID_Editorial")]
    public int? IdEditorial { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    [Display(Name = "Fecha de impresión")]
    [Column("Fecha_de_impresion")]
    public DateOnly? FechaDeImpresion { get; set; }

    [Required(ErrorMessage = "Seleccione una categoría")]
    [Column("id_Categorias")]
    public int? IdCategorias { get; set; }

    [Required(ErrorMessage = "El valor de reposición es obligatorio")]
    [Range(1, double.MaxValue, ErrorMessage = "El valor de reposición debe ser mayor que 0.")]
    [Display(Name = "Valor de reposición")]
    [Column("Valor_Reposicion")]
    public double? ValorReposicion { get; set; }

    [Required(ErrorMessage = "La imagen es obligatoria")]
    [Column("id_imgLibros")]
    public int? IdImgLibros { get; set; }

    [Required(ErrorMessage = "Seleccione una ubicacion")]
    [Column("id_Ubicacion")]
    public int? IdUbicacion { get; set; }

    /*
    [ForeignKey("IdAutores")]
    [InverseProperty("Libros")]
    public virtual Autore? IdAutores1 { get; set; }
    */

    [Display(Name = "¿?")]
    [ForeignKey("IdCategorias")]
    [InverseProperty("Libros")]
    public virtual Categoria? IdCategoriasNavigation { get; set; }

    [Display(Name = "¿?")]
    [ForeignKey("IdEditorial")]
    [InverseProperty("Libros")]
    public virtual Editorial? IdEditorialNavigation { get; set; }

    [Display(Name = "¿?")]
    [ForeignKey("IdGeneros")]
    [InverseProperty("Libros")]
    public virtual Genero? IdGenerosNavigation { get; set; }

    [Display(Name = "¿?")]
    [ForeignKey("IdImgLibros")]
    [InverseProperty("Libros")]
    public virtual ImgLibro? IdImgLibrosNavigation { get; set; }

    [Display(Name = "¿?")]
    [ForeignKey("IdUbicacion")]
    [InverseProperty("Libros")]
    public virtual Ubicacione? IdUbicacionNavigation { get; set; }

    [InverseProperty("IdLibroNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    /*
    [ForeignKey("IdLibro")]
    [InverseProperty("IdLibros")]
    public virtual ICollection<Autore> IdAutoresNavigation { get; set; } = new List<Autore>();
    */

    [Display(Name = "Autor")]
    public virtual ICollection<LibroAutor> LibroAutores { get; set; } = new List<LibroAutor>();


    [ForeignKey("IdDisponibilidad")]
    [InverseProperty("Libros")]
    public virtual Disponibilidad? IdDisponibilidadNavigation { get; set; }


    // Nuevo campo de borrado lógico
    [Required]
    [Column("BorradoLogico")]
    public bool BorradoLogico { get; set; } = false;

    [Column("Eliminado")]
    public int? Eliminado { get; set; }
}
