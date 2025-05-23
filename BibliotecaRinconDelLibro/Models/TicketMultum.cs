using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class TicketMultum
{
    [Key]
    [Column("id_Ticket_Multa")]
    public int IdTicketMulta { get; set; }

    [Column("id_Encabezado_Ticket")]
    public int? IdEncabezadoTicket { get; set; }

    [Column("id_TipoMulta")]
    public int? IdTipoMulta { get; set; }

    [Column("id_Prestamo")]
    public int? IdPrestamo { get; set; }

    [Column("subtotal")]
    public double? Subtotal { get; set; }

    public double? Total { get; set; }

    [Column("id_Imagenes")]
    public int? IdImagenes { get; set; }

    [ForeignKey("IdEncabezadoTicket")]
    [InverseProperty("TicketMulta")]
    public virtual EncabezadoTicket? IdEncabezadoTicketNavigation { get; set; }

    [ForeignKey("IdImagenes")]
    [InverseProperty("TicketMulta")]
    public virtual ImagenesTicket? IdImagenesNavigation { get; set; }

    [ForeignKey("IdPrestamo")]
    [InverseProperty("TicketMulta")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }

    [ForeignKey("IdTipoMulta")]
    [InverseProperty("TicketMulta")]
    public virtual TipoMultum? IdTipoMultaNavigation { get; set; }

    [ForeignKey("id_multa")]
    [InverseProperty("TicketMulta")]
    public virtual Multa? IdMultaNavigation { get; set; }

}
