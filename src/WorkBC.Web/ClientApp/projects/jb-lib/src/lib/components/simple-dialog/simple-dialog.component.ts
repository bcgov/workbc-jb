import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-simple-dialog',
  templateUrl: './simple-dialog.component.html',
  styleUrls: ['./simple-dialog.component.scss']
})
export class SimpleDialogComponent {

  constructor(
    private dialogRef: MatDialogRef<SimpleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { 
      title: string, btnLabel: string, content: string
    }) { }

  close() {
    this.dialogRef.close();
  }

  continue(event: Event) {
    event.preventDefault();
    this.dialogRef.close(true);
  }
}
