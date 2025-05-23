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

    [Required(ErrorMessage = "Favor de ingresar un nombre.")]
    [Column("nombre")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Favor de ingresar un apellido.")]
    [Column("apellidoP")]
    [StringLength(255)]
    [Unicode(false)]
    [Display(Name = "Apellido Paterno")]
    public string? ApellidoP { get; set; } = string.Empty;

    [Required(ErrorMessage = "Favor de ingresar un apellido.")]
    [Column("apellidoM")]
    [StringLength(255)]
    [Unicode(false)]
    [Display(Name = "Apellido Materno")]
    public string? ApellidoM { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [Column("correo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Correo { get; set; } = string.Empty;

    [Column("id_direccion")]
    [Display(Name = "Dirección")]
    public int? IdDireccion { get; set; }

    [Required(ErrorMessage = "La valoración es obligatoria.")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Valoracion { get; set; }

    [ForeignKey("IdDireccion")]
    [InverseProperty("Clientes")]
    [Display(Name = "Dirección")]
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

   /* [Column("Eliminado")]
    public int? Eliminado { get; set; }*/

    [Column("EstadoCliente")]
    public int? EstadoCliente { get; set; } = 1;

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {ApellidoP} {ApellidoM}";
}
