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

    [StringLength(255)]
    [Unicode(false)]
    public string? Titulo { get; set; }

    [Column("id_Autores")]
    public int? IdAutores { get; set; }

    [Column("id_Generos")]
    public int? IdGeneros { get; set; }

    [Column("ID_Disponibilidad")]
    public int? IdDisponibilidad { get; set; }

    [Column("ID_Editorial")]
    public int? IdEditorial { get; set; }

    [Column("Fecha_de_impresion")]
    public DateOnly? FechaDeImpresion { get; set; }

    [Column("id_Categorias")]
    public int? IdCategorias { get; set; }

    [Column("Valor_Reposicion")]
    public double? ValorReposicion { get; set; }

    [Column("id_imgLibros")]
    public int? IdImgLibros { get; set; }

    [Column("id_Ubicacion")]
    public int? IdUbicacion { get; set; }

    [ForeignKey("IdAutores")]
    [InverseProperty("Libros")]
    public virtual Autore? IdAutores1 { get; set; }

    [ForeignKey("IdCategorias")]
    [InverseProperty("Libros")]
    public virtual Categoria? IdCategoriasNavigation { get; set; }

    [ForeignKey("IdEditorial")]
    [InverseProperty("Libros")]
    public virtual Editorial? IdEditorialNavigation { get; set; }

    [ForeignKey("IdGeneros")]
    [InverseProperty("Libros")]
    public virtual Genero? IdGenerosNavigation { get; set; }

    [ForeignKey("IdImgLibros")]
    [InverseProperty("Libros")]
    public virtual ImgLibro? IdImgLibrosNavigation { get; set; }

    [ForeignKey("IdUbicacion")]
    [InverseProperty("Libros")]
    public virtual Ubicacione? IdUbicacionNavigation { get; set; }

    [Column("Eliminado")]
    public int? Eliminado { get; set; }


    [InverseProperty("IdLibroNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    [ForeignKey("IdLibro")]
    [InverseProperty("IdLibros")]
    public virtual ICollection<Autore> IdAutoresNavigation { get; set; } = new List<Autore>();
}
