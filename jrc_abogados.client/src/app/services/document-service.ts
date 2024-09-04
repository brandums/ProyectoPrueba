import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, map, throwError } from 'rxjs';
import { API_BASE_URL } from '../configuracion';
import { Documento } from '../Models/Documento';

@Injectable({
  providedIn: 'root'
})
export class DocumentoService {

  private baseUrl = `${API_BASE_URL}/Documento`;
  public $listaDocumentos: BehaviorSubject<void> = new BehaviorSubject<void>(undefined);
  public $documentoId: BehaviorSubject<number> = new BehaviorSubject<number>(0);

  constructor(private http: HttpClient) { }

  getDocumentos(id: number): Observable<Documento[]> {
    return this.http.get<Documento[]>(`${this.baseUrl}/documentsByExpedient/${id}`).pipe(
      map((documentos: Documento[]) => {
        return documentos.map(documento => {
          documento.fechaInicio = new Date(documento.fechaInicio).toISOString().split('T')[0];
          return documento;
        });
      })
    );
  }

  getDocumento(id: number): Observable<Documento> {
    return this.http.get<Documento>(`${this.baseUrl}/${id}`).pipe(
      map(documento => {
        documento.fechaInicio = new Date(documento.fechaInicio).toISOString().split('T')[0];
        return documento;
      })
    );
  }

  crearDocumento(formData: FormData): Observable<Documento> {
    return this.http.post<Documento>(`${this.baseUrl}`, formData).pipe(
      catchError((error) => {
        if (error.status === 409) {
          return throwError(() => new Error('Este documento ya está registrado.'));
        }
        return throwError(() => new Error('Error creando el documento. Inténtalo de nuevo más tarde.'));
      })
    );
  }

  actualizarDocumento(id: number, documento: Documento): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, documento);
  }

  eliminarDocumento(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  nuevoDocumento() {
    this.$listaDocumentos.next();
  }

  seleccionarDocumento(id: number) {
    this.$documentoId.next(id)
  }
}
