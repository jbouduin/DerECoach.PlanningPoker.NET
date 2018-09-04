import { Injectable, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { Game } from '../domain/game';
import { v4 as uuid } from 'uuid';
import { Participant } from '../domain/participant';
import { JoinRequest } from '../requests/join-request';
import { CreateRequest } from '../requests/create-request';
import { EstimateRequest } from '../requests/estimate-request';
import { EGameStatus } from '../domain/enum-game-status';
import * as signalR from '@aspnet/signalr';

@Injectable()
export class GameService {

  private uuid: string = uuid();
  private game: Game;
  private connection = new signalR.HubConnectionBuilder().withUrl("/game").build();

  public gameStatus: EGameStatus = EGameStatus.Started;

  
  get isInGame(): boolean {
    return this.game != null;
  }

  get teamName(): string {    
    return this.game.teamName;
  }
  
  get participants(): Array<Participant>{
    var result = this.game.allParticipants;    
    return result;
  }

  get scrumMaster(): Participant {
    return this.game.allParticipants.filter(p => p.isScrumMaster == true)[0];
    
  }

  get me(): Participant {
    return this.game.allParticipants.filter(p => p.uuid == this.uuid)[0];
  }

  get estimations(): Array<Participant> {

    var result = new Array<Participant>();
    this.game.allParticipants.filter(p => p.lastEstimation != "").forEach(function (fe) { console.debug("building estimations array", result); result.push(fe); });
    return result;
  }

  isMe(participant: Participant): boolean {    
    return this.uuid == participant.uuid;
  }

  isScrumMasterMe(): boolean {
    return this.me.uuid == this.scrumMaster.uuid;
  }

  constructor(private router: Router) {
    
    this.connection.start().catch(err => console.log(err));
    this.connection.on("joined", participant => this.onjoined(participant));
    this.connection.on("estimated", participant => this.onestimated(participant));
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
      .then(response => { this.game = response; this.router.navigate(['/playfield']); })
      .catch(error => { console.error(error); result = error.ToString; });
    
    return result;
  }

  create(request: CreateRequest): string {

    var result: string = null;

    request.uuid = this.uuid;
    this.connection.invoke<Game>("create", request)
      .then(response => { this.game = response; this.router.navigate(['/playfield']); })
      .catch(error => { console.error(error); result = error.ToString; });
    return result;
  }

  estimate(card: string) {
    var request = new EstimateRequest();
    request.card = card;
    request.uuid = this.uuid;
    request.teamName = this.teamName;
    
    this.connection.invoke("estimate", request)
      .then(() => {
        this.me.lastEstimation = card;
        this.setGameStatusAfterEstimation();
      })
      .catch(error => { console.error(error);  });
  }

  onjoined(participant: Participant): void {
    console.debug("joined", participant);
    this.game.allParticipants.push(participant);
  }

  onestimated(participant: Participant): void {
    console.debug("estimated", participant);
    var theoneweneed = this.game.allParticipants.filter(p => p.uuid == participant.uuid)[0];
    console.debug(theoneweneed);
    this.game.allParticipants.filter(p => p.uuid == participant.uuid)[0].lastEstimation = participant.lastEstimation;
    this.setGameStatusAfterEstimation();
  }

  // set gamestatus after estimation
  setGameStatusAfterEstimation(): void {
    if (this.game.allParticipants.filter(p => p.lastEstimation == "").length == 0) {
      this.gameStatus = EGameStatus.Revealed;
    }
    else {
      this.gameStatus = EGameStatus.EstimationGiven;
    }
  }
}
