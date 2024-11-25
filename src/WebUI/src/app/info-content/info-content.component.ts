import { Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, FormGroup, UntypedFormGroup, Validators, FormControl } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { fuseAnimations } from '@fuse/animations';
import { ToastrService } from 'ngx-toastr';
import * as _ from 'lodash';
import { RegistrationStatusService } from '../registration-status/registration-status.service';
import * as _moment from 'moment';

@Component({
    selector: 'app-info-content',
    templateUrl: './info-content.component.html',
    styleUrls: ['./info-content.component.scss'],
    animations: fuseAnimations
})
export class InfoContentComponent implements OnInit, OnDestroy {
   
    searchForm: FormGroup;
   
    SelectRow: any;
    setupData: any = [];
    possessionUnit: string;
    profileTxt:string;   
    paymentToSeller:string;
    form16B:string;
    tdsCompliance:string;
    loginPopUp:string;
    faq:string;
    
    constructor(private _formBuilder: UntypedFormBuilder,  private toastr: ToastrService, private regSvc: RegistrationStatusService ) {
    }

    ngOnInit(): void {

       this.possessionUnit="yes";
       this.getInfo();
    }
  
    isValid(valeu) {
        return valeu !== undefined && valeu !== null && valeu !== "";
    }

    getInfo() {
        var val = this.possessionUnit === "yes" ? 1 : 0;
        this.regSvc.getInfo(val).subscribe(r => {
            this.profileTxt = r.profileTxt;
            this.paymentToSeller = r.paymentToSeller;
            this.tdsCompliance = r.tdsCompliance;
            this.loginPopUp = r.loginPopUp;
            this.faq = r.faq;
            this.form16B = r.form16B;
        });
    }

    save(): void {

        var model = {
            "possessionUnit": this.possessionUnit==="yes"?true:false,
            "profileTxt": this.profileTxt,
            "paymentToSeller": this.paymentToSeller,
            "tdsCompliance": this.tdsCompliance,
            "loginPopUp": this.loginPopUp,
            "form16B": this.form16B,
            "faq": this.faq
        }
        this.regSvc.saveInfo(model).subscribe(res => {
            this.toastr.success("Info is updated");
            this.getInfo();

        });
    }


   onSelectionChange(eve){
    this.getInfo();
   }
   
    /**
       * On destroy
       */
    ngOnDestroy(): void {
    }  

    tabChanged(eve: MatTabChangeEvent) {
        if (eve.index == 0) {
            // this.search();
        }
    }  


    editRow(eve) {
        
    }
   

}
