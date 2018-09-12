import { Injectable } from '@angular/core';
import { DialogService } from './dialog.service';
import { MessageComponent } from '../../components/message/message.component';
import { DialogOptionsModel } from '../../components/models/dialog-options.model';
import { MessageModel } from '../../components/models/message-model';
import { MessageType } from '../domain/message-type.enum';

@Injectable()
export class MessageService {

  constructor(public dialog: DialogService) {

  }

  alert(message: string): Promise<any> {
    return this.open(MessageType.Alert, message);
  }

  open(type: MessageType, message: string): Promise<any> {
    const options = <DialogOptionsModel<MessageModel>>{
      data: <MessageModel>{
        message,
        type
      },
      top: '10vh'
    };

    return this.dialog.open<MessageModel>(MessageComponent, options).result;
  }

}
