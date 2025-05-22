using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Ubicacione
{
    [Required(ErrorMessage = "La ubicación es obligatoria")]
    [Key]
    [Column("id_ubicacion")]
    public int IdUbicacion { get; set; }

    [Display(Name = "Pasillo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Pasillo { get; set; }

    [Display(Name = "Estante")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Estante { get; set; }

    // Nuevo campo de borrado lógico
    [Required]
    [Column("BorradoLogico")]
    public bool BorradoLogico { get; set; } = false;

    [InverseProperty("IdUbicacionNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
