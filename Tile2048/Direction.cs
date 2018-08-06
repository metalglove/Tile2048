using System;
using System.Collections.Generic;
using System.Linq;

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
        public static void GetTilesToEvaluate(this Direction direction, GameState gameState, int decider, out List<Tile> tilesOnMainDecider)
        {
            switch (direction)
            {
                case Direction.Left:
                    tilesOnMainDecider = gameState.Where(item => item.Row == decider).ToList();
                    break;
                case Direction.Right:
                    tilesOnMainDecider = gameState.Where(item => item.Row == decider).ToList();
                    break;
                case Direction.Up:
                    tilesOnMainDecider = gameState.Where(item => item.Column == decider).ToList();
                    break;
                case Direction.Down:
                    tilesOnMainDecider = gameState.Where(item => item.Column == decider).ToList();
                    break;
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
        public static void SetTheOnlyTileToTheLastCellOfDirection(this Direction direction, ref GameState gameState, List<Tile> tiles)
        {
            switch (direction)
            {
                case Direction.Left:
                    gameState.Single(tile => tile.Row == tiles[0].Row && tile.Column == tiles[0].Column).Column = 0;
                    break;
                case Direction.Right:
                    gameState.Single(tile => tile.Row == tiles[0].Row && tile.Column == tiles[0].Column).Column = 3;
                    break;
                case Direction.Up:
                    gameState.Single(tile => tile.Row == tiles[0].Row && tile.Column == tiles[0].Column).Row = 0;
                    break;
                case Direction.Down:
                    gameState.Single(tile => tile.Row == tiles[0].Row && tile.Column == tiles[0].Column).Row = 3;
                    break;
                default:
                    throw new ArgumentException("SetTheOnlyTileToTheLastCellOfDirection faulted.");
            }
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
        public static void GetTile(this Direction direction, List<Tile> tiles, int mainDecider, int decider, out Tile currentTile)
        {
            switch (direction)
            {
                case Direction.Left:
                    currentTile = tiles.Single(tile => tile.Column == decider && tile.Row == mainDecider);
                    break;
                case Direction.Right:
                    currentTile = tiles.Single(tile => tile.Column == decider && tile.Row == mainDecider);
                    break;
                case Direction.Up:
                    currentTile = tiles.Single(tile => tile.Row == decider && tile.Column == mainDecider);
                    break;
                case Direction.Down:
                    currentTile = tiles.Single(tile => tile.Row == decider && tile.Column == mainDecider);
                    break;
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
        public static void SetTileDecider(this Direction direction, ref Tile tile, int deepDecider)
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
        public static void GetDecider(this Direction direction, out int decider)
        {
            switch (direction)
            {
                case Direction.Left:
                    decider = 1;
                    break;
                case Direction.Right:
                    decider = 2;
                    break;
                case Direction.Up:
                    decider = 1;
                    break;
                case Direction.Down:
                    decider = 2;
                    break;
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
