import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FuseSharedModule } from '@fuse/shared.module';
import { GridModule } from '@fuse/components';
import { CustomerTaxPasswordComponent } from './customer-tax-password.component';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { AuthenticationGuard } from 'app/core/authentication/authentication.guard';
const routes = [
  {
    path: 'taxlogin',
    component: CustomerTaxPasswordComponent,
    canActivate: [AuthenticationGuard]
  }
];

@NgModule({
  declarations: [
    CustomerTaxPasswordComponent
  ],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    MatTabsModule,
    MatButtonModule,
    MatRadioModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    FuseSharedModule,
    GridModule,
    NgxDatatableModule
  ]
})
export class TaxLoginModule {
}
