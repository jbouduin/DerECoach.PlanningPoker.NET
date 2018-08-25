import { Component, OnInit } from '@angular/core';
import { Game } from '../core/game';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit{

  private _game: Game;

  showJoin(): boolean {
    return this._game == null
  }

  ngOnInit() {
    var _loadedGame: any = localStorage.getItem("current_game");
    if (_loadedGame == null) {
      // show join game frame
      return;
    }
    
    this._game = JSON.parse(_loadedGame);
    if (this._game == null) {
      // show join game frame
      return;
    }

    // route to game
  }
}
