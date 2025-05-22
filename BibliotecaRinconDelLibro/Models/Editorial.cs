using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Editorial")]
public partial class Editorial
{
    [Key]
    [Column("ID_Editorial")]
    public int IdEditorial { get; set; }

    [Display(Name = "Editorial")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Nombre { get; set; }

    // Nuevo campo de borrado lógico
    [Required]
    [Column("BorradoLogico")]
    public bool BorradoLogico { get; set; } = false;


    [InverseProperty("IdEditorialNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
