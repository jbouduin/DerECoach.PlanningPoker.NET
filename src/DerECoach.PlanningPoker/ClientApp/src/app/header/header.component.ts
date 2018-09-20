import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})

export class HeaderComponent {

  get showLeaveButton(): boolean {
    return this.gameService.isInGame;
  }

  get leaveButtonLabel(): string {
    if (this.gameService.isScrumMasterMe()) {
      return "End game";
    }
    else {
      return "Leave";
    }
  }

  leave() {

    let messageString: string = this.gameService.isScrumMasterMe() ?
      "Are you sure you want to end the game?" :
      "Are you sure you want to leave?";
    let confirmed = confirm(messageString);    
    if (confirmed == true) {
      let result = this.gameService.isScrumMasterMe() ?
        this.gameService.end() :
        this.gameService.leave();
      if (result != null) {
        alert(result);
      }
    }
  }

  constructor(private gameService: GameService) {
  }
}
