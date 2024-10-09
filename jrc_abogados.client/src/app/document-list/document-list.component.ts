import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AlertService } from '../services/AlertService';
import { Documento } from '../Models/Documento';
import { DocumentoService } from '../services/document-service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../services/AuthService';

@Component({
  selector: 'app-document-list',
  templateUrl: './document-list.component.html'
})
export class DocumentListComponent implements OnInit {
  user: any;
  documentos: Documento[] = [];
  buscarPalabra: string = '';
  documentoSeleccionado: Documento | null = null;
  expedienteId: any;
  formValidado = false;
  extencionEmail = "gmail";
  nickEmail = ""
  enviandoPDF = false;

  paginaActual: number = 1;
  documentosPorPagina: number = 10;

  campoFiltro: string = 'nombre';

  constructor(
    private documentoService: DocumentoService,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private authService: AuthService,
  ) { }

  ngOnInit(): void {
    this.authService.usuario.subscribe(usuario => {
      this.user = usuario;
    });

    this.route.queryParams.subscribe(params => {
      this.expedienteId = +params['expedienteId'];
    });

    this.getDocumentos();

    this.documentoService.$listaDocumentos.subscribe(() => {
      this.getDocumentos()
    });
  }

  defLink: any;
  @ViewChild('modalPDF') _modal!: ElementRef;
  displayStyle = "none";
  openModal() {
    this.defLink = `${this.documentoSeleccionado?.path}#toolbar=0`

    this._modal.nativeElement.setAttribute('src', this.defLink)
    this.displayStyle = "block";
  }

  closeModal() {
    this.displayStyle = "none";
    this._modal.nativeElement.setAttribute('src', '')
  }

  printPDF() {
    const iframe: HTMLIFrameElement = this._modal.nativeElement;
    if (iframe && iframe.contentWindow) {
      iframe.contentWindow.print();
    }
  }

  downloadPDF() {
    const iframe: HTMLIFrameElement = this._modal.nativeElement;
    if (iframe) {
      const link = document.createElement('a');
      link.href = iframe.src;
      link.download = this.documentoSeleccionado?.nombre || 'documento.pdf';
      link.click();
    }
  }

  seleccionarDocumento(documentoId: number) {
    this.documentoService.seleccionarDocumento(documentoId);
  }

  verDocumento(documento: Documento) {
    this.documentoSeleccionado = documento;
  }

  editarDocumento(clienteId: number) {
    this.seleccionarDocumento(clienteId);
  }

  getDocumentos(): void {
    this.documentoService.getDocumentos(this.expedienteId)
      .subscribe((documentos: Documento[]) => this.documentos = documentos);
  }

  get filtroDocumentos(): Documento[] {
    return this.documentos.filter(expediente => {
      const valor = this.obtenerValor(expediente, this.campoFiltro);
      return valor && valor.toString().toLowerCase().includes(this.buscarPalabra.toLowerCase());
    });
  }

  obtenerValor(obj: any, ruta: string): any {
    return ruta.split('.').reduce((o, i) => o?.[i], obj);
  }

  eliminarDocumento(id: number): void {
    this.documentoService.eliminarDocumento(id, this.user.id)
      .subscribe(() => {
        this.documentos = this.documentos.filter(caso => caso.id !== id);
        this.alertService.showMessage('Documento eliminado con exito.');
      });
  }

  validarEmail(event: KeyboardEvent | ClipboardEvent) {
    const teclaPresionada = (event as KeyboardEvent).key;
    const patron = /^[^\s'"@]$/;

    if (event instanceof KeyboardEvent) {
      if (!patron.test(teclaPresionada) && teclaPresionada !== 'Backspace' && teclaPresionada !== 'Delete' && teclaPresionada !== 'ArrowLeft' && teclaPresionada !== 'ArrowRight') {
        event.preventDefault();
      }
    }

    if (event instanceof ClipboardEvent) {
      const clipboardData = event.clipboardData?.getData('text') || '';
      if (/[@^\s'"]/.test(clipboardData)) {
        event.preventDefault();
      }
    }
  }

  iniciarValidadores() {
    const forms = document.querySelectorAll('.needs-validation');

    Array.from(forms).forEach((form: Element) => {
      const typedForm = form as HTMLFormElement;
      typedForm.addEventListener('submit', (event) => {
        if (this.formValidado) {
          event.preventDefault();
          event.stopPropagation();
          return;
        }
        if (!typedForm.checkValidity()) {
          event.preventDefault();
          event.stopPropagation();
        }
        else {
          this.formValidado = true;
          this.enviarDocumento();
        }

        typedForm.classList.add('was-validated');
      }, false);
    });
  }

  enviarDocumento() {
    this.enviandoPDF = true;
    var email = this.nickEmail + "@" + this.extencionEmail + ".com"
    if (this.documentoSeleccionado) {
      this.documentoService.sendPDF(this.documentoSeleccionado.id, email).subscribe(() => {

        this.cerrarForm();
        this.enviandoPDF = false;
        this.alertService.showMessage('PDF enviado con exito.');
      }, () => {
        this.enviandoPDF = false;
        this.formValidado = false;
      })
    }
  }

  cerrarForm() {
    const button = document.getElementById('bClose');
    if (button instanceof HTMLElement) {
      button.click()
    }
  }

  limpiarForm() {
    this.nickEmail = "";
    this.formValidado = false;
    this.enviandoPDF = false;

    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach((form: Element) => {
      form.classList.remove('was-validated');
    });
  }

  // Métodos para la paginación
  get documentosPaginados(): Documento[] {
    const inicio = (this.paginaActual - 1) * this.documentosPorPagina;
    const fin = inicio + this.documentosPorPagina;
    return this.filtroDocumentos.slice(inicio, fin);
  }

  cambiarPagina(pagina: number): void {
    if (pagina >= 1 && pagina <= this.totalPaginas) {
      this.paginaActual = pagina;
    }
  }

  get totalPaginas(): number {
    return Math.ceil(this.filtroDocumentos.length / this.documentosPorPagina);
  }
}
