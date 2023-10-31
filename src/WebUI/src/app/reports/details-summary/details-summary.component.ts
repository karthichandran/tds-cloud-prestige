import { Component, OnDestroy, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators, FormGroupDirective } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { fuseAnimations } from '@fuse/animations';
import * as Xlsx from 'xlsx';
import { PropertyService } from '../../property/property.service';
import { ToastrService } from 'ngx-toastr';
import * as _ from 'lodash';
import { DetailsSummaryService} from '../details-summary/details-summary.service';
import { SellerService } from '../../seller/seller.service';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'details-summary-report',
  templateUrl: './details-summary.component.html',
  styleUrls: ['./details-summary.component.scss'],
  animations: fuseAnimations
})
export class DetailsSummaryComponent implements OnInit, OnDestroy {
  reportform: UntypedFormGroup;

  reportRowData: any[] = [];
  reportColumnDef: any[] = []; 

  constructor(private _formBuilder: UntypedFormBuilder, private lotSummarySvc: DetailsSummaryService, private sellerSvc: SellerService, private propertySvc: PropertyService, private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.reportform = this._formBuilder.group({
      lotNo: ['']
     
    });

    this.reportColumnDef = [
      { 'header': 'Lot No', 'field': 'lotNo', 'type': 'label', 'width': 80 },      
      { 'header': 'Project Name', 'field': 'addressPremises', 'type': 'label', 'width': 200 },
      { 'header': 'No.of Client Payments', 'field': 'totalPayment', 'type': 'label', 'width': 200 },     
      { 'header': 'Total TDS Value', 'field': 'tds', 'type': 'label', 'width': 200 },
      { 'header': 'Total TDS Value Paid', 'field': 'tdsPaid', 'type': 'label', 'width': 240 },
      { 'header': 'DA True', 'field': 'daCompleted', 'type': 'label', 'width': 150 },
      { 'header': 'DA False', 'field': 'daPending', 'type': 'label', 'width': 150 },
      { 'header': 'Form 16B Requested', 'field': 'f16bRequested', 'type': 'label', 'width': 200 },
      { 'header': 'Form 1B Downloaded', 'field': 'f16bDownloaded', 'type': 'label', 'width': 200  },
      { 'header': 'Form 16B sent to Customer', 'field': 'f16bEmailed', 'type': 'label', 'width': 220 },
      { 'header': 'Only TDS Payment', 'field': 'onlyTds', 'type': 'label', 'width': 150  },
      { 'header': 'Pending records', 'field': 'pending', 'type': 'label', 'width': 150  },
      { 'header': 'Resolved records', 'field': 'resolved', 'type': 'label','width':150 }
    ];
  
  }


  getReportList() {  
    var lotNo=this.reportform.value.lotNo;
    if(lotNo==null || lotNo=="" || lotNo==undefined)
    return;
    this.lotSummarySvc.getReportList(lotNo).subscribe(response => {     
      this.reportRowData = response;
    });
  }

  /**
     * On destroy
     */
  ngOnDestroy(): void {
  }

  downloadExcel() {   
    var lotNo=this.reportform.value.lotNo;
    if(lotNo==null || lotNo=="" || lotNo==undefined)
    return;
    this.lotSummarySvc.downloadtoExcel(lotNo).subscribe(response => {
      let blob: any = new Blob([response], { type: 'application/vnd.ms-excel' });
      fileSaver.saveAs(blob, 'DetailsSummaryReport.xls');
    });
  }

  search() {
    this.getReportList();
  }

  reset() {
    this.reportform.reset();
  }


}
