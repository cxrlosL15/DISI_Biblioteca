using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Categoria
{
    [Key]
    [Column("id_Categorias")]
    public int IdCategorias { get; set; }

    [Display(Name = "Categoría")]
    [Column("Categoria")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Categoria1 { get; set; }

    [InverseProperty("IdCategoriasNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}