import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SavedJobNoteModel } from '../../models/saved-job-note.model';
import { JobService } from '../../services/job.service';

@Component({
  selector: 'lib-saved-job-note',
  templateUrl: './saved-job-note.component.html',
  styleUrls: ['./saved-job-note.component.scss']
})
export class SavedJobNoteComponent implements OnInit {

  note: string;

  constructor(
    private dialogRef: MatDialogRef<SavedJobNoteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {
      savedJobNoteModel: SavedJobNoteModel
    },
    private jobService: JobService) { }

  ngOnInit(): void {
    this.note = this.data.savedJobNoteModel.note;
  }

  close(): void {
    this.dialogRef.close();
  }

  saveNote(event: Event): void {
    event.preventDefault();
    if (this.note !== this.data.savedJobNoteModel.note) {
      this.jobService.saveJobNote(this.data.savedJobNoteModel)
        .subscribe(() => {
          this.dialogRef.close({ updatedNote: this.data.savedJobNoteModel.note });
        });
    } else {
      this.close();
    }
  }

  deleteNote(event: Event): void {
    this.data.savedJobNoteModel.note = '';
    this.saveNote(event);
  }
}
