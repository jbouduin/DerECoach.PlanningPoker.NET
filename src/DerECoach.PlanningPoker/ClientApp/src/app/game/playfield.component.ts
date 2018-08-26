import { Component } from '@angular/core';

import { GameService } from '../core/services/game.service';
import { Participant } from '../core/domain/participant';
import { PokerCard } from '../core/domain/PokerCard';

@Component({
  selector: 'playfield',
  templateUrl: './playfield.component.html',
  styleUrls: ['./playfield.component.css']
})

export class PlayfieldComponent {

  private _gameService: GameService;
  private _availableCards: Array<PokerCard>;

  get teamName(): string {
    return this._gameService.teamName;
  }

  get participants(): Array<Participant> {
    return this._gameService.participants;
  }

  get participantScreenNames(): Array<string> {

    var result = new Array<string>();
    for (let p of this._gameService.participants) {
      if (this._gameService.isMe(p)) {
        result.push(p.screenName + " (me)");
      }
      else {
        result.push(p.screenName);
      }
    }
    return result;
  }

  get scrumMaster(): Participant {
    return this._gameService.scrumMaster;
  }

  get scrumMasterScreenName(): string {
    if (this._gameService.isScrumMasterMe()) {
      return this._gameService.scrumMaster.screenName + " (me)";
    }
    else {
      return this._gameService.scrumMaster.screenName
    }
  }

  get availableCards(): Array<PokerCard> {
    return this._availableCards;
  }

  constructor(gameService: GameService) {

    this._gameService = gameService;
    this._availableCards = new Array<PokerCard>(14);
    var card = new PokerCard();
    card.label = "0";
    this._availableCards[0] = card;

    card = new PokerCard();
    card.label = "½";
    this._availableCards[1] = card;

    card = new PokerCard();
    card.label = "1";
    this._availableCards[2] = card;

    card = new PokerCard();
    card.label = "2";
    this._availableCards[3] = card;

    card = new PokerCard();
    card.label = "3";
    this._availableCards[4] = card;

    card = new PokerCard();
    card.label = "5";
    this._availableCards[5] = card;

    card = new PokerCard();
    card.label = "8";
    this._availableCards[6] = card;

    card = new PokerCard();
    card.label = "13";
    this._availableCards[7] = card;

    card = new PokerCard();
    card.label = "20";
    this._availableCards[8] = card;

    card = new PokerCard();
    card.label = "40";
    this._availableCards[9] = card;

    card = new PokerCard();
    card.label = "100";
    this._availableCards[10] = card;

    card = new PokerCard();
    card.label = "∞";
    this._availableCards[11] = card;

    card = new PokerCard();
    card.label = "?";
    this._availableCards[12] = card;

    card = new PokerCard();
    card.label = "K";
    this._availableCards[13] = card;    
  }
}
