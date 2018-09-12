import { Component, OnInit } from '@angular/core';
import { DialogActiveModel } from '../models/dialog-active.model';
import { DialogOptionsModel } from '../models/dialog-options.model';
import { MessageModel } from '../models/message-model';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit {

  private message: string;
  constructor(private dialog: DialogActiveModel, private options: DialogOptionsModel<MessageModel>) {

  }

  ngOnInit() {
    this.message = this.options.data.message;
  }

  public onCloseClick() {
    console.log('close');
    this.dialog.close();
  }
}
