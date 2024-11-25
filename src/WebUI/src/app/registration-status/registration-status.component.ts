import { Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, FormGroup, UntypedFormGroup, Validators, FormControl } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { fuseAnimations } from '@fuse/animations';
import { ToastrService } from 'ngx-toastr';
import { GridComponent } from '../../@fuse/components/grid/grid.component';
import * as _ from 'lodash';
import * as fileSaver from 'file-saver';
import { RegistrationStatusService } from './registration-status.service';
import * as _moment from 'moment';
import { ConfirmationDialogService } from '../core/confirmation-dialog/confirmation-dialog.service';
import { ReplaySubject, Subject } from 'rxjs';
import { MatSelect } from '@angular/material/select';
import { takeUntil } from 'rxjs/operators';
import { PropertyService } from '../property/property.service';

@Component({
    selector: 'app-restration-status',
    templateUrl: './registration-status.component.html',
    //styleUrls: ['./prospect.component.scss'],
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class RegistrationStatusComponent implements OnInit, OnDestroy {
    @ViewChild(GridComponent) gridComp: GridComponent;
    searchForm: FormGroup;
    rowData: any[] = [];
    columnDef: any[] = [];

    SelectRow: any;
    setupData: any = [];
    passwordElm: string;

    propertyDDl: any[] = [];
    //Property Filter for search
    public propertyFilterCtrlForSearch: FormControl = new FormControl();
    @ViewChild('PropertyFilterSelectForSearch', { static: true }) PropertyFilterSelectForSearch: MatSelect;
    /** Subject that emits when the component has been destroyed. */
    protected _onDestroyOnSearch = new Subject<void>();
    public filteredPropertyForSearch: ReplaySubject<any[]> = new ReplaySubject<any[]>();

    customerDDl: any[] = [];
    //Property Filter for search
    public customerFilterCtrlForSearch: FormControl = new FormControl();
    @ViewChild('CustomerFilterSelectForSearch', { static: true }) CustomerFilterCtrlForSearch: MatSelect;
    /** Subject that emits when the component has been destroyed. */
    public filteredCustomerForSearch: ReplaySubject<any[]> = new ReplaySubject<any[]>();

    unitNoDDl: any[] = [];
    //Property Filter for search
    public unitNoFilterCtrlForSearch: FormControl = new FormControl();
    @ViewChild('UnitNoFilterSelectForSearch', { static: true }) UnitNoFilterCtrlForSearch: MatSelect;
    /** Subject that emits when the component has been destroyed. */
    public filteredUnitNoForSearch: ReplaySubject<any[]> = new ReplaySubject<any[]>();

    constructor(private _formBuilder: UntypedFormBuilder, private propertyService: PropertyService, private toastr: ToastrService, private regSvc: RegistrationStatusService,
        private confirmationDialogSrv: ConfirmationDialogService) {
    }

    ngOnInit(): void {

        this.searchForm = this._formBuilder.group({
            searchByPropertyID: [''],
            searchByCustomerID: [''],
            searchByUnitNo: ['']
        });
        this.columnDef = [
            { 'header': 'Project Name', 'field': 'projectName', 'type': 'label' },
            { 'header': 'Customer Name', 'field': 'customerName', 'type': 'label' },
            { 'header': 'Unit No', 'field': 'unitNo', 'type': 'label' },
            { 'header': 'Registered on', 'field': 'registered', 'type': 'label' },
            { 'header': 'User ID', 'field': 'pan', 'type': 'label' },
            { 'header': 'Password', 'field': 'pwd', 'type': 'label' },
            { 'header': 'Last Logged-In', 'field': 'lastUpdated', 'type': 'label' },
            { 'header': '', 'field': '', 'type': 'button', 'activity': ['edit'], 'width': 30 }
        ];


        //this.getSetup();     
        this.getAllProperties();
        this.propertyFilterCtrlForSearch.valueChanges.pipe(takeUntil(this._onDestroyOnSearch))
            .subscribe(() => {
                this.filterPropertyForSearch();
            });
        this.customerFilterCtrlForSearch.valueChanges.pipe(takeUntil(this._onDestroyOnSearch))
            .subscribe(() => {
                this.filterCustomerForSearch();
            });
        this.unitNoFilterCtrlForSearch.valueChanges.pipe(takeUntil(this._onDestroyOnSearch))
            .subscribe(() => {
                this.filterUnitNoForSearch();
            });
    }


    getAllProperties() {
        this.propertyService.getProperties().subscribe((response) => {
            this.propertyDDl = response;
            this.propertyDDl.splice(0, 0, { 'propertyID': '', 'addressPremises': '' });
        });
    }

    isValid(valeu) {
        return valeu !== undefined && valeu !== null && valeu !== "";
    }

    save(): void {
        if (this.isValid(this.passwordElm) && this.isValid(this.SelectRow) && this.SelectRow.userId !== 0) {
            var model = {
                userId: this.SelectRow.userId,
                pwd: this.passwordElm
            }
            this.regSvc.updatePwd(model).subscribe(res => {
                this.toastr.success("Password is updated");
                //this.getUserList();
                var ele = document.getElementsByClassName('mat-tab-label') as HTMLCollectionOf<HTMLElement>;
                ele[0].click();
            });
        }
        else {
            this.toastr.error("Please fill the Paasword");
        }
    }

    getUserList() {
        var model = this.searchForm.value;

        if (!this.isValid(model.searchByPropertyID) || model.searchByPropertyID == 0) {
            this.toastr.error("Please select the Project");
            return;
        }

        this.regSvc.getUserList(model.searchByPropertyID, model.searchByCustomerID, model.searchByUnitNo).subscribe(res => {
            _.forEach(res, o => {
                if (o.registered !== null)
                    o.registered = _moment(o.registered).local().format('DD-MMM-YYYY');
                if (o.lastUpdated !== null)
                    o.lastUpdated = _moment(o.lastUpdated).local().format('DD-MMM-YYYY');
            });
            this.rowData = res;
        });
    }

    downloadExcel() {
        var model = this.searchForm.value;

        if (!this.isValid(model.searchByPropertyID) || model.searchByPropertyID == 0) {
            this.toastr.error("Please select the Project");
            return;
        }
       
        this.regSvc.download(model.searchByPropertyID, model.searchByCustomerID, model.searchByUnitNo).subscribe((response) => {
            let blob: any = new Blob([response], { type: 'application/vnd.ms-excel' });
            fileSaver.saveAs(blob, 'RegistrationStaus.xls');
          });
      }
    /**
       * On destroy
       */
    ngOnDestroy(): void {
    }

    getSetup() {
        this.regSvc.getSetup().subscribe((response) => {
            this.setupData = response;

            this.propertyDDl = _.clone(this.setupData.projectName);
            this.propertyDDl.splice(0, 0, { 'item1': '', 'item2': '' });

            this.customerDDl = _.clone(this.setupData.customerName);
            this.customerDDl.splice(0, 0, { 'item1': '', 'item2': '' });

            this.unitNoDDl = _.clone(this.setupData.unitNo);
            this.unitNoDDl.splice(0, 0, { 'item1': '', 'item2': '' });
        });
    }

    tabChanged(eve: MatTabChangeEvent) {
        if (eve.index == 0) {
            // this.search();
        }
    }

    search() {
        this.getUserList();
    }

    reset() {
        this.searchForm.reset();
        this.rowData = [];
    }

    editRow(eve) {
        this.SelectRow = eve;
        this.passwordElm = eve.pwd;
        var ele = document.getElementsByClassName('mat-tab-label') as HTMLCollectionOf<HTMLElement>;
        ele[1].click();
    }
    selectedProperty(eve) {

        this.regSvc.getCustomerAndUnitNoDdlList(eve.value).subscribe((response) => {

            this.customerDDl = _.clone(response.customerName);
            this.customerDDl.splice(0, 0, { 'id': '', 'description': '' });

            this.unitNoDDl = _.clone(response.unitNo);
            this.unitNoDDl.splice(0, 0, { 'id': '', 'description': '' });
        });
    }

    protected filterPropertyForSearch() {
        if (!this.propertyDDl) {
            return;
        }
        // get the search keyword
        let search = this.propertyFilterCtrlForSearch.value;
        if (!search) {
            this.filteredPropertyForSearch.next(this.propertyDDl.slice());
            return;
        } else {
            search = search.toLowerCase();
        }
        // filter the banks
        this.filteredPropertyForSearch.next(this.filterProFunForSearch(search));
    }

    filterProFunForSearch(search) {
        var list = this.propertyDDl.filter(prop => prop.addressPremises.toLowerCase().indexOf(search) > -1);
        return list;
    }

    // Customer filter
    protected filterCustomerForSearch() {
        if (!this.customerDDl) {
            return;
        }
        // get the search keyword
        let search = this.customerFilterCtrlForSearch.value;
        if (!search) {
            this.filteredCustomerForSearch.next(this.customerDDl.slice());
            return;
        } else {
            search = search.toLowerCase();
        }
        // filter the banks
        this.filteredCustomerForSearch.next(this.filterCusFunForSearch(search));
    }

    filterCusFunForSearch(search) {
        var list = this.customerDDl.filter(prop => prop.description.toLowerCase().indexOf(search) > -1);
        return list;
    }

    // UnitNo filter
    protected filterUnitNoForSearch() {
        if (!this.unitNoDDl) {
            return;
        }
        // get the search keyword
        let search = this.unitNoFilterCtrlForSearch.value;
        if (!search) {
            this.filteredUnitNoForSearch.next(this.unitNoDDl.slice());
            return;
        } else {
            search = search.toLowerCase();
        }
        // filter the banks
        this.filteredUnitNoForSearch.next(this.filterUnitFunForSearch(search));
    }

    filterUnitFunForSearch(search) {
        var list = this.unitNoDDl.filter(prop => prop.description.toLowerCase().indexOf(search) > -1);
        return list;
    }

}
