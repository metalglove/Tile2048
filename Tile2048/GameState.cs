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
        private Random random;
        private int actions = 0;
        private int score = 0;

        public int HighestTile => this.Max(tile => tile.Number);
        public List<Point> AllPoints => GenerateAllPossiblePoints();
        public int Score
        {
            get => score;
            set
            {
                score = value;
                RaisePropertyChanged();
            }
        }
        public int Actions
        {
            get => actions;
            set => actions = value;
        }
        public double Value
        {
            get => Score + Count - (actions * 0.123);
        }

        public GameState()
        {
            Initialize();
            SpawnTile();
            SpawnTile();
        }
        public GameState(int Score, int Actions, IEnumerable<Tile> tiles) : base(tiles)
        {
            score = Score;
            actions = Actions;
            Initialize();
        }

        private void Initialize()
        {
            random = new Random();
        }
        public new void Remove(Tile item)
        {
            Score += item.Number * 2;
            base.Remove(item);
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
            return new GameState(Score, Actions, this.Select(tile => (Tile)tile.Clone()));
        }
        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                GameState gameState = (GameState)obj;
                if (Count == gameState.Count && Score == gameState.Score)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (gameState[i].Column != this[i].Column || gameState[i].Row != this[i].Row)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() * 17 + this.Score.GetHashCode() * 17 + this.Count.GetHashCode() * 17 + this.Actions.GetHashCode() * 17;
        }
        public void SpawnTile()
        {
            int number = random.Next(0, 100) >= 90 ? 4 : 2;
            Point position = GetAvailablePosition();
            Add(new Tile(number, (int)position.X, (int)position.Y));
        }
        public void SpawnTile(Point point, int number)
        {
            Add(new Tile(number, (int)point.X, (int)point.Y));
        }
        public List<Point> GetAvailablePositions()
        {
            return AllPoints.Except(ToListOfPoints()).ToList();
        }
        private Point GetAvailablePosition()
        {
            List<Point> availablePoints = GetAvailablePositions();
            return availablePoints.ElementAt(random.Next(0, availablePoints.Count));
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
        public override string ToString()
        {
            return $"Score: {Score}, Actions: {Actions}, Highest tile: {HighestTile}, Value: {Value}";
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
