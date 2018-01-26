using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GeneralsBot {
    public class GameMap {
        public const int TileEmpty       = -1;
        public const int TileMountain    = -2;
        public const int TileFog         = -3;
        public const int TileFogObstacle = -4;

        public           int        Width  { get; }
        public           int        Height { get; }

        public IList<int> CurrentVisiblePositions => _terrainValues.Select((v, i) => new { v, i })
                                                            .Where(x => x.v > TileFog)
                                                            .Select(x => x.i).ToList();

        private readonly HashSet<int> _seen;
        private readonly IList<int> _armyValues;
        private readonly IList<int> _terrainValues;
        private readonly HashSet<int> _cities;
        private readonly IList<int> _generals;

        private GameMap(int        width,
                        int        height,
                        IList<int> armyValues,
                        IList<int> terrainValues,
                        HashSet<int> cities,
                        IList<int> generals,
                        HashSet<int> seen) {
            Width          = width;
            Height         = height;
            _armyValues    = armyValues;
            _terrainValues = terrainValues;
            _cities        = cities;
            _generals      = generals;
            _seen = seen;
            
            foreach (int p in CurrentVisiblePositions) {
                _seen.Add(p);
            }
        }

        public GameTile this[int x, int y] {
            get {
                bool hasCity    = HasCity(x, y);
                bool hasGeneral = HasGeneral(x, y);
                switch (TerrainAt(x, y)) {
                    case TileFog when hasGeneral:
                        return new FogTile(true, typeof(GeneralTile));

                    case TileFog:
                        return new FogTile();
                        
                    case TileFogObstacle when hasCity:
                        return new FogTile(true, typeof(CityTile));

                    case TileFogObstacle:
                        return new ObstacleFogTile();

                    case TileEmpty:
                        if (hasCity) return new CityTile(TerrainAt(x, y), ArmyAt(x, y));
                        return new EmptyTile();

                    case TileMountain:
                        return new MountainTile();

                    default:
                        if (hasCity) return new CityTile(TerrainAt(x, y), ArmyAt(x, y));
                        if (hasGeneral) return new GeneralTile(TerrainAt(x, y), ArmyAt(x, y));
                        else return new OccupiedTile(TerrainAt(x, y), ArmyAt(x, y));
                }
            }
        }

        private bool     HasCity(int     x, int y) => _cities.Any(c   => UCoord(x, y) == c);
        private bool     HasGeneral(int  x, int y) => _generals.Any(c => UCoord(x, y) == c);
        private int      TerrainAt(int   x, int y) => _terrainValues[UCoord(x, y)];
        private int      ArmyAt(int      x, int y) => _armyValues   [UCoord(x, y)];
        public  int      UCoord(int      x, int y) => x + y * Width;
        public  int      UCoord(Position p)        => UCoord(p.X, p.Y);
        public  int      UnitsAt(Position p)       => ArmyAt(p.X, p.Y);
        public  int      TotalArmies(int index)    => _armyValues.Where((c, i) => _terrainValues[i] == index).Sum();
        public  bool     EverSeen(Position p)      => _seen.Contains(UCoord(p));
        private Position FromUCoord(int  c)        => new Position(c % Width, c / Width);

        public Position GeneralPosition(int playerIndex) {
            return FromUCoord(_generals[playerIndex]);
        }

        public static GameMap FromRawLists(IList<int> map, HashSet<int> cities, IList<int> generals, HashSet<int> seen) {
            int size = map[0] * map[1];
            return new GameMap(map[0], map[1], map.Skip(2).Take(size).ToList(), map.Skip(2 + size).ToList(),
                               cities, generals, seen);
        }

        public void PrettyPrint() {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    this[x, y].PrettyPrint();
                }

                Console.WriteLine(Environment.NewLine);
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
        }

        public GameTile this[Position position] => this[position.X, position.Y];
    }
}
