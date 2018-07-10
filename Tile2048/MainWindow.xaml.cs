using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Tile2048
{
    public partial class MainWindow : Window
    {
        private Random rSpawnTile = new Random();
        private int[,] grid = new int[4, 4];
        public ObservableCollection<Tile> Tiles { get; set; } = new ObservableCollection<Tile>(); 

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
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
            int[,] position = GetAvailablePosition();
            
        }

        private int[,] GetAvailablePosition()
        {
            throw new NotImplementedException();
        }

        private int Get2Or4()
        {
            throw new NotImplementedException();
        }
    }


    public class GameState
    {

    }
}
