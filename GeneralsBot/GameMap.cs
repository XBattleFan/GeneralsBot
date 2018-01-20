using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GeneralsBot {
    public class GameMap {
        private const int TILE_EMPTY        = -1;
        private const int TILE_MOUNTAIN     = -2;
        private const int TILE_FOG          = -3;
        private const int TILE_FOG_OBSTACLE = -4;

        public           int        Width  { get; }
        public           int        Height { get; }
        private readonly IList<int> _armyValues;
        private readonly IList<int> _terrainValues;
        private readonly IList<int> _cities;
        private readonly IList<int> _generals;

        private GameMap(int        width,
                        int        height,
                        IList<int> armyValues,
                        IList<int> terrainValues,
                        IList<int> cities,
                        IList<int> generals) {
            Width          = width;
            Height         = height;
            _armyValues    = armyValues;
            _terrainValues = terrainValues;
            _cities        = cities;
            _generals      = generals;
        }

        public GameTile this[int x, int y] {
            get {
                bool hasCity    = HasCity(x, y);
                bool hasGeneral = HasGeneral(x, y);
                switch (TerrainAt(x, y)) {
                    case TILE_FOG:
                        return new FogTile();

                    case TILE_FOG_OBSTACLE:
                        return new ObstacleFogTile();

                    case TILE_EMPTY:
                        if (hasCity) return new CityTile(TerrainAt(x, y), ArmyAt(x, y));
                        return new EmptyTile();

                    case TILE_MOUNTAIN:
                        return new MountainTile();

                    default:
                        if (hasCity) return new CityTile(TerrainAt(x, y), ArmyAt(x, y));
                        if (hasGeneral) return new GeneralTile(TerrainAt(x, y), ArmyAt(x, y));
                        else return new OccupiedTile(TerrainAt(x, y), ArmyAt(x, y));
                }
            }
        }

        private bool     HasCity(int     x, int y) => _cities.Any(c =>   UCoord(x, y) == c);
        private bool     HasGeneral(int  x, int y) => _generals.Any(c => UCoord(x, y) == c);
        private int      TerrainAt(int   x, int y) => _terrainValues[UCoord(x, y)];
        private int      ArmyAt(int      x, int y) => _armyValues   [UCoord(x, y)];
        public  int      UCoord(int      x, int y) => x + y * Width;
        public  int      UCoord(Position p)        => p.X + p.Y * Width;
        public  int      UnitsAt(Position p)       => ArmyAt(p.X, p.Y);
        public  int      TotalArmies(int index)    => _armyValues.Where((c, i) => _terrainValues[i] == index).Sum();
        private Position FromUCoord(int  c)        => new Position(c % Width, c / Width);

        public Position GeneralPosition(int playerIndex) {
            return FromUCoord(_generals[playerIndex]);
        }

        public static GameMap FromRawLists(IList<int> map, IList<int> cities, IList<int> generals) {
            int size = map[0] * map[1];
            return new GameMap(map[0], map[1], map.Skip(2).Take(size).ToList(), map.Skip(2 + size).ToList(),
                               cities, generals);
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

        public object this[Position position] => this[position.X, position.Y];
    }
}
