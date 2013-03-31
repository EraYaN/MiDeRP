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
        uint m; //x-size
        uint n; //y-size
        double marginlarge = 80;
        double marginsmall = 30;
        double mineradius = 8;
        double controlpointsize = 20;
        public Visualization(Canvas _c, uint _m, uint _n)
        {
            //Constructor
            c = _c;
            m = _m;
            n = _n;
        }
        ~Visualization()
        {
            //c.Children.Clear(); kan niet. verkeerde thread
        }
        private void DrawMines(uint xlim, uint ylim, double xstep, double ystep)
        {
            for (uint x = 0; x < xlim; x++)
            {
                for (uint y = 0; y < ylim; y++)
                {
                    //Add mines (circles) on horizontal lines
                    Ellipse mine = new Ellipse();
                    Coord N1 = new Coord();
                    Coord N2 = new Coord();
                    NodeConnection ml = new NodeConnection();
                    NodeConnection ml2 = new NodeConnection();
                    if (xlim == m && ylim != n)
                    {
                        N1.X = x;
                        N1.Y = y;
                        N2.X = x;
                        N2.Y = y + 1;
                    }
                    else
                    {
                        N1.X = x;
                        N1.Y = y;
                        N2.X = x+1;
                        N2.Y = y;
                    }
                    ml.N1 = N1;
                    ml.N2 = N2;
                    ml2.N1 = N2;
                    ml2.N2 = N1;
                    if (Data.nav.mines.Contains(ml) || Data.nav.mines.Contains(ml2)) //method the takes position and returns true if there is a mine
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
                    mine.MouseUp += mine_MouseUp;
                    Data.SetMineLocation(mine, ml);
                    if (xlim == m&&ylim!=n)
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x) - mineradius / 2);
                        Canvas.SetBottom(mine, marginlarge + ystep * (y + 0.5) - mineradius / 2);
                    }
                    else if (ylim==n&&xlim!=m)
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x + 0.5) - mineradius / 2);
                        Canvas.SetBottom(mine, marginlarge + ystep * (y) - mineradius / 2);
                    }
                    else
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x + 0.5) - mineradius / 2);
                        Canvas.SetBottom(mine, marginlarge + ystep * (y + 0.5) - mineradius / 2);
                    }
                }
            }
        }
        private void DrawControlpoints(double xstep, double ystep)
        {
            for (int x = 1; x < m-1; x++)
            {
                //Create object and set parameters
                Border cpt = new Border(); //top
                Border cpb = new Border(); //bottom
                TextBlock cptt = new TextBlock(); //top
                TextBlock cpbt = new TextBlock(); //bottom                
                cpb.Width = cpb.Height = controlpointsize;
                cpt.Width = cpt.Height = controlpointsize;
                cpb.Background = cpt.Background = Brushes.Black;
                cpbt.FontSize = cptt.FontSize = controlpointsize - 4;
                cpbt.Foreground = cptt.Foreground = Brushes.White;
                cpbt.Text = x.ToString();
                cptt.Text = ((2*(m-2)+(n-2))-(x-1)).ToString();
                cpbt.HorizontalAlignment = cptt.HorizontalAlignment = HorizontalAlignment.Center;
                cpbt.VerticalAlignment = cptt.VerticalAlignment = VerticalAlignment.Center;
                Canvas.SetLeft(cpb, marginlarge + xstep * x - controlpointsize / 2);
                Canvas.SetBottom(cpb, marginsmall - controlpointsize / 2);               
                Canvas.SetLeft(cpt, marginlarge + xstep * x - controlpointsize / 2);
                Canvas.SetBottom(cpt, c.ActualHeight - marginsmall - controlpointsize / 2);
                cpb.Child = cpbt;
                cpt.Child = cptt;
                cpb.MouseUp += cp_MouseUp;
                cpt.MouseUp += cp_MouseUp;
                c.Children.Add(cpb);
                c.Children.Add(cpt);
            }
            //add horizontal lines
            for (int y = 1; y < n-1; y++)
            {
                //Create object and set parameters
                Border cpl = new Border(); //top
                Border cpr = new Border(); //bottom
                TextBlock cplt = new TextBlock(); //top
                TextBlock cprt = new TextBlock(); //bottom                
                cpr.Width = cpr.Height = controlpointsize;
                cpl.Width = cpl.Height = controlpointsize;
                cpr.Background = cpl.Background = Brushes.Black;
                cprt.FontSize = cplt.FontSize = controlpointsize - 4;
                cprt.Foreground = cplt.Foreground = Brushes.White;
                cprt.Text = ((m - 2) + y).ToString();
                cplt.Text = ((2 * (m - 2) + 2 * (n - 2)) - (y - 1)).ToString();
                cprt.HorizontalAlignment = cplt.HorizontalAlignment = HorizontalAlignment.Center;
                cprt.VerticalAlignment = cplt.VerticalAlignment = VerticalAlignment.Center;
                Canvas.SetLeft(cpr, c.ActualWidth - marginsmall - controlpointsize / 2);
                Canvas.SetBottom(cpr, marginlarge + ystep * (y) - controlpointsize / 2);
                Canvas.SetLeft(cpl, marginsmall - controlpointsize / 2);
                Canvas.SetBottom(cpl, marginlarge + ystep * (y) - controlpointsize / 2);
                cpr.Child = cprt;
                cpl.Child = cplt;
                cpr.MouseUp += cp_MouseUp;
                cpl.MouseUp += cp_MouseUp;
                c.Children.Add(cpr);
                c.Children.Add(cpl);
            }
        }

        void cp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border cp = (Border)sender;
            int id = Convert.ToInt32(((TextBlock)cp.Child).Text);
            MessageBox.Show("CP #"+id+" clicked");
        }
        void mine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse circle = (Ellipse)e.OriginalSource;
            NodeConnection ml = Data.GetMineLocation(circle);
            NodeConnection ml2 = new NodeConnection(ml.N2, ml.N1);
            if (Data.nav.mines.Contains(ml))
            {
                Data.nav.mines.Remove(ml);
            }
            else if (Data.nav.mines.Contains(ml))
            {
                Data.nav.mines.Remove(ml2);
            }
            else
            {
                Data.nav.mines.Add(ml);
            }

            Data.vis.DrawField();
            Data.db.UpdateProperty("MineCount");
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
            DrawControlpoints(xstep, ystep);
        }
    }
}
