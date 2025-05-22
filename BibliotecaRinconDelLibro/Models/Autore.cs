using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Autore
{
    [Key]
    [Column("Id_Autores")]
    public int IdAutores { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Nombres { get; set; }

    [Column("Apellido_P")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoP { get; set; }

    [Column("Apellido_M")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoM { get; set; }

    [InverseProperty("IdAutores1")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();

    [ForeignKey("IdAutores")]
    [InverseProperty("IdAutoresNavigation")]
    public virtual ICollection<Libro> IdLibros { get; set; } = new List<Libro>();
}
