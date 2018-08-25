import { Injectable, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import { Game } from '../domain/game';
import { Participant } from '../domain/participant';
import { JoinRequest } from '../requests/join-request';
import { CreateRequest } from '../requests/create-request';

@Injectable()
export class GameService {

  private _http: HttpClient;
  private _baseUrl: string;
  private _router: Router;

  private _game: Game;
  

  get isInGame(): boolean {
    return this._game != null;
  }

  get teamName(): string {    
    return this._game.teamName;
  }
  
  get participants(): Array<Participant>{
    return this._game.allParticipants;
  }

  get scrumMaster(): Participant {
    return this._game.scrumMaster;
  }

  constructor(http: HttpClient, router: Router, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._router = router;
    this._baseUrl = baseUrl;

    console.debug("creating gameservice");
    if (this._game != null)
      return;

    var _loadedGame: any = localStorage.getItem("current_game");
    if (_loadedGame == null) {
      console.debug("no game in storage");
      return;
    }

    this._game = JSON.parse(_loadedGame);
  }
  
  join(request: JoinRequest): string {

    var result: string = null;
    this
      ._http.post<Game>(this._baseUrl + 'api/game/join', request)
      .subscribe(
        response => {
          this._game = response;
          console.debug(this._game);
          this._router.navigate(['/playfield']);
        },
        error => {
          console.error(error);
          result = error.ToString;
        });
    
    return result;
  }

  create(request: CreateRequest): string {
    var result: string = null;
    this
      ._http.post<Game>(this._baseUrl + 'api/game/create', request)
      .subscribe(
        response => { this._game = response; this._router.navigate(['/playfield']); },
      error => { console.error(error); result = error.ToString; });
    return result;
  }
  
}
