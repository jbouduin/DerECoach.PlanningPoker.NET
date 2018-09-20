import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';
import { GameService } from '../core/services/game.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {  

  constructor(private router: Router,
    private gameService: GameService) {
  }

  ngOnInit() {
    if (this.gameService.isInGame)
      this.router.navigate(['/playfield']);    
  }

}
