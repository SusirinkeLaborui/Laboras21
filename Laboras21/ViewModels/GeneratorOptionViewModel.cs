﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Laboras21.ViewModels
{
    class GeneratorOptionViewModel : INotifyPropertyChanged
    {
        private int n;
        private int minX;
        private int maxX;
        private int minY;
        private int maxY;
        private double standardDeviation;

        public int NumberOfPoints
        {
            get { return n; }
            set { n = value; NotifyPropertyChanged(); }
        }

        public int MinX
        {
            get { return minX; }
            set { minX = value; NotifyPropertyChanged(); }
        }

        public int MaxX
        {
            get { return maxX; }
            set { maxX = value; NotifyPropertyChanged(); }
        }

        public int MinY
        {
            get { return minY; }
            set { minY = value; NotifyPropertyChanged(); }
        }

        public int MaxY
        {
            get { return maxY; }
            set { maxY = value; NotifyPropertyChanged(); }
        }

        public double StandardDeviation
        {
            get { return standardDeviation; }
            set { standardDeviation = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
