import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';
import { JoinRequest } from '../core/requests/join-request';


@Component({
  selector: 'join-component',
  templateUrl: './join.component.html'
})
export class JoinComponent {

  
  private teamNameLabel = "Team name";
  private screenNameLabel = "Participant name";
  private sendButtonCaption = "Join team";

  private errorText: string;
  private screenName: string;
  private teamName: string;
  private request = new JoinRequest();
  
  constructor(private gameService: GameService) {
  }

  join(): void {        
    this.errorText = this.gameService.join(this.request);
  }
}
