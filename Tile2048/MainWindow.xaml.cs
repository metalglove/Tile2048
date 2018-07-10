using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

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
            StartGame();
        }

        #region Game
        private void StartGame()
        {
            SpawnTile();
            SpawnTile();
        }
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

        private int Get2Or4()
        {
            return rSpawnTile.Next(0, 100) >= 90 ? 4 : 2;
        }
        #endregion Game

        #region Movement
        private void SlideUp(object sender, RoutedEventArgs e)
        {
            for(int column = 0; column <= 3; column++)
            {
                List<Tile> tilesOnColumn = Tiles.Where(item => item.Column == column).ToList();
                if (tilesOnColumn.Any())
                {
                    if (tilesOnColumn.Count == 1 && tilesOnColumn.Single().Row != 0)
                    {
                        Tiles.Single(tile => tile.Row == tilesOnColumn[0].Row && tile.Column == tilesOnColumn[0].Column).Row = 0;
                    }
                    else
                    {
                        // shift until the top or until a tile is found
                        // check if they are the same number ifso, sum and merge to one tile.
                        for (int row = 1; row <= 3; row++)
                        {
                            if (tilesOnColumn.Any(tile => tile.Row == row))
                            {
                                Tile currentTile = tilesOnColumn.SingleOrDefault(tile => tile.Row == row && tile.Column == column);
                                int deepRow = row;
                                deeper:
                                deepRow--;
                                bool OnSameLocationMinusDeepRow(Tile tile) => tile.Row == deepRow && tile.Column == column;
                                if (tilesOnColumn.Any(OnSameLocationMinusDeepRow))
                                {
                                    Tile OtherTile = tilesOnColumn.Single(OnSameLocationMinusDeepRow);
                                    if (OtherTile.Number == currentTile.Number)
                                    {
                                        // merge
                                        OtherTile.Number += currentTile.Number;
                                        Tiles.Remove(currentTile);
                                    }
                                    else
                                    {
                                        // replace the one under if it is not the same already
                                        if (!OnSameLocationMinusDeepRow(currentTile))
                                        {
                                            currentTile.Row = deepRow + 1;
                                        }
                                        continue;
                                    }
                                }
                                else
                                {
                                    if(deepRow == 0)
                                    {
                                        currentTile.Row = deepRow;
                                    }
                                    else
                                    {
                                        goto deeper;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void SlideLeft(object sender, RoutedEventArgs e)
        {
            foreach (Tile item in Tiles)
            {
                int row = item.Row;
                int col = item.Column;

                if (row == 0)
                {
                    continue;
                }

            }
        }
        private void SlideDown(object sender, RoutedEventArgs e)
        {
            for (int column = 0; column <= 3; column++)
            {
                List<Tile> tilesOnColumn = Tiles.Where(item => item.Column == column).ToList();
                if (tilesOnColumn.Any())
                {
                    if (tilesOnColumn.Count == 1 && tilesOnColumn.Single().Row != 3)
                    {
                        Tiles.Single(tile => tile.Row == tilesOnColumn[0].Row && tile.Column == tilesOnColumn[0].Column).Row = 3;
                    }
                    else
                    {
                        // shift until the bottom or until a tile is found
                        // check if they are the same number ifso, sum and merge to one tile.
                        for (int row = 2; row >= 0; row--)
                        {
                            if (tilesOnColumn.Any(tile => tile.Row == row))
                            {
                                Tile currentTile = tilesOnColumn.SingleOrDefault(tile => tile.Row == row && tile.Column == column);
                                int deepRow = row;
                                deeper:
                                deepRow++;
                                bool OnSameLocationMinusDeepRow(Tile tile) => tile.Row == deepRow && tile.Column == column;
                                if (tilesOnColumn.Any(OnSameLocationMinusDeepRow))
                                {
                                    Tile OtherTile = tilesOnColumn.Single(OnSameLocationMinusDeepRow);
                                    if (OtherTile.Number == currentTile.Number)
                                    {
                                        // merge
                                        OtherTile.Number += currentTile.Number;
                                        Tiles.Remove(currentTile);
                                    }
                                    else
                                    {
                                        // replace the one above if it is not the same already
                                        if (!OnSameLocationMinusDeepRow(currentTile))
                                        {
                                            currentTile.Row = deepRow - 1;
                                        }
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (deepRow == 3)
                                    {
                                        currentTile.Row = deepRow;
                                    }
                                    else
                                    {
                                        goto deeper;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void SlideRight(object sender, RoutedEventArgs e)
        {
            foreach (Tile item in Tiles)
            {
                int row = item.Row;
                int col = item.Column;

                if (row == 0)
                {
                    continue;
                }

            }
        }
        #endregion Movement

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SpawnTile();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged Members
    }

    public class GameState : ObservableCollection<Tile>, INotifyPropertyChanged
    {
        private int score = 0;
        public int Score
        {
            get => score;
            set
            {
                score = value;
                RaisePropertyChanged();
            }
        }

        public new void Add(Tile item)
        {
            Score += item.Number;
            base.Add(item);
        }
        public List<Point> ToListOfPoints()
        {
            List<Point> points = new List<Point>();
            foreach (Tile tile in this)
            {
                points.Add(new Point(tile.Row, tile.Column));
            }
            return points;
        }

        #region INotifyPropertyChanged Members
        public new event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged Members
    }
}
