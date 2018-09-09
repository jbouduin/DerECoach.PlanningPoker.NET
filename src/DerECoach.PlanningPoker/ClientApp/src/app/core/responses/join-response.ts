import { Game } from "../domain/game";
import { Card } from "../domain/card";

export class JoinResponse{
  game: Game;
  cards: Array<Card>;
}
