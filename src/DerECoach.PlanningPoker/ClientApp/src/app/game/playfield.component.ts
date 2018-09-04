import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';
import { Participant } from '../core/domain/participant';
import { PokerCard } from '../core/domain/poker-card';
import { EGameStatus } from '../core/domain/enum-game-status';

@Component({
  selector: 'playfield',
  templateUrl: './playfield.component.html',
  styleUrls: ['./playfield.component.css']
})

export class PlayfieldComponent {

  private arrayOfCards: Array<PokerCard>;

  get teamName(): string {
    return this.gameService.teamName;
  }

  get participants(): Array<Participant> {
    return this.gameService.participants;
  }

  get developerScreenNames(): Array<string> {

    var result = new Array<string>();
    for (let p of this.gameService.participants.filter(p => p.isScrumMaster == false)) {
      if (this.gameService.isMe(p)) {
        result.push(p.screenName + " (me)");
      }
      else {
        result.push(p.screenName);
      }
    }
    return result;
  }

  get scrumMaster(): Participant {
    return this.gameService.scrumMaster;
  }

  get scrumMasterScreenName(): string {
    if (this.gameService.isScrumMasterMe()) {
      return this.gameService.scrumMaster.screenName + " (me)";
    }
    else {
      return this.gameService.scrumMaster.screenName
    }
  }

  get availableCards(): Array<PokerCard> {
    return this.arrayOfCards;
  }
  
  get isShowCards(): boolean {
    return this.gameService.gameStatus != EGameStatus.None &&
      this.gameService.gameStatus != EGameStatus.Revealed      
  }

  get isShowEstimations(): boolean {
    return this.gameService.gameStatus == EGameStatus.Started ||
      this.gameService.gameStatus == EGameStatus.EstimationGiven ||
      this.gameService.gameStatus == EGameStatus.Revealed;
  }

  get estimations(): Array<Participant> {
    var result = new Array<Participant>();
    this.gameService.participants
      .filter(p => p.lastEstimation != "")
      .forEach(fe => {
        var estimation = new Participant();
        estimation.screenName = fe.screenName;
        if (this.gameService.gameStatus == EGameStatus.Revealed) {
          estimation.lastEstimation = fe.lastEstimation;
        }
        else {
          if (fe.uuid == this.gameService.me.uuid)
            estimation.lastEstimation = fe.lastEstimation;
          else
            estimation.lastEstimation = "";
        }
        result.push(estimation);
      });
    return result;
  }

  get showEstimationValues(): boolean {
    return this.gameService.gameStatus == EGameStatus.Revealed;
  }

  estimate(card: string) {
    this.gameService.estimate(card);
  }

  constructor(private gameService: GameService) {

    this.gameService = gameService;
    this.fillArrayOfCards();
  }

  fillArrayOfCards(): void {
    this.arrayOfCards = new Array<PokerCard>(14);
    var card = new PokerCard();
    card.label = "0";
    this.arrayOfCards[0] = card;

    card = new PokerCard();
    card.label = "½";
    this.arrayOfCards[1] = card;

    card = new PokerCard();
    card.label = "1";
    this.arrayOfCards[2] = card;

    card = new PokerCard();
    card.label = "2";
    this.arrayOfCards[3] = card;

    card = new PokerCard();
    card.label = "3";
    this.arrayOfCards[4] = card;

    card = new PokerCard();
    card.label = "5";
    this.arrayOfCards[5] = card;

    card = new PokerCard();
    card.label = "8";
    this.arrayOfCards[6] = card;

    card = new PokerCard();
    card.label = "13";
    this.arrayOfCards[7] = card;

    card = new PokerCard();
    card.label = "20";
    this.arrayOfCards[8] = card;

    card = new PokerCard();
    card.label = "40";
    this.arrayOfCards[9] = card;

    card = new PokerCard();
    card.label = "100";
    this.arrayOfCards[10] = card;

    card = new PokerCard();
    card.label = "∞";
    this.arrayOfCards[11] = card;

    card = new PokerCard();
    card.label = "?";
    this.arrayOfCards[12] = card;

    card = new PokerCard();
    card.label = "K";
    this.arrayOfCards[13] = card;
  }
}
