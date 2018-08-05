using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
            KeyDown += Slide;
        }

        private void Slide(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W)
                SlideUp(this, new RoutedEventArgs());
            else if (e.Key == Key.Down || e.Key == Key.S)
                SlideDown(this, new RoutedEventArgs());
            else if (e.Key == Key.Left || e.Key == Key.A)
                SlideLeft(this, new RoutedEventArgs());
            else if (e.Key == Key.Right || e.Key == Key.D)
                SlideRight(this, new RoutedEventArgs());
            else
            {
                MessageBox.Show("Use the arrow or AWSD keys to slide the numbers.");
            }
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
        #endregion Game

        #region Movement
        private void SlideUp(object sender, RoutedEventArgs e)
        {
            GameState newTiles = (GameState)Tiles.Clone();
            for (int column = 0; column <= 3; column++)
            {
                List<Tile> tilesOnColumn = newTiles.Where(item => item.Column == column).ToList();
                if (tilesOnColumn.Any())
                {
                    if (tilesOnColumn.Count == 1 && tilesOnColumn.Single().Row != 0)
                    {
                        newTiles.Single(tile => tile.Row == tilesOnColumn[0].Row && tile.Column == tilesOnColumn[0].Column).Row = 0;
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
                                        newTiles.Remove(currentTile);
                                        tilesOnColumn = newTiles.Where(item => item.Column == column).ToList();
                                    }
                                    else
                                    {
                                        // replace the one under if it is not the same already
                                        if (!OnSameLocationMinusDeepRow(currentTile))
                                        {
                                            currentTile.Row = deepRow + 1;
                                        }
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
            SpawnTileIfTilesChanged(newTiles);
        }
        private void SlideLeft(object sender, RoutedEventArgs e)
        {
            GameState newTiles = (GameState)Tiles.Clone();
            for (int row = 0; row <= 3; row++)
            {
                List<Tile> tilesOnRow = newTiles.Where(item => item.Row == row).ToList();
                if (tilesOnRow.Any())
                {
                    if (tilesOnRow.Count == 1 && tilesOnRow.Single().Column != 0)
                    {
                        newTiles.Single(tile => tile.Row == tilesOnRow[0].Row && tile.Column == tilesOnRow[0].Column).Column = 0;
                    }
                    else
                    {
                        // shift until the left or until a tile is found
                        // check if they are the same number ifso, sum and merge to one tile.
                        for (int column = 1; column <= 3; column++)
                        {
                            if (tilesOnRow.Any(tile => tile.Column == column))
                            {
                                Tile currentTile = tilesOnRow.SingleOrDefault(tile => tile.Column == column && tile.Row == row);
                                int deepColumn = column;
                                deeper:
                                deepColumn--;
                                bool OnSameLocationMinusDeepColumn(Tile tile) => tile.Row == row && tile.Column == deepColumn;
                                if (tilesOnRow.Any(OnSameLocationMinusDeepColumn))
                                {
                                    Tile OtherTile = tilesOnRow.Single(OnSameLocationMinusDeepColumn);
                                    if (OtherTile.Number == currentTile.Number)
                                    {
                                        // merge
                                        OtherTile.Number += currentTile.Number;
                                        newTiles.Remove(currentTile);
                                        tilesOnRow = newTiles.Where(item => item.Row == row).ToList();
                                    }
                                    else
                                    {
                                        // replace the one under if it is not the same already
                                        if (!OnSameLocationMinusDeepColumn(currentTile))
                                        {
                                            currentTile.Column = deepColumn + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (deepColumn == 0)
                                    {
                                        currentTile.Column = deepColumn;
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
            SpawnTileIfTilesChanged(newTiles);
        }
        private void SlideDown(object sender, RoutedEventArgs e)
        {
            GameState newTiles = (GameState)Tiles.Clone();
            for (int column = 0; column <= 3; column++)
            {
                List<Tile> tilesOnColumn = newTiles.Where(item => item.Column == column).ToList();
                if (tilesOnColumn.Any())
                {
                    if (tilesOnColumn.Count == 1 && tilesOnColumn.Single().Row != 3)
                    {
                        newTiles.Single(tile => tile.Row == tilesOnColumn[0].Row && tile.Column == tilesOnColumn[0].Column).Row = 3;
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
                                        newTiles.Remove(currentTile);
                                        tilesOnColumn = newTiles.Where(item => item.Column == column).ToList();
                                    }
                                    else
                                    {
                                        // replace the one above if it is not the same already
                                        if (!OnSameLocationMinusDeepRow(currentTile))
                                        {
                                            currentTile.Row = deepRow - 1;
                                        }
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
            SpawnTileIfTilesChanged(newTiles);
        }
        private void SlideRight(object sender, RoutedEventArgs e)
        {
            GameState newTiles = (GameState)Tiles.Clone();
            for (int row = 0; row <= 3; row++)
            {
                List<Tile> tilesOnRow = newTiles.Where(item => item.Row == row).ToList();
                if (tilesOnRow.Any())
                {
                    if (tilesOnRow.Count == 1 && tilesOnRow.Single().Column != 3)
                    {
                        newTiles.Single(tile => tile.Row == tilesOnRow[0].Row && tile.Column == tilesOnRow[0].Column).Column = 3;
                    }
                    else
                    {
                        // shift until the right or until a tile is found
                        // check if they are the same number ifso, sum and merge to one tile.
                        for (int column = 2; column >= 0; column--)
                        {
                            if (tilesOnRow.Any(tile => tile.Column == column))
                            {
                                Tile currentTile = tilesOnRow.SingleOrDefault(tile => tile.Column == column && tile.Row == row);
                                int deepColumn = column;
                                deeper:
                                deepColumn++;
                                bool OnSameLocationMinusDeepColumn(Tile tile) => tile.Row == row && tile.Column == deepColumn;
                                if (tilesOnRow.Any(OnSameLocationMinusDeepColumn))
                                {
                                    Tile OtherTile = tilesOnRow.Single(OnSameLocationMinusDeepColumn);
                                    if (OtherTile.Number == currentTile.Number)
                                    {
                                        // merge
                                        OtherTile.Number += currentTile.Number;
                                        newTiles.Remove(currentTile);
                                        tilesOnRow = newTiles.Where(item => item.Row == row).ToList();
                                    }
                                    else
                                    {
                                        // replace the one under if it is not the same already
                                        if (!OnSameLocationMinusDeepColumn(currentTile))
                                        {
                                            currentTile.Column = deepColumn - 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (deepColumn == 3)
                                    {
                                        currentTile.Column = deepColumn;
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
            SpawnTileIfTilesChanged(newTiles);
        }
        #endregion Movement

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged Members
    }
}
