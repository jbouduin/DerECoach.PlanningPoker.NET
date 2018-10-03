import { Component, OnInit, AfterViewInit } from '@angular/core';
import { GameService } from '../core/services/game.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements AfterViewInit {  

  constructor(private gameService: GameService) {
  }

  ngAfterViewInit() {
    console.debug("afterviewinit");
    this.gameService.initConnection().then(() => {
      if (this.gameService.isInGame) {
        let tryReenter = confirm("Do you want to rejoin the team '" + this.gameService.teamName + "' as + '" + this.gameService.me.screenName + "'? ");
        if (tryReenter == true) {
          this.gameService.rejoin();
        }
      }
      else {
        this.gameService.leave();
      }
    });
  }

}
