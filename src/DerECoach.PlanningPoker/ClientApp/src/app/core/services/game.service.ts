import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { v4 as uuid } from 'uuid';
import { ToasterService, Toast } from 'angular2-toaster';

import { Game } from '../domain/game';
import { BaseResult, Result } from '../responses/result';
import { Participant } from '../domain/participant';
import { JoinRequest } from '../requests/join-request';
import { CreateRequest } from '../requests/create-request';
import { EstimateRequest } from '../requests/estimate-request';
import { EndRequest } from '../requests/end-request';
import { GameStatus } from '../domain/game-status.enum';
import * as signalR from '@aspnet/signalr';
import { Card } from '../domain/card';
import { JoinResponse } from '../responses/join-response';
import { CreateResponse } from '../responses/create-response';
import { Estimation } from '../domain/estimation';
import { LeaveRequest } from '../requests/leave-request';

@Injectable()
export class GameService {

  private readonly gameKey: string = "current_game";
  private readonly uuidKey: string = "current_uuid";

  private uuid: string = uuid();
  private game: Game;
  private connection: signalR.HubConnection;
  
  public gameStatus: GameStatus = GameStatus.None;
  public cards: Array<Card>;

  get isInGame(): boolean {
    return this.game != null;
  }

  get teamName(): string {    
    return this.game.teamName;
  }
  
  get participants(): Array<Participant>{
    return this.game.participants;
  }

  get scrumMaster(): Participant {
    return this.game.participants.filter(p => p.scrumMaster == true)[0];    
  }

  get me(): Participant {
    return this.game.participants.filter(p => p.uuid == this.uuid)[0];
  }

  get estimations(): Array<Estimation> {
    return this.game.estimations;
  }

  isMe(participant: Participant): boolean {    
    return this.uuid == participant.uuid;
  }

  isScrumMasterMe(): boolean {
    return this.me.uuid == this.scrumMaster.uuid;
  }

  cardByIndex(index: number): Card {
    return this.cards.filter(card => card.index == index)[0];
  }

  validateGame(): Boolean {

    return false;
  }

  constructor(private router: Router,
    private toasterService: ToasterService) {
    console.debug("in gameservice constructor");
    this.connection = new signalR.HubConnectionBuilder().withUrl("/game").build();
    
    this.connection.on("joined", message => this.onjoined(message));
    this.connection.on("rejoined", message => this.onrejoined(message));
    this.connection.on("estimated", message => this.onestimated(message));
    this.connection.on("started", () => this.onstarted());
    this.connection.on("left", message => this.onleft(message));
    this.connection.on("ended", () => this.onended());
    this.connection.on("disconnected", message => this.ondisconnected(message));
    
    if (this.game != null)
      return;

    let loadedGame: any = localStorage.getItem(this.gameKey);
    this.uuid = localStorage.getItem(this.uuidKey);

    if (loadedGame == null) {
      console.debug("no game in storage");
      return;
    }

    if (this.uuid == null) {
      console.debug("no uuid in storage");
      this.uuid = uuid();
      localStorage.setItem(this.uuidKey, this.uuid);
      return;
    }
        
    this.game = JSON.parse(loadedGame);
    if (this.me == null) {
      console.debug("not my game");
      this.game = null;
      this.uuid = uuid();
      localStorage.setItem(this.uuidKey, this.uuid);
    }
    console.debug("found my game");
  }

  async initConnection(): Promise<void>{
    await this.connection.start()
      .then(() => {
        console.log("hub connected");
        this.infoToast("Connected", "You are now connected");
      })
      .catch(error => console.log(error));
  }

