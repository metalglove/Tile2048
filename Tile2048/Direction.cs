using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Tile2048
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    public static class DirectionExtension
    {
        public static List<Tile> GetTilesToEvaluate(this Direction direction, GameState gameState, int decider)
        {
            switch (direction)
            {
                case Direction.Left:
                    return gameState.Where(item => item.Row == decider).ToList();
                case Direction.Right:
                    return gameState.Where(item => item.Row == decider).ToList();
                case Direction.Up:
                    return gameState.Where(item => item.Column == decider).ToList();
                case Direction.Down:
                    return gameState.Where(item => item.Column == decider).ToList();
                default:
                    throw new ArgumentException("GetTilesToEvaluate faulted.");
            }
        }
        public static bool NotOnLastCellOfDirectionToSlideTo(this Direction direction, Tile tile)
        {
            switch (direction)
            {
                case Direction.Left:
                    return tile.Column != 0;
                case Direction.Right:
                    return tile.Column != 3;
                case Direction.Up:
                    return tile.Row != 0;
                case Direction.Down:
                    return tile.Row != 3;
                default:
                    throw new ArgumentException("NotOnLastCellOfDirectionToSlideTo faulted.");
            }
        }
        public static GameState SetTheOnlyTileToTheLastCellOfDirection(this Direction direction, GameState gameState, List<Tile> tiles)
        {
            Tile onlyTile() => gameState.Single(tile => tile.Row == tiles[0].Row && tile.Column == tiles[0].Column);
            switch (direction)
            {
                case Direction.Left:
                    onlyTile().Column = 0;
                    break;
                case Direction.Right:
                    onlyTile().Column = 3;
                    break;
                case Direction.Up:
                    onlyTile().Row = 0;
                    break;
                case Direction.Down:
                    onlyTile().Row = 3;
                    break;
                default:
                    throw new ArgumentException("SetTheOnlyTileToTheLastCellOfDirection faulted.");
            }
            return gameState;
        }
        public static Func<Tile, bool> AnyTileOnDecider(this Direction direction, int decider)
        {
            switch (direction)
            {
                case Direction.Left:
                    return tile => tile.Column == decider;
                case Direction.Right:
                    return tile => tile.Column == decider;
                case Direction.Up:
                    return tile => tile.Row == decider;
                case Direction.Down:
                    return tile => tile.Row == decider;
                default:
                    throw new ArgumentException("AnyTileOnDecider faulted.");
            }
        }
        public static Tile GetTile(this Direction direction, List<Tile> tiles, int mainDecider, int decider)
        {
            switch (direction)
            {
                case Direction.Left:
                    return tiles.SingleOrDefault(tile => tile.Column == decider && tile.Row == mainDecider);
                case Direction.Right:
                    return tiles.SingleOrDefault(tile => tile.Column == decider && tile.Row == mainDecider);
                case Direction.Up:
                    return tiles.SingleOrDefault(tile => tile.Row == decider && tile.Column == mainDecider);
                case Direction.Down:
                    return tiles.SingleOrDefault(tile => tile.Row == decider && tile.Column == mainDecider);
                default:
                    throw new ArgumentException("GetTile faulted.");
            }
        }
        public static void IncrementOrDecrementDeepDecider(this Direction direction, ref int deepDecider)
        {
            switch (direction)
            {
                case Direction.Left:
                    deepDecider--;
                    break;
                case Direction.Right:
                    deepDecider++;
                    break;
                case Direction.Up:
                    deepDecider--;
                    break;
                case Direction.Down:
                    deepDecider++;
                    break;
                default:
                    throw new ArgumentException("IncrementOrDecrementDeepDecider faulted.");
            }
        }
        public static bool OnSameLocationMinusDeepDecider(this Direction direction, Tile tile, int mainDecider, int deepDecider)
        {
            switch (direction)
            {
                case Direction.Left:
                    return tile.Row == mainDecider && tile.Column == deepDecider;
                case Direction.Right:
                    return tile.Row == mainDecider && tile.Column == deepDecider;
                case Direction.Up:
                    return tile.Row == deepDecider && tile.Column == mainDecider;
                case Direction.Down:
                    return tile.Row == deepDecider && tile.Column == mainDecider;
                default:
                    throw new ArgumentException("OnSameLocationMinusDeepdecider faulted.");
            }
        }
        public static Tile SetTileDecider(this Direction direction, Tile tile, int deepDecider)
        {
            switch (direction)
            {
                case Direction.Left:
                    tile.Column = deepDecider + 1;
                    break;
                case Direction.Right:
                    tile.Column = deepDecider - 1;
                    break;
                case Direction.Up:
                    tile.Row = deepDecider + 1;
                    break;
                case Direction.Down:
                    tile.Row = deepDecider - 1;
                    break;
                default:
                    throw new ArgumentException("SetTileDecider faulted.");
            }
            return tile;
        }
        public static bool VerifyDeepDeciderHasReachedLastCell(this Direction direction, ref Tile tile, int deepDecider)
        {
            bool returnValue = false;
            switch (direction)
            {
                case Direction.Left:
                    if (returnValue = deepDecider == 0)
                        tile.Column = deepDecider;
                    break;
                case Direction.Right:
                    if (returnValue = deepDecider == 3)
                        tile.Column = deepDecider;
                    break;
                case Direction.Up:
                    if(returnValue = deepDecider == 0)
                        tile.Row = deepDecider;
                    break;
                case Direction.Down:
                    if (returnValue = deepDecider == 3)
                        tile.Row = deepDecider;
                    break;
                default:
                    throw new ArgumentException("VerifyDeepDeciderHasReachedLastCell faulted.");
            }
            return returnValue;
        }
        public static int GetDecider(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return 1;
                case Direction.Right:
                    return 2;
                case Direction.Up:
                    return 1;
                case Direction.Down:
                    return 2;
                default:
                    throw new ArgumentException("GetDecider faulted.");
            }
        }
        public static bool GetDeciderCondition(this Direction direction, int decider)
        {
            switch (direction)
            {
                case Direction.Left:
                    return decider <= 3;
                case Direction.Right:
                    return decider >= 0;
                case Direction.Up:
                    return decider <= 3;
                case Direction.Down:
                    return decider >= 0;
                default:
                    throw new ArgumentException("GetDeciderCondition faulted.");
            }
        }
        public static void GetDeciderIterator(this Direction direction, ref int decider)
        {
            switch (direction)
            {
                case Direction.Left:
                    decider++;
                    break;
                case Direction.Right:
                    decider--;
                    break;
                case Direction.Up:
                    decider++;
                    break;
                case Direction.Down:
                    decider--;
                    break;
                default:
                    throw new ArgumentException("GetDeciderIterator faulted.");
            }
        }
    }
}
