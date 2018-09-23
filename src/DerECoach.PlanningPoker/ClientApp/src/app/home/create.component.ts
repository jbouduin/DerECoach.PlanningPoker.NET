import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';

@Component({
  selector: 'create-component',
  templateUrl: './create.component.html'
})

export class CreateComponent {

  get teamNameLabel(): string {
    return "Team name";
  }

  get scrumMasterLabel(): string {
    return "Scrum master name";
  }

  get sendButtonCaption(): string {
    return "Create team";
  }

  public errorText: string;
  public scrumMasterText: string;
  public teamNameText: string;
  
  
  constructor(private gameService: GameService) {    
  }

  create(): void {
    this.errorText = this.gameService.create(this.teamNameText, this.scrumMasterText);    
  }  
}
