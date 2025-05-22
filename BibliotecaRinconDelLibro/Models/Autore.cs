using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Autore
{
    [Required(ErrorMessage = "El autor es obligatorio")]
    [Key]
    [Column("Id_Autores")]
    public int IdAutores { get; set; }

    [Display(Name = "Nombre")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Nombres { get; set; }

    [Display(Name = "Apellido Paterno")]
    [Column("Apellido_P")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoP { get; set; }

    [Display(Name = "Apellido Materno")]
    [Column("Apellido_M")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoM { get; set; }

    // Nuevo campo de borrado lógico
    [Required]
    [Column("BorradoLogico")]
    public bool BorradoLogico { get; set; } = false;

    public virtual ICollection<LibroAutor> LibroAutores { get; set; } = new List<LibroAutor>();
}
