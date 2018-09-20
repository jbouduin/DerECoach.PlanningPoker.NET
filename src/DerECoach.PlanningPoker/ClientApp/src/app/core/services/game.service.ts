import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { v4 as uuid } from 'uuid';

import { Game } from '../domain/game';
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
  private connection = new signalR.HubConnectionBuilder().withUrl("/game").build();
  
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

  public cardByIndex(index: number): Card {
    return this.cards.filter(card => card.index == index)[0];
  }

  constructor(private router: Router) {
    
    this.connection.start().catch(err => console.log(err));
    this.connection.on("joined", message => this.onjoined(message));
    this.connection.on("estimated", message => this.onestimated(message));
    this.connection.on("started", () => this.onstarted());
    this.connection.on("left", message => this.onleft(message));
    this.connection.on("ended", () => this.onended());        

    if (this.game != null)
      return;

    var loadedGame: any = localStorage.getItem(this.gameKey);
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
    
  }
  
  join(request: JoinRequest): string {

    var result: string = null;
    request.uuid = this.uuid;    
    this.connection.invoke<JoinResponse>("join", request)
      .then(response => {
        console.debug(response);
        this.game = response.game;
        this.cards = response.cards;
        this.router.navigate(['/playfield']);
        this.saveGame();
      })
      .catch(error => { console.error(error); result = error.ToString; });
    
    return result;
  }
    
  create(request: CreateRequest): string {

    var result: string = null;

    request.uuid = this.uuid;
    this.connection.invoke<CreateResponse>("create", request)
      .then(response => {
        console.debug(response);
        this.game = response.game;
        this.cards = response.cards;
        this.router.navigate(['/playfield']);
        this.saveGame();
      })
      .catch(error => { console.error(error); result = error.ToString; });
    return result;
  }

  estimate(index: number) {
    var request = new EstimateRequest();
    request.index = index;
    request.uuid = this.uuid;
    request.teamName = this.teamName;
    
    this.connection.invoke("estimate", request)
      .then(() => {
        var estimation = new Estimation();
        estimation.uuid = this.uuid;
        estimation.index = index;
        this.setEstimation(estimation);
      })
      .catch(error => { console.error(error);  });
  }

  startGame() {
    this.connection.invoke("start", this.teamName)
      .then(() => {
        this.startEstimating();
      })
      .catch(error => { console.error(error); });    
  }

  leave(): string {
    var result: string = null;
    var request = new LeaveRequest();
    request.teamName = this.game.teamName;
    request.uuid = this.uuid;    

    this.connection.invoke("leave", request)
      .then(() => {        
        this.game = null;
        localStorage.removeItem(this.gameKey);
        this.router.navigate(['']);
      })
      .catch(error => {
        console.error(error);
        result = error.ToString;
      });

    return result;
  }

  end(): string {
    let result: string = null;
    let request = new EndRequest();
    request.teamName = this.game.teamName;
    request.uuid = this.uuid;

    this.connection.invoke("end", request)
      .then(() => {
        this.game = null;
        localStorage.removeItem(this.gameKey);
        this.router.navigate(['']);
      })
      .catch(error => {
        console.error(error);
        result = error.ToString;
      });

    return result;
  }

  onjoined(message: Participant): void {
    console.debug("joined", message);
    this.game.participants.push(message);
    this.saveGame();
  }

  onleft(message: Participant): void {
    console.debug("left", message);
    var theOne = this.game.participants.filter(f => f.uuid == message.uuid)[0];
    let index = this.game.participants.indexOf(theOne)
    this.game.participants.splice(index, 1);
    this.saveGame();
  }


  onestimated(message: Estimation): void {
    console.debug("estimated", message);
    this.setEstimation(message);
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

  setEstimation(newEstimation: Estimation): void {
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

  saveGame(): void {
    localStorage.setItem(this.gameKey, JSON.stringify(this.game));  
  }
}
