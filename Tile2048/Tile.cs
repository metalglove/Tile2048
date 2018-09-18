using System;
using System.ComponentModel;
using System.Media;
using System.Runtime.CompilerServices;

namespace Tile2048
{
    public class Tile : INotifyPropertyChanged, ICloneable
    {
        private int number;
        private int row;
        private int column;

        public int Number
        {
            get => number;
            set
            {
                //new SoundPlayer(Properties.Resources.Grow).Play();
                number = value;
                RaisePropertyChanged();
            }
        }
        public int Row
        {
            get => row;
            set
            {
                if (row != value)
                {
                    row = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int Column
        {
            get => column;
            set
            {
                if(column != value)
                {
                    column = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Tile(int number, int row, int column)
        {
            this.number = number;
            RaisePropertyChanged("Number");
            Row = row;
            Column = column;
        }

        public object Clone()
        {
            return new Tile(number, row, column);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
