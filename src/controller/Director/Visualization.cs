using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Director
{
    public class Visualization
    {
        Canvas c;
        int m; //x-size
        int n; //y-size
        double marginlarge = 80;
        double marginsmall = 30;
        double mineradius = 6;
        public Visualization(Canvas _c, int _m, int _n)
        {
            //Constructor
            c = _c;
            m = _m;
            n = _n;
        }
        private void DrawMines(int xlim, int ylim, double xstep, double ystep)
        {
            for (int x = 0; x < xlim; x++)
            {
                for (int y = 0; y < ylim; y++)
                {
                    //Add mines (circles) on horizontal lines
                    Ellipse mine = new Ellipse();
                    if (xlim == m&&ylim!=n) //method the takes position and returns true if there is a mine
                    {
                        //mine
                        mine.Fill = Brushes.Red;
                    }
                    else
                    {
                        //no mine
                        mine.Fill = Brushes.Black;
                    }
                    mine.Height = mineradius;
                    mine.Width = mineradius;
                    c.Children.Add(mine);
                    if (xlim == m&&ylim!=n)
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x) - mineradius / 2);
                        Canvas.SetTop(mine, marginlarge + ystep * (y + 0.5) - mineradius / 2);
                    }
                    else if (ylim==n&&xlim!=m)
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x + 0.5) - mineradius / 2);
                        Canvas.SetTop(mine, marginlarge + ystep * (y) - mineradius / 2);
                    }
                    else
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x + 0.5) - mineradius / 2);
                        Canvas.SetTop(mine, marginlarge + ystep * (y + 0.5) - mineradius / 2);
                    }
                }
            }
        }
        public void DrawField()
        {
            //remove all objects
            c.Children.Clear();
            //add vertical lines
            double xstep = ((c.ActualWidth - marginlarge * 2) / (m - 1)); //spacing between vertical lines
            double ystep = ((c.ActualHeight - marginlarge * 2) / (n - 1)); //spacing between horizontal lines
            for (int x = 0; x < m; x++)
            {
                //Create line object and set parameters.
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = marginlarge + xstep * x;
                if (x == 0 || x == m - 1)
                {
                    //shorter
                    line.Y1 = marginlarge-0.5; //minus 0.5 to close gap
                    line.Y2 = c.ActualHeight - marginlarge;
                }
                else
                {
                    //longer
                    line.Y1 = marginsmall;
                    line.Y2 = c.ActualHeight - marginsmall;
                }
                line.X2 = line.X1;

                //Add to canvas
                c.Children.Add(line);
            }
            //add horizontal lines
            for (int y = 0; y < n; y++)
            {
                //Create line object and set parameters.
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.Y1 = marginlarge + ystep * y;
                if (y == 0 || y == n - 1)
                {
                    //shorter
                    line.X1 = marginlarge-0.5;
                    line.X2 = c.ActualWidth - marginlarge;
                }
                else
                {
                    //longer
                    line.X1 = marginsmall;
                    line.X2 = c.ActualWidth - marginsmall;
                }
                line.Y2 = line.Y1;

                //Add to canvas
                c.Children.Add(line);
            }
            DrawMines(m - 1, n, xstep, ystep);
            DrawMines(m, n - 1, xstep, ystep);           
        }
    }
}
