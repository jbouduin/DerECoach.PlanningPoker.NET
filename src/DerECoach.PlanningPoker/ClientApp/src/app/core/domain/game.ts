import { Participant } from './participant';
import { Estimation } from './estimation';

export class Game {
  teamName: string;  
  participants: Array<Participant>;
  estimations: Array<Estimation>;
}
