import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent implements OnInit {

   message: string;
   acceptLabel:string;
   rejectLabel:string;

  constructor(public dialogRef: MatDialogRef<ConfirmationDialogComponent>,  @Inject(MAT_DIALOG_DATA) data, ) {
   
    this.message = data.message;
    this.acceptLabel=data.acceptBtn;
    this.rejectLabel=data.rejectBtn;
  }

  ngOnInit() {
   
  }

  save() {
    this.dialogRef.close("ok");
  }

  
  close() {
    this.dialogRef.close("cancel");
  }


}


