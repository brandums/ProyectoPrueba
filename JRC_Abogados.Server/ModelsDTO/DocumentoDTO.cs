namespace JRC_Abogados.Server.ModelsDTO
{
    public class DocumentoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int TipoDocumentoId { get; set; }
        public int ExpedienteId { get; set; }
    }
}
