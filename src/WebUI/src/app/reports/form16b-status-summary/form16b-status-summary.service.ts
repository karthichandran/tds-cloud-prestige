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
export class Form16BStatusSummaryService {

  /**
   * @param {HttpClient} http Http Client to send requests.
   */
  constructor(private http: HttpClient) { }

  getReportList( propertyId: string,  user: string, lot: string,fromDate:string,toDate: string): Observable<any> {
    let params = new HttpParams();   
    if (propertyId != "" && propertyId != null)
      params = params.set("propertyId", propertyId);    
    if (user != "" && user != null)
      params = params.set("paymentBy", user);
    if (lot != "" && lot != null)
      params = params.set("lotNo", lot);   
      params = params.set("expectedFromDate", fromDate);
      params = params.set("expectedToDate", toDate);


    return this.http.get('/Form16BStatusSummary', { params: params });
  }

  downloadtoExcel( propertyId: string,  user: string, lot: string,fromDate:string,toDate: string): Observable<any> {
    let params = new HttpParams();   
    if (propertyId != "" && propertyId != null)
      params = params.set("propertyId", propertyId);    
    if (user != "" && user != null)
      params = params.set("PaymentBy", user);
    if (lot != "" && lot != null)
      params = params.set("lotNo", lot);   
      params = params.set("ExpectedFromDate", fromDate);
      params = params.set("ExpectedToDate", toDate);

    return this.http.get('/Form16BStatusSummary/getExcel', { params: params, responseType: 'blob' });
  }

  getLotNo(): Observable<any> {
    return this.http.get('/clientpayment/lotNumbers');
  }

  getUsers(): Observable<any> {
    return this.http.get('/TdsRemittance/UserList');
  }
}
