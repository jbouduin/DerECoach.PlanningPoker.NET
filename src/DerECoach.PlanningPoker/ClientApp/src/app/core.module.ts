import { NgModule, Optional, SkipSelf, ModuleWithProviders } from '@angular/core';

import { GameService } from './core/services/game.service';

@NgModule({
  imports: [
  ],
  declarations: [
  ],
  entryComponents: [    
  ]
})
export class GameModule {
  constructor(@Optional() @SkipSelf() parentModule: GameModule) {
    if (parentModule)
      throw Error('This module is already loaded on AppModule!');
  }

  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: GameModule,
      providers: [
        GameService
      ]
    }
  }
}
