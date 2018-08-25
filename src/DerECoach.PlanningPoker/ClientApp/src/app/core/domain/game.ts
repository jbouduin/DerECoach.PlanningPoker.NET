import { Participant } from './participant';

export class Game {
  teamName: string;
  scrumMaster: Participant;  
  allParticipants: Array<Participant>
}
