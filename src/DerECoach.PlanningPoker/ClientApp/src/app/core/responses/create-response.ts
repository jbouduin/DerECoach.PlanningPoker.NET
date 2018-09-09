import { Game } from "../domain/game";
import { Card } from "../domain/card";

export class CreateResponse{
  game: Game;
  cards: Array<Card>;
}
