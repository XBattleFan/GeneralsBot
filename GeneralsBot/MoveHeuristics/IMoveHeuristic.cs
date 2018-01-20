using System.Collections.Generic;

namespace GeneralsBot.MoveHeuristics {
    public interface IMoveHeuristic {
        IList<(int, Attack)> Calculate(GameMap map, int playerIndex);
    }
}
