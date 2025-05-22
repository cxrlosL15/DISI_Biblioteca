using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("EstadoRegreso")]
public partial class EstadoRegreso
{
    [Key]
    [Column("Id_EstadoRegreso")]
    public int IdEstadoRegreso { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DescripcionEstado { get; set; }

    [InverseProperty("IdEstadoRegresoNavigation")]
    public virtual ICollection<Devolucione> Devoluciones { get; set; } = new List<Devolucione>();
}
