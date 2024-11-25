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
import { RegistrationStatusComponent } from './registration-status.component';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { AuthenticationGuard } from 'app/core/authentication/authentication.guard';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
const routes = [
  {
    path: 'client-portal',
    component: RegistrationStatusComponent,
    canActivate: [AuthenticationGuard]
  }
];

@NgModule({
  declarations: [
    RegistrationStatusComponent
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
    NgxDatatableModule,
    NgxMatSelectSearchModule
  ]
})
export class RegistrationStatusModule {
}
