import { Component } from '@angular/core';
import { GameService } from '../core/services/game.service';

@Component({
  selector: 'join-component',
  templateUrl: './join.component.html'
})
export class JoinComponent {

  
  get teamNameLabel(): string {
    return "Team name";
  }

  get screenNameLabel(): string {
    return "Participant name";
  }

  get sendButtonCaption(): string {
    return "Join team";
  }

  public errorText: string;
  public screenNameText: string;
  public teamNameText: string;
  
  
  constructor(private gameService: GameService) {
  }

  join(): void {
    this.errorText = this.gameService.join(this.teamNameText, this.screenNameText);
  }
}
