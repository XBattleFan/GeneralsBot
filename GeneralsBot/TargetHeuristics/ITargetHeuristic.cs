using System.Collections.Generic;

namespace GeneralsBot.TargetHeuristics {
    public interface ITargetHeuristic {
        IList<(int, Position)> Calculate(GameMap map, int playerIndex);
    }
}