  join(teamName: string, screenName: string): void {        
    let request = new JoinRequest();
    request.screenName = screenName;
    request.teamName = teamName;
    request.uuid = this.uuid;    
    this.connection.invoke<Result<JoinResponse>>("join", request)
      .then(response => {
        console.debug(response);
        if (response.succeeded == true) {
          this.game = response.value.game;
          this.cards = response.value.cards;
          this.router.navigate(['/playfield']);
          this.saveGame();
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => {
        console.error(error);
        this.errorToast("Error", error);
      });
  }

  rejoin(): void {    
    let request = new JoinRequest();
    request.teamName = this.game.teamName;
    request.screenName = this.me.screenName;
    request.uuid = this.uuid;
    this.connection.invoke<Result<JoinResponse>>("rejoin", request)
      .then(response => {
        console.debug(response);
        if (response.succeeded == true) {
          if (response.value.game != null) {
            this.game = response.value.game;
            this.cards = response.value.cards;
            this.router.navigate(['/playfield']);
            this.saveGame();
          }
          else {
            this.errorToast("Error", "Unkown error");
          }
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => {
        console.error(error);
        this.errorToast("Error", error);
      });
  }

  create(teamName: string, scrumMaster: string): void {    
    let request = new CreateRequest();
    request.teamName = teamName;
    request.scrumMaster = scrumMaster;
    request.uuid = this.uuid;
    this.connection.invoke<Result<CreateResponse>>("create", request)
      .then(response => {
        console.debug(response);
        if (response.succeeded == true) {
          this.game = response.value.game;
          this.cards = response.value.cards;
          this.router.navigate(['/playfield']);
          this.saveGame();
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => {
        console.error(error);
        this.errorToast("Error", error);
      });
  }

  estimate(index: number): void {
    let request = new EstimateRequest();
    request.index = index;
    request.uuid = this.uuid;
    request.teamName = this.teamName;
    
    this.connection.invoke<BaseResult>("estimate", request)
      .then(response => {
        if (response.succeeded) {
          var estimation = new Estimation();
          estimation.uuid = this.uuid;
          estimation.index = index;
          this.upsertEstimation(estimation);
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => this.errorToast("Error", error));
  }

  startGame(): void {
    this.connection.invoke<BaseResult>("start", this.teamName)
      .then(response => {
        if (response.succeeded == true) {
          this.startEstimating();
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => this.errorToast("Error", error));    
  }

  leave(): void {    
    let request = new LeaveRequest();
    request.teamName = this.game.teamName;
    request.uuid = this.uuid;    

    this.connection.invoke<BaseResult>("leave", request)
      .then(response => {
        if (response.succeeded) {
          this.game = null;
          localStorage.removeItem(this.gameKey);
          this.router.navigate(['']);
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => {        
        this.errorToast("Error", error);
      });
  }

  end(): void {
    
    let request = new EndRequest();
    request.teamName = this.game.teamName;
    request.uuid = this.uuid;

    this.connection.invoke<BaseResult>("end", request)
      .then(response => {
        if (response.succeeded) {
          this.game = null;
          localStorage.removeItem(this.gameKey);
          this.router.navigate(['']);
        }
        else {
          this.errorToast("Error", response.message);
        }
      })
      .catch(error => {
        console.error(error);
        this.errorToast("Error", error);
      });
  }

  onjoined(message: Participant): void {
    console.debug("joined", message);
    this.infoToast("Joined", message.screenName + " joined");
    this.upsertParticipant(message);
  }

  ondisconnected(message: Participant): void {
    console.debug("disconnected", message);
    this.infoToast("Disconnected", message.screenName + " disconnected");
    this.upsertParticipant(message);
  }

  onrejoined(message: Participant): void {
    console.debug("rejoined", message);
    this.infoToast("Rejoined", message.screenName + " is back");
    this.upsertParticipant(message);
  }

  onleft(message: Participant): void {
    console.debug("left", message);
    this.infoToast("Left", message.screenName + " has left the game");
    var theOne = this.game.participants.filter(f => f.uuid == message.uuid)[0];
    let index = this.game.participants.indexOf(theOne)
    this.game.participants.splice(index, 1);
    this.saveGame();
  }
  
  onestimated(message: Estimation): void {
    console.debug("estimated", message);
    this.upsertEstimation(message);
  }

  onstarted(): void {
    this.startEstimating();
  }

  onended(): void {
    this.game = null;
    localStorage.removeItem(this.gameKey);
    alert("The Scrum Master has ended the game");
    this.router.navigate(['']);
  }

  // set gamestatus after estimation
  setGameStatusAfterEstimation(): void {
    console.debug("setGameStatusAfterEstimation ", this.game.participants.filter(f => f.waiting == false).length, this.game.estimations.length);
    if (this.game.participants.filter(f => f.waiting == false).length == this.game.estimations.length) {
      this.gameStatus = GameStatus.Revealed;
    }
    
  }

  upsertEstimation(newEstimation: Estimation): void {
    var estimation = this.game.estimations.filter(f => f.uuid == newEstimation.uuid)[0];
    if (estimation == null) {
      this.game.estimations.push(newEstimation);
    }
    else {
      estimation.index = newEstimation.index;
    }
    this.setGameStatusAfterEstimation();
    this.saveGame();
  }

  startEstimating(): void {
    this.game.estimations = new Array<Estimation>();
    this.gameStatus = GameStatus.Started;
    this.participants.forEach(fe => {
      fe.waiting = false;
    });
    this.saveGame();
  }

  upsertParticipant(participant: Participant) {
    var theOne = this.game.participants.filter(f => f.uuid == participant.uuid)[0];
    if (theOne == null) {
      this.game.participants.push(participant);
    }
    else {
      let index = this.game.participants.indexOf(theOne)
      this.game.participants[index] = participant;
    }
    this.saveGame();
  }

  saveGame(): void {
    localStorage.setItem(this.gameKey, JSON.stringify(this.game));  
  }

  infoToast(titleText: string, message: string): void {
    let toast: Toast = {
      type: "info",
      title: titleText,
      body: message
    }
    this.toasterService.popAsync(toast)
  }

  errorToast(titleText: string, message: string): void {
    let toast: Toast = {
      type: "error",
      title: titleText,
      body: message
    }
    this.toasterService.popAsync(toast)
  }
}
