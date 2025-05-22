using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Rol")]
public partial class Rol
{
    [Key]
    [Column("id_rol")]
    public int IdRol { get; set; }

    [Column("tipo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Tipo { get; set; }

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
