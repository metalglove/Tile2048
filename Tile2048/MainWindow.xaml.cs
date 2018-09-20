using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AI;
using Tile2048.Autoplayer;

namespace Tile2048
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Random rSpawnTile = new Random();
        private List<Point> allPoints;
        private GameState tiles;

        public GameState Tiles
        {
            get => tiles ?? (tiles = new GameState());
            set
            {
                tiles = value;
                RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            allPoints = GenerateAllPossiblePoints();
            KeyDown += Slide;
        }

        private void Slide(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W)
                SlideTo(Direction.Up);
            else if (e.Key == Key.Down || e.Key == Key.S)
                SlideTo(Direction.Down);
            else if (e.Key == Key.Left || e.Key == Key.A)
                SlideTo(Direction.Left);
            else if (e.Key == Key.Right || e.Key == Key.D)
                SlideTo(Direction.Right);
            else
            {
                MessageBox.Show("Use the arrow or AWSD keys to slide the numbers.");
            }
        }

        #region Game
        private void SpawnTile()
        {
            int number = Get2Or4();
            Point position = GetAvailablePosition();
            Tiles.Add(new Tile(number, (int)position.X, (int)position.Y));
        }
        private Point GetAvailablePosition()
        {
            List<Point> availablePoints = allPoints.Except(Tiles.ToListOfPoints()).ToList();
            return availablePoints.ElementAt(rSpawnTile.Next(0, availablePoints.Count));
        }
        private int Get2Or4()
        {
            return rSpawnTile.Next(0, 100) >= 90 ? 4 : 2;
        }
        private static List<Point> GenerateAllPossiblePoints()
        {
            List<Point> allPoints = new List<Point>();
            Enumerable.Range(0, 4).All(T =>
            Enumerable.Range(0, 4).All(A =>
            {
                allPoints.Add(new Point(T, A));
                return true;
            }));
            return allPoints;
        }
        private void SpawnTileIfTilesChanged(GameState newTiles)
        {
            if (newTiles.Count == Tiles.Count)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    if (Tiles[i].Column != newTiles[i].Column || Tiles[i].Row != newTiles[i].Row)
                    {
                        Tiles = newTiles;
                        SpawnTile();
                        return;
                    }
                }
            }
            else
            {
                Tiles = newTiles;
                SpawnTile();
            }
        }
        private void SlideTo(Direction direction)
        {
            GameState newTiles = (GameState)Tiles.Clone();
            for (int mainDecider = 0; mainDecider <= 3; mainDecider++)
            {
                direction.GetTilesToEvaluate(newTiles, mainDecider, out List<Tile> tilesOnMainDecider);
                if (tilesOnMainDecider.Any())
                {
                    if (tilesOnMainDecider.Count == 1 && direction.NotOnLastCellOfDirectionToSlideTo(tilesOnMainDecider.Single()))
                    {
                        direction.SetTheOnlyTileToTheLastCellOfDirection(ref newTiles, tilesOnMainDecider);
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
                                        newTiles.Remove(currentTile);
                                        direction.GetTilesToEvaluate(newTiles, mainDecider, out tilesOnMainDecider);
                                    }
                                    // replace the one under if it is not the same already (needs better description...)
                                    else if (!OnSameLocationMinusDeepDecider(currentTile))
                                    {
                                        direction.SetTileDecider(ref currentTile, deepDecider);
                                    }
                                }
                                else if(!direction.VerifyDeepDeciderHasReachedLastCell(ref currentTile, deepDecider))
                                {
                                    goto deeper;
                                }
                            }
                        }
                    }
                }
            }
            SpawnTileIfTilesChanged(newTiles);
            if(IsGameOver())
            {
                MessageBox.Show($"Game Over! Your score was: {Tiles.Score}.");
            }
        }
        private bool IsGameOver()
        {
            foreach (Point point in GenerateAllPossiblePoints())
            {
                Tile currentTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X).DefaultIfEmpty(null).Single();
                if (currentTile == null)
                {
                    return false;
                }
                if (point.X == 0)
                {
                    if (point.Y == 0)
                    {
                        // don't check UP and LEFT
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (downTile == null || rightTile == null || currentTile.Number == downTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.Y == 3)
                    {
                        // don't check UP and RIGHT
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (downTile == null || leftTile == null || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check UP
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (downTile == null || leftTile == null || rightTile == null || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                }
                else if(point.X == 3)
                {
                    if (point.Y == 0)
                    {
                        // don't check DOWN and LEFT
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (upTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.Y == 3)
                    {
                        // don't check DOWN and RIGHT
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (upTile == null || leftTile == null || currentTile.Number == upTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check DOWN
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
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
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                        if (downTile == null || rightTile == null || currentTile.Number == downTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.X == 3)
                    {
                        // don't check LEFT and DOWN
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (upTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == rightTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check LEFT
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
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
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                        if (downTile == null || leftTile == null || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else if (point.X == 3)
                    {
                        // don't check RIGHT and DOWN
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        if (upTile == null || leftTile == null || currentTile.Number == upTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // don't check RIGHT
                        Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                        Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                        Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                        if (upTile == null || downTile == null || leftTile == null || currentTile.Number == upTile.Number || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    // do check every direction
                    Tile upTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X - 1).DefaultIfEmpty(null).Single();
                    Tile rightTile = Tiles.Where(tile => tile.Column == point.Y + 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                    Tile leftTile = Tiles.Where(tile => tile.Column == point.Y - 1 && tile.Row == point.X).DefaultIfEmpty(null).Single();
                    Tile downTile = Tiles.Where(tile => tile.Column == point.Y && tile.Row == point.X + 1).DefaultIfEmpty(null).Single();
                    if (upTile == null || downTile == null || leftTile == null || rightTile == null || currentTile.Number == upTile.Number || currentTile.Number == rightTile.Number || currentTile.Number == downTile.Number || currentTile.Number == leftTile.Number)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion Game

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged Members

        private void AutoPlay(object sender, RoutedEventArgs e)
        {
            /*
            Random random = new Random();
            while (!IsGameOver())
            {
                int x = random.Next(0, 100);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (x < 25)
                        SlideTo(Direction.Up);
                    else if (x < 50)
                        SlideTo(Direction.Down);
                    else if (x < 75)
                        SlideTo(Direction.Left);
                    else
                        SlideTo(Direction.Right);
                }, System.Windows.Threading.DispatcherPriority.Input);
                //Task.Delay(100).Wait();
            }
            */
            //NeuralNetwork nn = new NeuralNetwork(16, 8, 4);
            //NeuralNetwork.ShowWeights(nn.GetWeights(), 10, 3, true);
            double highestResult = 0;
            int i = 0;
            while(highestResult < 400000)
            {
                GameEnvironment gameEnvironment = new GameEnvironment(new GameState());
                GameState result = gameEnvironment.GetResult();
                highestResult = highestResult < result.Score ? result.Score : highestResult;
                Debug.WriteLine($"[{i++}] {result}");
            }
            
        }
    }
}
