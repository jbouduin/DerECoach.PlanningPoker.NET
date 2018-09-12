import { NgModule, Optional, SkipSelf, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DialogComponent } from './components/dialog/dialog.component';
import { BackdropComponent } from './components/backdrop/backdrop.component';
import { MessageComponent } from './components/message/message.component';

import { DialogService } from './core/services/dialog.service';
import { GameService } from './core/services/game.service';
import { MessageService } from './core/services/message.service';

const components = [
  DialogComponent,
  BackdropComponent
];
@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    ...components
  ],
  entryComponents: [
    ...components
  ]
})
export class DialogModule {
  constructor(@Optional() @SkipSelf() parentModule: DialogModule) {
    if (parentModule)
      throw Error('This module is already loaded on AppModule!');
  }

  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: DialogModule,
      providers: [
        DialogService
      ]
    }
  }
}

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


@NgModule({
  imports: [
    CommonModule,
    DialogModule.forRoot()
  ],
  declarations: [
    MessageComponent
  ],
  entryComponents: [
    MessageComponent
  ]
})
export class MessageModule {

  constructor(@Optional() @SkipSelf() parentModule: MessageModule) {
    if (parentModule) {
      throw Error('Core Module is already loaded on AppModule!');
    }
  }

  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: MessageModule,
      providers: [MessageService]
    };
  }

}
