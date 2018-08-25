import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';
import { JoinRequest } from '../core/requests/join-request';


@Component({
  selector: 'join-component',
  templateUrl: './join.component.html'
})
export class JoinComponent {

  private _gameService: GameService;  
  private _teamNameLabel = "Team name";
  private _screenNameLabel = "Participant name";
  private _sendButtonCaption = "Join team";

  private _errorText: string;
  private _screenName: string;
  private _teamName: string;
  private _request = new JoinRequest();
  
  constructor(gameService: GameService) {
    this._gameService = gameService;  
  }

  join(): void {        
    this._errorText = this._gameService.join(this._request);
  }
}
