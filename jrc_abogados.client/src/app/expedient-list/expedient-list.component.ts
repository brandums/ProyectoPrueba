import { Component, OnInit } from '@angular/core';
import { AlertService } from '../services/AlertService';
import { Expediente } from '../Models/Expediente';
import { ExpedienteService } from '../services/expediente-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-expedient-list',
  templateUrl: './expedient-list.component.html'
})
export class ExpedientListComponent implements OnInit {
  expedientes: Expediente[] = [];
  buscarPalabra: string = '';
  expedienteSeleccionado: Expediente | null = null;

  paginaActual: number = 1;
  expedientesPorPagina: number = 10;

  campoFiltro: string = 'nombre';

  constructor(
    private expedienteService: ExpedienteService,
    private alertService: AlertService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.getExpedientes();

    this.expedienteService.$listaExpedientes.subscribe(data => this.expedientes = data);
  }

  openDocumentList(expedienteId: number) {
    this.router.navigate(['/document-list'], { queryParams: { expedienteId: expedienteId } });
  }

  seleccionarExpediente(expedienteId: number) {
    this.expedienteService.seleccionarExpediente(expedienteId);
  }

  verExpediente(expediente: Expediente) {
    this.expedienteSeleccionado = expediente;
  }

  editarExpediente(clienteId: number) {
    this.seleccionarExpediente(clienteId);
  }

  getExpedientes(): void {
    this.expedienteService.getExpedientes()
      .subscribe((expedientes: Expediente[]) => this.expedientes = expedientes);
  }

  get filtroExpedientes(): Expediente[] {
    return this.expedientes.filter(expediente => {
      const valor = this.obtenerValor(expediente, this.campoFiltro);
      return valor && valor.toString().toLowerCase().includes(this.buscarPalabra.toLowerCase());
    });
  }

  obtenerValor(obj: any, ruta: string): any {
    return ruta.split('.').reduce((o, i) => o?.[i], obj);
  }

  eliminarExpediente(id: number): void {
    this.expedienteService.eliminarExpediente(id)
      .subscribe(() => {
        this.expedientes = this.expedientes.filter(caso => caso.id !== id);
        this.alertService.showMessage('Expediente eliminado con exito.');
      });
  }

  // Métodos para la paginación
  get expedientesPaginados(): Expediente[] {
    const inicio = (this.paginaActual - 1) * this.expedientesPorPagina;
    const fin = inicio + this.expedientesPorPagina;
    return this.filtroExpedientes.slice(inicio, fin);
  }

  cambiarPagina(pagina: number): void {
    if (pagina >= 1 && pagina <= this.totalPaginas) {
      this.paginaActual = pagina;
    }
  }

  get totalPaginas(): number {
    return Math.ceil(this.filtroExpedientes.length / this.expedientesPorPagina);
  }
}
