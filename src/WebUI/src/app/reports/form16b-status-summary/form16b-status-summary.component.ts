import { Component, OnDestroy, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormGroupDirective } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { fuseAnimations } from '@fuse/animations';
import * as Xlsx from 'xlsx';
import { PropertyService } from '../../property/property.service';
import { ToastrService } from 'ngx-toastr';
import * as _ from 'lodash';
import { Form16BStatusSummaryService } from './form16b-status-summary.service';
import * as moment from 'moment';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'form16b-status-summary',
  templateUrl: './form16b-status-summary.component.html',
  styleUrls: ['./form16b-status-summary.component.scss'],
  animations: fuseAnimations
})
export class Form16BStatusSummaryComponent implements OnInit, OnDestroy {
  reportform: FormGroup;

  reportRowData: any[] = [];
  reportColumnDef: any[] = [];
  premisesDDl: any[] = [];
  userDDl: any[] = [];
  lotNoDDl: any[] = [];

  constructor(private _formBuilder: FormBuilder, private form16bSvc: Form16BStatusSummaryService,  private propertySvc: PropertyService, private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.reportform = this._formBuilder.group({
      lotNo: [''],
      premisesId: [''],
      fromDate: [''],
      toDate: [''],
      user:['']
    });

    this.reportColumnDef = [
      { 'header': 'Lot No', 'field': 'lotNo', 'type': 'label', 'width': 70 },
      { 'header': 'Date ', 'field': 'expectedPaymentDate', 'type': 'label', 'width': 50 },
      // { 'header': 'Total No.of Payments without Remarks', 'field': 'totalPaymentWithoutRemark', 'type': 'label', 'width': 70 },
      { 'header': 'Completed No.of payments', 'field': 'noOfCompleted', 'type': 'label', 'width': 150 }, 
      { 'header': 'No.of Pending challan download', 'field': 'noOfCompleted', 'type': 'label', 'width': 150 },    
      { 'header': 'No.of Challan downloaded', 'field': 'noOfChallanDownloaded', 'type': 'label', 'width': 110 },
      { 'header': 'No.of Form 16B Requested', 'field': 'noOfForm16BReq', 'type': 'label', 'width': 170 },
      { 'header': 'No.of Form 16B Downloaded', 'field': 'noOfForm16BDownloaded', 'type': 'label', 'width': 160 },
      { 'header': 'Pending with Remarks', 'field': 'pendingWithRemarks', 'type': 'label', 'width': 140 }   
     
    ];

    this.getProperties();  
    this.getLotNo();
    this.getUsers();
  }

  getProperties() {
    this.propertySvc.getProperties().subscribe(response => {
      var orderedRes = _.orderBy(response, ['addressPremises'], ['asc']);
    
      this.premisesDDl = orderedRes;
      this.premisesDDl.splice(0, 0, { 'propertyID': '', 'addressPremises': '' });
    });
  }

  getLotNo() {
    this.form16bSvc.getLotNo().subscribe(response => {
      var orderedLot = _.orderBy(response, ['lotNo'], ['asc']);
      this.lotNoDDl = orderedLot;
      this.lotNoDDl.splice(0, 0, { 'lotNo': '' });
    });
  }

  getUsers() {
    this.form16bSvc.getUsers().subscribe(response => {     
      this.userDDl = response;
      this.userDDl.splice(0, 0, {'id':'', 'userName': '' });
    });
  }
  validateDate() {
    let fromDate = this.reportform.value.fromDate;
    let toDate = this.reportform.value.toDate;
    if (fromDate != "" && toDate != "") {
      if (moment(fromDate) > moment(toDate)) {
        this.toastr.error("FromDate should be less than ToDate");
        return false;
      }
    }
    return true;
  }
  getReportList() {
    var filters = this.reportform.value;

    this.form16bSvc.getReportList( filters.premisesId, filters.user,filters.lotNo, moment(filters.fromDate).format("DD-MMM-YYYY"), moment(filters.toDate).format("DD-MMM-YYYY")).subscribe(response => {
      _.forEach(response, obj => {
        obj.expectedPaymentDate = obj.expectedPaymentDate == null ? "" : moment(obj.expectedPaymentDate).local().format("DD-MMM-YYYY");
      });
      this.reportRowData = response;
    });
  }

  /**
     * On destroy
     */
  ngOnDestroy(): void {
  }

  downloadExcel() {
    var filters = this.reportform.value;

    this.form16bSvc.downloadtoExcel( filters.premisesId, filters.user,filters.lotNo, moment(filters.fromDate).format("DD-MMM-YYYY"), moment(filters.toDate).format("DD-MMM-YYYY")).subscribe(response => {
      let blob: any = new Blob([response], { type: 'application/vnd.ms-excel' });
      fileSaver.saveAs(blob, 'Form16B_Status_Summary_Report.xls');
    });
  }

  search() {
    this.getReportList();
  }

  reset() {
    this.reportform.reset();
  }


}
