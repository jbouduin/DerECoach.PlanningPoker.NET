import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JoinRequest } from '../core/join-request';


@Component({
  selector: 'join-component',
  templateUrl: './join.component.html'
})

export class JoinComponent {
  private _http: HttpClient;
  private _baseUrl: string;
  private _teamNameLabel = "Team";
  private _screenNameLabel = "Name";
  private _sendButtonCaption = "Join";

  private _errorText: string;
  private _screenName: string;
  private _teamName: string;
  private _request = new JoinRequest();

  public screenName = "";

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._baseUrl = baseUrl;
  }

  joinTeam(): void {
    
    this
      ._http.post(this._baseUrl + 'api/participant/join', this._request)
      .subscribe(
      result => { this._errorText = result.toString(); this._errorText = null; },
      error => { console.error(error); this._errorText = error; });
    
    
  }
}
