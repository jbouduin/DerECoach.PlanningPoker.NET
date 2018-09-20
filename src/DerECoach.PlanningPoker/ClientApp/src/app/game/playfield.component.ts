import { Component } from '@angular/core';
import { GameService } from '../core/services/game.service';
import { Participant } from '../core/domain/participant';
import { Card } from '../core/domain/card';
import { GameStatus } from '../core/domain/game-status.enum';
import { EstimationModel } from '../components/models/estimation.model';

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

    let result = new Array<string>();
    for (let p of this.gameService.participants.filter(p => p.scrumMaster == false)) {
      let screenName = p.screenName;

      if (this.gameService.isMe(p)) {
        screenName = screenName + " (me)";
      }
      if (p.waiting) {
        screenName = screenName + " (waiting)";
      }
      result.push(screenName);
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
    return this.gameService.gameStatus != GameStatus.None &&
      this.gameService.gameStatus != GameStatus.Revealed &&
      this.gameService.me.waiting == false;
  }

  get isShowEstimations(): boolean {
    return this.gameService.gameStatus == GameStatus.Started ||      
      this.gameService.gameStatus == GameStatus.Revealed ||
      this.gameService.me.waiting == true;
  }

  get canStartGame(): boolean {
    return this.gameService.isScrumMasterMe();
  }

  get estimations(): Array<EstimationModel> {
    var result = new Array<EstimationModel>();
    this.gameService.estimations
      .sort((p1, p2) => {
        if (this.gameService.gameStatus == GameStatus.Revealed) {
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
        if (this.gameService.gameStatus == GameStatus.Revealed || fe.uuid == this.gameService.me.uuid) {
          var card = this.gameService.cardByIndex(fe.index);
          model.label = card.label;
        }
        result.push(model);
      });
    return result;
  }

  get showEstimationValues(): boolean {
    return this.gameService.gameStatus == GameStatus.Revealed;
  }

  estimate(index: number) {
    this.gameService.estimate(index);
  }

  startGame() {
    this.gameService.startGame();
  }

  constructor(private gameService: GameService) {
    this.gameService = gameService;  
  }
}
