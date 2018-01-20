using System;
using System.Collections.Generic;

namespace GeneralsBot.TargetHeuristics {
    public class AttackHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Position)> desireds = new List<(int, Position)>();
            Console.WriteLine("RUNNING ATTACK HEURISTIC");
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is OccupiedTile tile && tile.Faction >= 0 && tile.Faction != playerIndex) {
                        Position position = new Position(x, y);
                        int desire = map.TotalArmies(playerIndex) - map.TotalArmies(tile.Faction);
                        if (map.UnitsAt(position) <= 3 && desire >= 0) desire *= 2;
                        else if (map.UnitsAt(position) <= 3) desire /= 2;
                        if (tile is GeneralTile) desire += 100000;
                        if (position.NaiveMoveDistance(map.GeneralPosition(playerIndex)) < 5) {
                            desire += (6 - position.NaiveMoveDistance(map.GeneralPosition(playerIndex))) * 250;
                        }
                        desireds.Add((desire, position));
                    }
                }
            }
            
            return desireds;
        }
    }
}
