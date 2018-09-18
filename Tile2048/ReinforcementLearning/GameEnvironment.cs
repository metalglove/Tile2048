using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tile2048.ReinforcementLearning
{
    public class GameEnvironment
    {
        private bool endReached = false;
        private GameState currentGameState;
        private List<InterpretedGameState> interpretedGameStates = new List<InterpretedGameState>();

        public GameEnvironment(GameState startingState)
        {
            currentGameState = startingState;
            while (!IsGameOver(currentGameState))
            {
                InterpretedGameState interpretedGameState = Interpreter(currentGameState);
                currentGameState = interpretedGameState.GameState;
                interpretedGameStates.Add(interpretedGameState);
            }
        }

        public GameState GetResult()
        {
            return currentGameState;
        }
        private InterpretedGameState Interpreter(GameState gameState)
        {
            // determine possible actions
            List<InterpretedGameState> possibleGameStates = GetPossibleGameStates(gameState);

            // find best reward for actions
            double highestValue = possibleGameStates.Max(pgs => pgs.GameState.Value);

            // upon choosing the best rewarded action, spawn a new tile
            InterpretedGameState bestGameState = possibleGameStates.First(pgs => pgs.GameState.Value == highestValue);
            bestGameState.GameState.SpawnTile();

            return bestGameState;
        }
        private static List<InterpretedGameState> GetPossibleGameStates(GameState gameState)
        {
            List<InterpretedGameState> possibleGameStates = new List<InterpretedGameState>();
            foreach(Direction direction in new Direction[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down })
            {
                InterpretedGameState newGameState = CopyAndSlideTo(gameState, direction);
                if (!Equals(newGameState, null))
                {
                    possibleGameStates.Add(newGameState);
                }
            }
            return possibleGameStates;
        }
        private static InterpretedGameState CopyAndSlideTo(GameState gameState, Direction direction)
        {
            GameState clonedGameState = (GameState)gameState.Clone();
            for (int mainDecider = 0; mainDecider <= 3; mainDecider++)
            {
                direction.GetTilesToEvaluate(clonedGameState, mainDecider, out List<Tile> tilesOnMainDecider);
                if (tilesOnMainDecider.Any())
                {
                    if (tilesOnMainDecider.Count == 1 && direction.NotOnLastCellOfDirectionToSlideTo(tilesOnMainDecider.Single()))
                    {
                        direction.SetTheOnlyTileToTheLastCellOfDirection(ref clonedGameState, tilesOnMainDecider);
                    }
                    else
                    {
                        // shift until the maindecider or until a tile is found
                        // check if they are the same number ifso, sum and merge to one tile.
                        for (direction.GetDecider(out int decider); direction.GetDeciderCondition(decider); direction.GetDeciderIterator(ref decider))
                        {
                            if (tilesOnMainDecider.Any(direction.AnyTileOnDecider(decider)))
                            {
                                direction.GetTile(tilesOnMainDecider, mainDecider, decider, out Tile currentTile);
                                int deepDecider = decider;
                                deeper: // label to repeat
                                direction.IncrementOrDecrementDeepDecider(ref deepDecider);
                                bool OnSameLocationMinusDeepDecider(Tile tile) => direction.OnSameLocationMinusDeepDecider(tile, mainDecider, deepDecider);
                                if (tilesOnMainDecider.Any(OnSameLocationMinusDeepDecider))
                                {
                                    Tile OtherTile = tilesOnMainDecider.Single(OnSameLocationMinusDeepDecider);
                                    if (OtherTile.Number == currentTile.Number)
                                    {
                                        // merge tile
                                        OtherTile.Number += currentTile.Number;
                                        clonedGameState.Remove(currentTile);
                                        direction.GetTilesToEvaluate(clonedGameState, mainDecider, out tilesOnMainDecider);
                                    }
                                    // replace the one under if it is not the same already (needs better description...)
                                    else if (!OnSameLocationMinusDeepDecider(currentTile))
                                    {
                                        direction.SetTileDecider(ref currentTile, deepDecider);
                                    }
                                }
                                else if (!direction.VerifyDeepDeciderHasReachedLastCell(ref currentTile, deepDecider))
                                {
                                    goto deeper;
                                }
                            }
                        }
                    }
                }
            }
            if (gameState.Equals(clonedGameState))
            {
                return null;
            }
            clonedGameState.Actions++;
            return new InterpretedGameState(clonedGameState, direction);
        }
        private static bool IsGameOver(GameState gameState)
        {
            foreach (Point point in gameState.AllPoints)
            {
                Tile currentTile = gameState.Where(tile => tile.Column == point.Y && tile.Row == point.X).DefaultIfEmpty(null).Single();
                if (currentTile == null)
                {
                    return false;
                }
                Tile downTile = GetDownTile(gameState, point);
                Tile rightTile = GetRightTile(gameState, point);
                Tile leftTile = GetLeftTile(gameState, point);
                Tile upTile = GetUpTile(gameState, point);
                if (point.X == 0)
                {
                    if (point.Y == 0)
                    {
                        // don't check UP and LEFT
                        if (downTile == null || rightTile == null || currentTile.Number == downTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.Y == 3)
                    {
                        // don't check UP and RIGHT
                        if (downTile == null || leftTile == null || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check UP
                        if (downTile == null || leftTile == null || rightTile == null || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                }
                else if (point.X == 3)
                {
                    if (point.Y == 0)
                    {
                        // don't check DOWN and LEFT
                        if (upTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.Y == 3)
                    {
                        // don't check DOWN and RIGHT
                        if (upTile == null || leftTile == null || currentTile.Number == upTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check DOWN
                        if (upTile == null || leftTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == leftTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                }
                else if (point.Y == 0)
                {
                    if (point.X == 0)
                    {
                        // don't check LEFT and UP
                        if (downTile == null || rightTile == null || currentTile.Number == downTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.X == 3)
                    {
                        // don't check LEFT and DOWN
                        if (upTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check LEFT
                        if (upTile == null || downTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == downTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                }
                else if (point.Y == 3)
                {
                    if (point.X == 0)
                    {
                        // don't check RIGHT and UP
                        if (downTile == null || leftTile == null || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.X == 3)
                    {
                        // don't check RIGHT and DOWN
                        if (upTile == null || leftTile == null || currentTile.Number == upTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check RIGHT
                        if (upTile == null || downTile == null || leftTile == null || currentTile.Number == upTile.Number || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    // do check every direction
                    if (upTile == null || downTile == null || leftTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == rightTile.Number || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static Tile GetUpTile(GameState gameState, Point point)
        {
            return gameState.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
        }
        private static Tile GetLeftTile(GameState gameState, Point point)
        {
            return gameState.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
        }
        private static Tile GetRightTile(GameState gameState, Point point)
        {
            return gameState.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
        }
        private static Tile GetDownTile(GameState gameState, Point point)
        {
            return gameState.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
        }
    }
}
