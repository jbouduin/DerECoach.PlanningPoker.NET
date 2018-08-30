import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Game } from '../domain/game';
import { v4 as uuid } from 'uuid';
import { Participant } from '../domain/participant';
import { JoinRequest } from '../requests/join-request';
import { CreateRequest } from '../requests/create-request';
import * as signalR from '@aspnet/signalr';

@Injectable()
export class GameService {

  private uuid: string = uuid();
  private game: Game;
  private connection = new signalR.HubConnectionBuilder().withUrl("/game").build();
    
  get isInGame(): boolean {
    return this.game != null;
  }

  get teamName(): string {    
    return this.game.teamName;
  }
  
  get participants(): Array<Participant>{
    return this.game.allParticipants;
  }

  get scrumMaster(): Participant {
    return this.game.scrumMaster;
  }

  isMe(participant: Participant): boolean {    
    return this.uuid == participant.uuid;
  }

  isScrumMasterMe(): boolean {
    return this.isMe(this.game.scrumMaster);
  }

  constructor(private router: Router) {
    
    this.connection.start().catch(err => console.log(err));
    this.connection.on("joined", participant => this.onjoined(participant));
    console.debug("creating gameservice");
    console.debug(this.uuid);
    if (this.game != null)
      return;

    var _loadedGame: any = localStorage.getItem("current_game");
    if (_loadedGame == null) {
      console.debug("no game in storage");
      return;
    }

    this.game = JSON.parse(_loadedGame);
    
  }
  
  join(request: JoinRequest): string {

    var result: string = null;
    request.uuid = this.uuid;
    this.connection.invoke<Game>("join", request)
      .then(response => { this.game = response; console.debug(this.game); this.router.navigate(['/playfield']); })
      .catch(error => { console.error(error); result = error.ToString; });
    
    return result;
  }

  create(request: CreateRequest): string {

    var result: string = null;

    request.uuid = this.uuid;
    this.connection.invoke<Game>("create", request)
      .then(response => { this.game = response; console.debug(this.game); this.router.navigate(['/playfield']); })
      .catch(error => { console.error(error); result = error.ToString; });
    return result;
  }

  onjoined(participant: Participant): void {
    console.debug(participant);
    this.game.allParticipants.push(participant);
  }
}
