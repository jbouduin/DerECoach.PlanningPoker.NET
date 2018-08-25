import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateRequest } from '../core/requests/create-request';


@Component({
  selector: 'create-component',
  templateUrl: './create.component.html'
})

export class CreateComponent {
  private _http: HttpClient;
  private _baseUrl: string;
  private _teamNameLabel = "Team name";
  private _scrumMasterLabel = "Scrum master name";
  private _sendButtonCaption = "Create team";

  private _errorText: string;
  private _scrumMaster: string;
  private _teamName: string;
  private _request = new CreateRequest();
  
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._baseUrl = baseUrl;
  }

  createTeam(): void {
    
    this
      ._http.post(this._baseUrl + 'api/game/create', this._request)
      .subscribe(
      result => { this._errorText = result.toString(); this._errorText = null; },
      error => { console.error(error); this._errorText = error; });
    
    
  }
}
