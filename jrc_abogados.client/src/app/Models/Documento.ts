import { Cliente } from "./Cliente";
import { Expediente } from "./Expediente";
import { TipoDocumento } from "./TipoDocumento";

export class Documento {
  id: number = 0;
  nombre: string = "";
  path: string = "";
  fechaInicio: string = "";
  tipoDocumentoId = "";
  tipoDocumento: TipoDocumento = new TipoDocumento;
  clienteId = "";
  cliente: Cliente = new Cliente;
  expedienteId = 0;
  expediente: Expediente = new Expediente;
}
