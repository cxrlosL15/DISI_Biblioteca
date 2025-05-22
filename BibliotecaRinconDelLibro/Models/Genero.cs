using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Genero
{
    [Key]
    [Column("id_Generos")]
    public int IdGeneros { get; set; }

    [Display(Name = "Género")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Generos { get; set; }

    [InverseProperty("IdGenerosNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
