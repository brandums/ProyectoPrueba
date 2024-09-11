import { Component, Input, OnInit } from '@angular/core';
import { Cliente } from '../Models/Cliente';
import { AlertService } from '../services/AlertService';
import { Documento } from '../Models/Documento';
import { TipoDocumento } from '../Models/TipoDocumento';
import { TipoDocumentoService } from '../services/tipoDocumento-service';
import { DocumentoService } from '../services/document-service';

@Component({
  selector: 'app-create-document',
  templateUrl: './create-document.component.html'
})
export class CreateDocumentComponent implements OnInit {
  documento: Documento = new Documento;
  documentoId = 0;
  clientes: Cliente[] = [];
  creandoDocumento = false;
  tituloForm = "Crear nuevo Documento";
  formValidado = false;
  errorMensaje: string | null = null;
  archivoSeleccionado: File | null = null;

  @Input() expedienteId!: number; 

  constructor(
    private documentoService: DocumentoService,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.documentoService.$documentoId.subscribe(id => {
      this.documentoId = id;
      if (id != 0) {
        this.documentoService.getDocumento(id).subscribe(data => {
          this.documento = data;
        })
        this.tituloForm = "Actualizar datos del Documento"
      }
      else {
        this.limpiarForm()
      }
    })
  }

  validarTexto(event: KeyboardEvent, minChars: number) {
    const teclaPresionada = event.key;
    const valorActual = (event.target as HTMLInputElement).value;

    if (valorActual.length <= minChars && teclaPresionada === ' ') {
      event.preventDefault();
      return;
    }

    if (valorActual.slice(-1) === ' ' && teclaPresionada === ' ') {
      event.preventDefault();
      return;
    }
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    const fileType = file.type;

    if (fileType === 'application/pdf') {
      this.archivoSeleccionado = file;
    } else {
      this.archivoSeleccionado = null;
      alert('Por favor, seleccione un archivo en formato PDF.');
      event.target.value = '';
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
          if (this.documentoId != 0) {
            this.actualizarDocumento();
          }
          else {
            this.crearDocumento();
          }
        }

        typedForm.classList.add('was-validated');
      }, false);
    });
  }

  crearDocumento() {
    this.creandoDocumento = true;
    this.errorMensaje = null;

    this.documento.expedienteId = this.expedienteId;

    const formData = new FormData();
    formData.append('file', this.archivoSeleccionado as Blob);
    formData.append('nombre', this.documento.nombre);
    formData.append('descripcion', this.documento.descripcion);
    formData.append('expedienteId', this.documento.expedienteId.toString());

    this.documentoService.crearDocumento(formData).subscribe(() => {
      this.documentoService.nuevoDocumento();

      this.cerrarForm();
      this.creandoDocumento = false;
      this.alertService.showMessage('Documento creado con exito.');
    }, error => {
      this.errorMensaje = error.message;
      this.creandoDocumento = false;
      this.formValidado = false;
    })
  }

  actualizarDocumento() {
    this.creandoDocumento = true;

    this.documentoService.actualizarDocumento(this.documentoId, this.documento).subscribe(() => {
      this.documentoService.nuevoDocumento();

      this.cerrarForm();
      this.creandoDocumento = false;
      this.alertService.showMessage('Expediente actualizado con exito.');
    }, () => {
      this.creandoDocumento = false;
      this.formValidado = false;
    })
  }

  cerrarForm() {
    const button = document.getElementById('bClose');
    if (button instanceof HTMLElement) {
      button.click()
    }
  }

  limpiarForm() {
    this.documento = new Documento;
    this.tituloForm = "Crear nuevo Documento"
    this.formValidado = false;
    this.errorMensaje = null;

    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach((form: Element) => {
      form.classList.remove('was-validated');
    });
  }
}
