﻿using System;
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
        public Coord(uint controlPost)
        {
            const uint hori = Data.M - 2; //controlPosts per horizontale zijde van het veld
            const uint vert = Data.N - 2; //controlPosts per verticale zijde van het veld
            if (controlPost > Data.numControlPosts || controlPost < 1)
            {
                throw new ArgumentException("Controlpost is not valid.");
            }
            if (controlPost <= hori)
            {
                //onder
                X = controlPost;
                Y = 0;
            }
            else if (controlPost > hori && controlPost <= hori + vert)
            {
                //rechts
                X = Data.M - 1;
                Y = controlPost - hori;
            }
            else if (controlPost > hori + vert && controlPost <= 2 * hori + vert)
            {
                //boven
                X = (Data.M - 1) - (controlPost - hori - vert);
                Y = Data.N - 1;
            }
            else if (controlPost > 2 * hori + vert && controlPost <= 2 * hori + 2 * vert)
            {
                //links
                X = 0;
                Y = (Data.N - 1) - (controlPost - 2 * hori - vert);
            }
            else
            {
                //bestaat niet!
                throw new Exception("Controlpost is not valid.");
            }        
        }
        public uint Id
        {
            get { return Data.M * Y + X; }
            set { X = value % Data.M; Y = (uint)Math.Floor((decimal)value / Data.M); }
        }
    }
    public struct NodeConnection
    {
        public Coord To;
        public Coord From;
        public NodeConnection(Coord to, Coord from)
        {
            To = to;
            From = from;
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
        public const uint M = 15;
        public const uint N = 10;
        public const uint numNodes = M*N;
        public const uint numControlPosts = 2 * (M - 2) + 2 * (N - 2);
        public static uint entryCP = 1;
        public static uint exitCP = (M-2)+(N-2);
        public static NodeConnection currentPos;
        public static List<NodeConnection> path = new List<NodeConnection>();
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