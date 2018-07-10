﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tile2048
{
    public class Tile : INotifyPropertyChanged
    {
        private int number;
        private int row;
        private int column;

        public int Number
        {
            get => number;
            set
            {
                number = value;
                RaisePropertyChanged();
            }
        }
        public int Row
        {
            get => row;
            set
            {
                row = value;
                RaisePropertyChanged();
            }
        }
        public int Column
        {
            get => column;
            set
            {
                column = value;
                RaisePropertyChanged();
            }
        }

        public Tile(int number, int row, int column)
        {
            Number = number;
            Row = row;
            Column = column;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
