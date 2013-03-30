using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Director
{
    public struct Coord{
        public uint X;
        public uint Y;
        public Coord(uint x, uint y)
        {
            X = x;
            Y = y;
        }
    }
    public struct MineLocation
    {
        public Coord N1;
        public Coord N2;
        public MineLocation(Coord n1, Coord n2)
        {
            N1 = n1;
            N2 = n2;
        }
    }
    public class Databindings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int MineCount
        {
            get { return Data.mines.Count; }
            set
            {
                //name = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("MineCount");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    public static class Data 
    {
        public static List<MineLocation> mines = new List<MineLocation>();
        public static Databindings db = new Databindings();
        public static string ComPort = "COM1";
        public static uint BaudRate = 9600;
        public static readonly DependencyProperty MineLocationProperty = DependencyProperty.RegisterAttached(
          "MineLocation",
          typeof(MineLocation),
          typeof(Data),
          new FrameworkPropertyMetadata(new MineLocation(new Coord(0,0), new Coord(0,1)), FrameworkPropertyMetadataOptions.AffectsRender)
        );
        public static void SetMineLocation(UIElement element, MineLocation value)
        {
            element.SetValue(MineLocationProperty, value);
        }
        public static MineLocation GetMineLocation(UIElement element)
        {
            return (MineLocation)element.GetValue(MineLocationProperty);
        }
        
    }
}