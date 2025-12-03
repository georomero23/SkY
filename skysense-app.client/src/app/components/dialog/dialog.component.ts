import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ButtonCloseDirective, ButtonDirective, ModalModule } from '@coreui/angular';
import { YesNoDialogService, YesNoDialogData } from '@services/Front/yes-no-dialog.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-yes-no-dialog',
  imports: [CommonModule, ModalModule, ButtonDirective, ButtonCloseDirective],
  templateUrl: './dialog.component.html',
  styleUrls: ['./dialog.component.scss']
})
export class YesNoDialogComponent implements OnInit {
  visible = false;
  message = '';
  title = '';
  private response$?: Subject<boolean>;

  constructor(private dialogService: YesNoDialogService) {}

  ngOnInit() {
    this.dialogService.dialog$.subscribe((data: YesNoDialogData) => {
      this.message = data.message;
      this.title = data.title;
      this.response$ = data.response$;
      this.visible = true;
    });
  }

  onYes() {
    this.response$?.next(true);
    this.response$?.complete();
    this.visible = false;
  }

  onNo() {
    this.response$?.next(false);
    this.response$?.complete();
    this.visible = false;
  }
}
