import { Component } from '@angular/core';
import { GameService } from '../core/services/game.service';
import { Participant } from '../core/domain/participant';
import { Card } from '../core/domain/card';
import { EGameStatus } from '../core/domain/enum-game-status';
import { EstimationModel } from '../core/domain/estimation-model';

@Component({
  selector: 'playfield',
  templateUrl: './playfield.component.html',
  styleUrls: ['./playfield.component.css']
})

export class PlayfieldComponent {

  get teamName(): string {
    return this.gameService.teamName;
  }

  get participants(): Array<Participant> {
    return this.gameService.participants;
  }

  get developerScreenNames(): Array<string> {

    var result = new Array<string>();
    for (let p of this.gameService.participants.filter(p => p.scrumMaster == false)) {
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

  get availableCards(): Array<Card> {
    return this.gameService.cards;
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

  get estimations(): Array<EstimationModel> {
    var result = new Array<EstimationModel>();
    this.gameService.estimations
      .sort((p1, p2) => {
        if (this.gameService.gameStatus == EGameStatus.Revealed) {
          if (p1.index > p2.index) {
            return 1;
          }
          if (p1.index < p2.index) {
            return -1;
          }
          return 0;
        }
      })
      .forEach(fe => {
        var model = new EstimationModel();
        var participant = this.gameService.participants.filter(p => p.uuid == fe.uuid)[0];
        model.screenName = participant.screenName;
        if (this.gameService.gameStatus == EGameStatus.Revealed || fe.uuid == this.gameService.me.uuid) {
          var card = this.gameService.cardByIndex(fe.index);
          model.label = card.label;
        }
        result.push(model);
      });
    return result;
  }

  get showEstimationValues(): boolean {
    return this.gameService.gameStatus == EGameStatus.Revealed;
  }

  estimate(index: number) {
    this.gameService.estimate(index);
  }
  
  constructor(private gameService: GameService) {
    this.gameService = gameService;  
  }
}
