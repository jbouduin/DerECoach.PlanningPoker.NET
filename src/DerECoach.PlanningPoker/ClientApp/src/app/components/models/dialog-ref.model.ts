import { ComponentRef } from '@angular/core';
import { DialogComponent } from '../dialog/dialog.component';
import { DialogContentRefModel } from './dialog-content-ref.model';
import { BackdropComponent } from '../backdrop/backdrop.component';


export class DialogRefModel {

  // The result is responsible for the Dialog response on the service
  public result: Promise<any>;

  // Resolve will be invoked when the dialog is closed
  private _resolve: (result?: any) => void;

  // Reject will be invoked when the user click out of the Dialog or press ESC
  private _reject: (reason?: any) => void;

  get instance(): any {
    if (this._contentRef.componentRef) {
      return this._contentRef.componentRef.instance;
    }
  }

  constructor(
    private _dialogComponentRef: ComponentRef<DialogComponent>,
    private _backdropComponentRef: ComponentRef<BackdropComponent>,
    private _contentRef: DialogContentRefModel,
    private _beforeDismiss?: Function) {

    _dialogComponentRef.instance.dismiss.subscribe((reason: any) => this.dismiss(reason));

    this.result = new Promise((resolve, reject) => {
      this._resolve = resolve;
      this._reject = reject;
    });
    this.result.then(null, () => { });
  }

  private _clear() {
    const el = this._dialogComponentRef.location.nativeElement;
    el.parentNode.removeChild(el);
    this._dialogComponentRef.destroy();

    if (this._backdropComponentRef) {
      const elBd = this._backdropComponentRef.location.nativeElement;
      elBd.parentNode.removeChild(elBd);
      this._backdropComponentRef.destroy();
    }

    if (this._contentRef && this._contentRef.viewRef) {
      this._contentRef.viewRef.destroy();
    }

    this._dialogComponentRef = null;
    this._contentRef = null;
  }

  public dismiss = (reason: String) => {
    if (this._dialogComponentRef) {
      if (!this._beforeDismiss || this._beforeDismiss() !== false) {
        this._resolve(reason);
        this._clear();
      }
    }
  }

  public close = (result?: any) => {
    if (this._dialogComponentRef) {
      this._resolve(result);
      this._clear();
    }
  }

}
