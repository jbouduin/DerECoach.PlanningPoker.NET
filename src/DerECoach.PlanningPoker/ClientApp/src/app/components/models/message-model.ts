import { MessageType } from "../../core/domain/message-type.enum";

export class MessageModel {
  constructor(public type: MessageType,
    public message: string) {

  }
}
