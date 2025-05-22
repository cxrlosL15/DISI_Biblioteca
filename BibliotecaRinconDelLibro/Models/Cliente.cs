using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Cliente
{
    [Key]
    [Column("id_clientes")]
    public int IdClientes { get; set; }

    [Column("nombre")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Nombre { get; set; }

    [Column("apellidoP")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoP { get; set; }

    [Column("apellidoM")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoM { get; set; }

    [Column("correo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Correo { get; set; }

    [Column("id_direccion")]
    public int? IdDireccion { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Valoracion { get; set; }

    [ForeignKey("IdDireccion")]
    [InverseProperty("Clientes")]
    public virtual Direccion? IdDireccionNavigation { get; set; }

    public string DireccionCompleta
    {
        get
        {
            // Verificar si IdDireccionNavigation no es nulo antes de acceder a sus propiedades
            return $"{IdDireccionNavigation?.Calle}, {IdDireccionNavigation?.Colonia}, {IdDireccionNavigation?.CodigoPostal}, {IdDireccionNavigation?.Ciudad}, {IdDireccionNavigation?.Estado}";
        }
    }

    [InverseProperty("IdClientesNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    [Column("Eliminado")]
    public int? Eliminado { get; set; }

    [Column("EstadoCliente")]
    public int? EstadoCliente { get; set; } = 1;

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {ApellidoP} {ApellidoM}";
}
