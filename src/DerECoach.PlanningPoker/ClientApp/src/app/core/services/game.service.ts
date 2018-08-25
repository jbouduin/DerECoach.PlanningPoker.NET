import { Injectable, OnInit } from '@angular/core';
import { Game } from '../domain/game';

@Injectable()
export class GameService implements OnInit {

  private _game: Game;

  get IsInGame(): boolean {
    return this._game != null;
  }

  ngOnInit() {
    var _loadedGame: any = localStorage.getItem("current_game");
    if (_loadedGame == null) {
      return;
    }

    this._game = JSON.parse(_loadedGame);
  }
}
