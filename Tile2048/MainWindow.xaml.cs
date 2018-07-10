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
            SpawnTile();
            SpawnTile();
        }

        #region Game
        private void StartGame()
        {

        }
        private void EndGame()
        {

        }
        #endregion Game

        private void SpawnTile()
        {
            int number = Get2Or4();
            Point position = GetAvailablePosition();
            Tiles.Add(new Tile(number, (int)position.X, (int)position.Y));
        }

        private Point GetAvailablePosition()
        {
            List<Point> allPoints = new List<Point>();
            Enumerable.Range(0, 4).All(T => 
            Enumerable.Range(0, 4).All(A =>
            {
                allPoints.Add(new Point(T, A));
                return true;
            }));

            List<Point> availablePoints = allPoints.Except(Tiles.ToListOfPoints()).ToList();
            return availablePoints.ElementAt(rSpawnTile.Next(0, availablePoints.Count));
        }

        private int Get2Or4()
        {
            return rSpawnTile.Next(0, 100) >= 90 ? 4 : 2;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged Members

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SpawnTile();
        }
    }


    public class GameState : ObservableCollection<Tile>
    {
        public List<Point> ToListOfPoints()
        {
            List<Point> points = new List<Point>();
            foreach (Tile tile in this)
            {
                points.Add(new Point(tile.Row, tile.Column));
            }
            return points;
        }
    }
}
