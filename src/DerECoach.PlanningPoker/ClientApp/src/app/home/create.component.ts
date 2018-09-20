import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';
import { CreateRequest } from '../core/requests/create-request';


@Component({
  selector: 'create-component',
  templateUrl: './create.component.html'
})

export class CreateComponent {

  private teamNameLabel = "Team name";
  private scrumMasterLabel = "Scrum master name";
  private sendButtonCaption = "Create team";

  private errorText: string;
  private scrumMaster: string;
  private teamName: string;
  private request = new CreateRequest();
  
  constructor(private gameService: GameService) {    
  }

  create(): void {
    this.errorText = this.gameService.create(this.request);    
  }  
}
