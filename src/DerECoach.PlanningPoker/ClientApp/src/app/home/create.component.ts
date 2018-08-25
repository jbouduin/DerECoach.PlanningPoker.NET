import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';
import { CreateRequest } from '../core/requests/create-request';


@Component({
  selector: 'create-component',
  templateUrl: './create.component.html'
})

export class CreateComponent {

  private _gameService: GameService;

  private _teamNameLabel = "Team name";
  private _scrumMasterLabel = "Scrum master name";
  private _sendButtonCaption = "Create team";

  private _errorText: string;
  private _scrumMaster: string;
  private _teamName: string;
  private _request = new CreateRequest();
  
  constructor(gameService: GameService) {
    this._gameService = gameService;
  }

  create(): void {
    this._errorText = this._gameService.create(this._request);    
  }
}
