using System;
using System.Collections.Generic;
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
    public static class Data
    {
        public static List<MineLocation> mines = new List<MineLocation>();

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