using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Tile2048
{
    public class GameState : ObservableCollection<Tile>, INotifyPropertyChanged, ICloneable
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

        public GameState()
        {

        }
        public GameState(int Score, IEnumerable<Tile> tiles) : base(tiles)
        {
            score = Score;
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
        public object Clone()
        {
            return new GameState(Score, this.Select(tile => (Tile)tile.Clone()));
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
