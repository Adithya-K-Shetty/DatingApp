import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  bsModelRef?: BsModalRef<ConfirmDialogComponent>;
  constructor(private modalService: BsModalService) {}
  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this ?',
    btnOkText = 'Ok',
    btnCancleText = 'Cancel'
  ): Observable<boolean> {
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancleText,
      },
    };
    this.bsModelRef = this.modalService.show(ConfirmDialogComponent, config);
    return this.bsModelRef.onHidden!.pipe(
      map(() => {
        return this.bsModelRef!.content!.result;
      })
    );
  }
}
