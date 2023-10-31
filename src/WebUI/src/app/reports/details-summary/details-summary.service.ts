/** Angular Imports */
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpEvent } from '@angular/common/http';
//import { PropertyDto } from '../ReProServices-api';


/** rxjs Imports */
import { Observable } from 'rxjs';

/**
 * Accounting service.
 */
@Injectable({
  providedIn: 'root'
})
export class DetailsSummaryService {

  /**
   * @param {HttpClient} http Http Client to send requests.
   */
  constructor(private http: HttpClient) { }

  getReportList(lotNo:number): Observable<any> {
    return this.http.get('/detailsSummaryReport/'+lotNo);
  }

  downloadtoExcel(lotNo:number): Observable<any> {
    return this.http.get('/detailsSummaryReport/getExcel/'+lotNo, { responseType: 'blob' });
  }

  getLotNo(): Observable<any> {
    return this.http.get('/clientpayment/lotNumbers');
  }
}
