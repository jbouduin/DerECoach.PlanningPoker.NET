import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GameService } from '../core/services/game.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {  

  constructor(public gameService: GameService,
  public router: Router) { }

  ngOnInit() {
    if (this.gameService.IsInGame)
      this.router.navigate(['/playfield']);;
    
  }
}
