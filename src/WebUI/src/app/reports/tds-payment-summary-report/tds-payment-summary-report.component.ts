import { Component, OnDestroy, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormGroupDirective } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { fuseAnimations } from '@fuse/animations';
import * as Xlsx from 'xlsx';
import { PropertyService } from '../../property/property.service';
import { ToastrService } from 'ngx-toastr';
import * as _ from 'lodash';
import { TdsPaymentSummaryReportService } from '../tds-payment-summary-report/tds-payment-summary-report.service';
import * as moment from 'moment';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'tds-payment-summary-report',
  templateUrl: './tds-payment-summary-report.component.html',
  styleUrls: ['./tds-payment-summary-report.component.scss'],
  animations: fuseAnimations
})
export class TdsPaymentSummaryReportComponent implements OnInit, OnDestroy {
  reportform: FormGroup;

  reportRowData: any[] = [];
  reportColumnDef: any[] = [];
  premisesDDl: any[] = [];
  userDDl: any[] = [];
  lotNoDDl: any[] = [];

  constructor(private _formBuilder: FormBuilder, private tdsPaymentSvc: TdsPaymentSummaryReportService,  private propertySvc: PropertyService, private toastr: ToastrService) {
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
      { 'header': 'Expected Date of Payment ', 'field': 'expectedPaymentDate', 'type': 'label', 'width': 250 },
      { 'header': 'Total No.of Payments', 'field': 'totalPayment', 'type': 'label', 'width': 70 },
      { 'header': 'TDS Amount', 'field': 'tDSamount', 'type': 'label', 'width': 150 },
      { 'header': 'User', 'field': 'paymentBy', 'type': 'label', 'width': 210 },
      { 'header': 'Completed No.of Payments', 'field': 'completedPayment', 'type': 'label', 'width': 140 },
      { 'header': 'Completed payments TDS value', 'field': 'completedPaymentTDS', 'type': 'label', 'width': 110 },
      { 'header': 'Pending with Remarks', 'field': 'pendingPaymentWithRemark', 'type': 'label', 'width': 140 },
      { 'header': 'Pending payments TDS value with Remarks', 'field': 'pendingPaymentWithRemarkTDS', 'type': 'label', 'width': 170 },
      { 'header': 'Pending without Remarks', 'field': 'pendingPaymentWithoutRemark', 'type': 'label', 'width': 160 },
      { 'header': 'Pending payments TDS value with Remarks', 'field': 'pendingPaymentWithoutRemarkTDS', 'type': 'label', 'width': 200 }
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
    this.tdsPaymentSvc.getLotNo().subscribe(response => {
      var orderedLot = _.orderBy(response, ['lotNo'], ['asc']);
      this.lotNoDDl = orderedLot;
      this.lotNoDDl.splice(0, 0, { 'lotNo': '' });
    });
  }

  getUsers() {
    this.tdsPaymentSvc.getUsers().subscribe(response => {     
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

    this.tdsPaymentSvc.getReportList( filters.premisesId, filters.user,filters.lotNo, moment(filters.fromDate).format("DD-MMM-YYYY"), moment(filters.toDate).format("DD-MMM-YYYY")).subscribe(response => {
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

    this.tdsPaymentSvc.downloadtoExcel( filters.premisesId, filters.user,filters.lotNo, moment(filters.fromDate).format("DD-MMM-YYYY"), moment(filters.toDate).format("DD-MMM-YYYY")).subscribe(response => {
      let blob: any = new Blob([response], { type: 'application/vnd.ms-excel' });
      fileSaver.saveAs(blob, 'TDS_Payment_Summary_Report.xls');
    });
  }

  search() {
    this.getReportList();
  }

  reset() {
    this.reportform.reset();
  }


}
