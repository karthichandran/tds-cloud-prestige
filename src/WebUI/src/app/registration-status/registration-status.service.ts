/** Angular Imports */
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpEvent } from '@angular/common/http';
import { IPropertyVM } from '../property/property.model';
import { timeout } from 'rxjs/operators';


/** rxjs Imports */
import { Observable } from 'rxjs';
//import { isDefined } from '@angular/compiler/src/util';

/**
 * Accounting service.
 */
@Injectable({
  providedIn: 'root'
})
export class RegistrationStatusService {

  /**
   * @param {HttpClient} http Http Client to send requests.
   */
  constructor(private http: HttpClient) { }


  /**
   * @param {string} provisioningEntryId Provisioning entry ID of provisioning entry.
   * @returns {Observable<any>} Provisioning entry.
   */
  isValid(param:any) {
    return (param != "" && param != null && !(param===undefined));
  }
  getUserList(premises: any, customer: any,unitno: any): Observable<any> {
    let params = new HttpParams();
    if (this.isValid(premises))
      params = params.set("projectId", premises);
    if (this.isValid(customer))
      params = params.set("customerId", customer);
    if (this.isValid(unitno))
      params = params.set("unitNo", unitno);

    return this.http.get(`/portalregistration/user-list`, { params: params });
  }
  download(premises: any, customer: any,unitno: any): Observable<any> {
    let params = new HttpParams();
    if (this.isValid(premises))
      params = params.set("projectId", premises);
    if (this.isValid(customer))
      params = params.set("customerId", customer);
    if (this.isValid(unitno))
      params = params.set("unitNo", unitno);

    return this.http.get(`/portalregistration/getExcel`, { params: params , responseType: 'blob' }).pipe(timeout(300000));
  }

  getCustomerAndUnitNoDdlList(projectId: any): Observable<any> {
    
    return this.http.get(`/portalregistration/customer-unitno/${projectId}`);
  }
  /**
   * @returns {Observable<any>} Loan products data.
   */
  getSetup(): Observable<any> {
    return this.http.get('/portalregistration/setup');
  }

  updatePwd(userModel: any): Observable<any> {         
    return this.http.put('/portalregistration/update-pwd',  userModel );  
  } 

  getInfo(status:any): Observable<any> {
    return this.http.get('/portalregistration/infocontent/'+status);
  }
  saveInfo(model: any): Observable<any> {         
    return this.http.put('/portalregistration/save-info',  model );  
  }
}
