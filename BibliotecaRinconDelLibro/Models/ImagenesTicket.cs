using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("ImagenesTicket")]
public partial class ImagenesTicket
{
    [Key]
    [Column("id_Imagenes")]
    public int IdImagenes { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Imagenes { get; set; }

    [InverseProperty("IdImagenesNavigation")]
    public virtual ICollection<TicketMultum> TicketMulta { get; set; } = new List<TicketMultum>();

    [InverseProperty("IdImagenesNavigation")]
    public virtual ICollection<TicketPrestamo> TicketPrestamos { get; set; } = new List<TicketPrestamo>();
}
