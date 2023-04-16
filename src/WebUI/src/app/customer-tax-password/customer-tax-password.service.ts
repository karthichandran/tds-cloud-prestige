/** Angular Imports */
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpEvent } from '@angular/common/http';
import { IPropertyVM} from '../property/property.model';
//import { PropertyDto } from '../ReProServices-api';


/** rxjs Imports */
import { Observable } from 'rxjs';

/**
 * Accounting service.
 */
@Injectable({
  providedIn: 'root'
})
export class CustomertaxPasswordService {

  /**
   * @param {HttpClient} http Http Client to send requests.
   */
  constructor(private http: HttpClient) { }



  /**
   * @returns {Observable<any>} Loan products data.
   */
  getCustomers(): Observable<any> {
    return this.http.get('/customertaxlogin/postedcustomers/0');    
  }

  saveCustomers(customers:any): Observable<any> {
    return this.http.post('/customertaxlogin/updateitpwd',customers);    
  }
 

}
