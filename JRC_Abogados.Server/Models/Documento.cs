﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRC_Abogados.Server.Models
{
    public class Documento
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string Path { get; set; }
        public int TipoDocumentoId { get; set; }
        [ForeignKey("TipoDocumentoId")]
        public virtual TipoDocumento TipoDocumento { get; set; }
        public DateTime FechaInicio { get; set; }
        public int ExpedienteId { get; set; }
        [ForeignKey("ExpedienteId")]
        public virtual Expediente Expediente { get; set; }
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }
    }
}
