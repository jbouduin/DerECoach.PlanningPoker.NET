import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ToasterModule } from 'angular2-toaster';
import { GameModule } from './core.module';

import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { HomeComponent } from './home/home.component';
import { JoinComponent } from './home/join.component';
import { CreateComponent } from './home/create.component';
import { PlayfieldComponent } from './game/playfield.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
    JoinComponent,
    CreateComponent,
    PlayfieldComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    GameModule.forRoot(),
    ToasterModule.forRoot(),
    HttpClientModule,
    FormsModule,    
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'playfield', component: PlayfieldComponent }      
    ])
  ],
  providers: [    
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
