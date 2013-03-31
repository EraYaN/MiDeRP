using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

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
    public struct NodeConnection
    {
        public Coord N1;
        public Coord N2;
        public NodeConnection(Coord n1, Coord n2)
        {
            N1 = n1;
            N2 = n2;
        }
    }
    public class Databindings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string MineCount
        {
            get
            {
                if (Data.nav!=null)
                {
                    return Data.nav.mines.Count.ToString();
                }
                else
                {
                    return "??";
                }
            }
        }
        public string SerialPortStatus
        {
            get
            {
                if (Data.com == null)
                    return "NULL";
                if (Data.com.IsOpen)
                {
                    return Data.com.BytesInRBuffer + "|" + Data.com.BytesInTBuffer;
                }
                else
                {
                    return "NC";
                }
            }
        }
        public Brush SerialPortStatusColor
        {
            get
            {
                if(Data.com==null)
                    return Brushes.Red; ;
                if (Data.com.IsOpen)
                {
                    int b = Data.com.BytesInRBuffer + Data.com.BytesInTBuffer;
                    if (b==0)
                    {
                        return Brushes.Green;
                    }
                    else if (b > 0&&b<=2)
                    {
                        return Brushes.LightGreen;
                    }
                    else
                    {
                        return Brushes.Orange;
                    }                    
                }
                else
                {
                    return Brushes.OrangeRed;
                }
            }            
        }
        public void UpdateProperty(string name)
        {
            OnPropertyChanged(name);
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
        
        public static Databindings db = new Databindings();
        public static string ComPort = "COM1";
        public static int BaudRate = 9600;
        static public Visualization vis;
        static public Navigation nav;
        static public SerialInterface com;
        public static readonly DependencyProperty MineLocationProperty = DependencyProperty.RegisterAttached(
          "MineLocation",
          typeof(NodeConnection),
          typeof(Data),
          new FrameworkPropertyMetadata(new NodeConnection(new Coord(0,0), new Coord(0,1)), FrameworkPropertyMetadataOptions.AffectsRender)
        );
        public static void SetMineLocation(UIElement element, NodeConnection value)
        {
            element.SetValue(MineLocationProperty, value);
        }
        public static NodeConnection GetMineLocation(UIElement element)
        {
            return (NodeConnection)element.GetValue(MineLocationProperty);
        }
        
    }
}