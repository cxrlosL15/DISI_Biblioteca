using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Ticket_Prestamo")]
public partial class TicketPrestamo
{
    [Key]
    [Column("id_Ticket_Prestamo")]
    public int IdTicketPrestamo { get; set; }

    [Column("id_Encabezado_Ticket")]
    public int? IdEncabezadoTicket { get; set; }

    [Column("id_Prestamo")]
    public int? IdPrestamo { get; set; }

    [Column("id_Imagenes")]
    public int? IdImagenes { get; set; }

    public double? Subtotal { get; set; }

    public double? Total { get; set; }

    [ForeignKey("IdEncabezadoTicket")]
    [InverseProperty("TicketPrestamos")]
    public virtual EncabezadoTicket? IdEncabezadoTicketNavigation { get; set; }

    [ForeignKey("IdImagenes")]
    [InverseProperty("TicketPrestamos")]
    public virtual ImagenesTicket? IdImagenesNavigation { get; set; }

    [ForeignKey("IdPrestamo")]
    [InverseProperty("TicketPrestamos")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }
}
