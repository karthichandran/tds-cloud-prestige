import { Component, OnDestroy, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators, FormGroupDirective } from '@angular/forms';
import { fuseAnimations } from '@fuse/animations';
import { CustomertaxPasswordService } from './customer-tax-password.service';

import { ToastrService } from 'ngx-toastr';
import { GridComponent} from '../../@fuse/components/grid/grid.component';
import * as _ from 'lodash';

@Component({
  selector: 'app-customer-tax-password',
  templateUrl: './customer-tax-password.component.html',
  styleUrls: ['./customer-tax-password.component.scss'],
  animations: fuseAnimations
})
export class CustomerTaxPasswordComponent implements OnInit, OnDestroy {
  @ViewChild(GridComponent) gridComp: GridComponent;

  form: UntypedFormGroup;
 
  rowData: any[] = [];
  columnDef: any[] = [];

  constructor(private _formBuilder: UntypedFormBuilder, private cusTaxPwedSrv: CustomertaxPasswordService, private toastr: ToastrService) {

  }

  ngOnInit(): void {
    // Reactive Form


    this.columnDef = [
      { 'header': '', 'field': 'isSelected', 'type': 'checkbox' ,'checkall': true},
      { 'header': 'Customer Name', 'field':'name','type':'label'},
      { 'header': 'PAN No', 'field': 'pan', 'type': 'label' },
      { 'header': 'Password', 'field': 'taxPassword', 'type': 'label' },
      { 'header': 'OptOut', 'field': 'isOptOut', 'type': 'label' }     
    ];
  
    this.getCustomers();
  }


  getCustomers() {
    this.cusTaxPwedSrv.getCustomers().subscribe((response) => {
      this.rowData = response;
    });
  }

  clear() {
    this.form.reset();
    this.form.get('propertyType').setValue(2);
    this.form.get('isActive').setValue('true');
  }

  refresh(): void {
   this.getCustomers();
  }

  process(): void {

    let isValid = false;
    _.forEach(this.rowData, o => {
      if (o.isSelected)
        isValid = true;
    });

    if(!isValid){
      this.toastr.error("Please select the records then proceed");
      return;
    }
    
    this.cusTaxPwedSrv.saveCustomers(this.rowData).subscribe((response) => {
      this.toastr.success("Customer details are Updated");
      this.refresh();
    },()=>{
      this.toastr.error("Failed to proceed");
    });
  }

  /**
     * On destroy
     */
  ngOnDestroy(): void {
  }

  
   
 
  formatRowData(response: any) {
    function splitData(data: string) {
      let splitData = data.split(',');

      let concatdata: any[] = [];
      _.forEach(splitData, o => {
        concatdata.push({ 'name': o });
      });
      return concatdata;
    }
    _.forEach(response, o => {
      o.sellerNames = splitData(o.sellerNames);
      o.panNumbers = splitData(o.panNumbers);
      o.isActive = o.isActive == null ? true : o.isActive;
    })
    this.rowData = response;
  }

 
}
