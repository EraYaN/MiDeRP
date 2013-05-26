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
using System.Windows.Threading;

namespace MiDeRP
{
    public class Visualization
    {
        Canvas c;
        const double marginlarge = 80;
        const double marginsmall = 30;
        const double mineradius = 16;
        const double controlpointsize = 20;
        const double pathArrowHeadHeight = 2;
        const double pathArrowHeadWidth = 10;
        const double pathArrowThickness = 3; 
        const double currentPosArrowHeadHeight = 8;
        const double currentPosArrowHeadWidth = 10;
        const double currentPosArrowThickness = 5;
        const double currentPosArrowLength = 0.5; // piece of xstep of ystep; true length = this * xstep or ystep
        const double lineThickness = 1;
        readonly Brush lineBrush = Brushes.Black;
        readonly Brush noMineBrush = Brushes.Black;
        readonly Brush entryBrush = Brushes.Green;
        readonly Brush mineBrush = Brushes.Red;
        readonly Brush pathBrush = Brushes.BlueViolet;
        readonly Brush currentPosBrush = Brushes.ForestGreen;
        readonly Brush exitBrush = Brushes.Blue;

        delegate void DrawFieldDelegate();
        DrawFieldDelegate drawField; 

        public Visualization(Canvas _c)
        {
            //Constructor
            c = _c;
            drawField = DrawField;
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
                    if (xlim == Data.M && ylim != Data.N)
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
                    ml.To = N1;
                    ml.From = N2;
                    ml2.To = N2;
                    ml2.From = N1;
                    if (Data.nav.mines.Contains(ml) || Data.nav.mines.Contains(ml2)) //method the takes position and returns true if there is a mine
                    {
                        //mine
                        mine.Fill = mineBrush;
                    }
                    else
                    {
                        //no mine
                        mine.Fill = noMineBrush;
                    }
                    mine.Height = mineradius;
                    mine.Width = mineradius;
                    c.Children.Add(mine);
                    mine.MouseUp += mine_MouseUp;
                    Data.SetMineLocation(mine, ml);
                    if (xlim == Data.M&&ylim!=Data.N)
                    {
                        Canvas.SetLeft(mine, marginlarge + xstep * (x) - mineradius / 2);
                        Canvas.SetBottom(mine, marginlarge + ystep * (y + 0.5) - mineradius / 2);
                    }
                    else if (ylim==Data.N&&xlim!=Data.M)
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
            for (int x = 1; x < Data.M - 1; x++)
            {
                //Create object and set parameters
                Border cpt = new Border(); //top
                Border cpb = new Border(); //bottom
                TextBlock cptt = new TextBlock(); //top
                TextBlock cpbt = new TextBlock(); //bottom                
                cpb.Width = cpb.Height = controlpointsize;
                cpt.Width = cpt.Height = controlpointsize;
                cpb.Background = cpt.Background = Brushes.Black;
                if ((2 * (Data.M - 2) + (Data.N - 2)) - (x - 1) == Data.entryCP)
                {
                    cpt.Background = entryBrush;
                }
				else if (Data.nav.targetCPs.Contains((uint)((2 * (Data.M - 2) + (Data.N - 2)) - (x - 1))) || (2 * (Data.M - 2) + (Data.N - 2)) - (x - 1) == Data.exitCP)
                {
                    cpt.Background = exitBrush;
                }
                if (x == Data.entryCP)
                {
                    cpb.Background = entryBrush;
                }
                else if (x == Data.exitCP)
                {
                    cpb.Background = exitBrush;
                }
                cpbt.FontSize = cptt.FontSize = controlpointsize - 4;
                cpbt.Foreground = cptt.Foreground = Brushes.White;
                cpbt.Text = x.ToString();
                cptt.Text = ((2 * (Data.M - 2) + (Data.N - 2)) - (x - 1)).ToString();
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
            for (int y = 1; y < Data.N - 1; y++)
            {
                //Create object and set parameters
                Border cpl = new Border(); //top
                Border cpr = new Border(); //bottom
                TextBlock cplt = new TextBlock(); //top
                TextBlock cprt = new TextBlock(); //bottom                
                cpr.Width = cpr.Height = controlpointsize;
                cpl.Width = cpl.Height = controlpointsize;
                cpr.Background = cpl.Background = Brushes.Black;
                if ((Data.M - 2) + y == Data.entryCP)
                {
                    cpr.Background = entryBrush;
                }
                else if ((Data.M - 2) + y == Data.exitCP)
                {
                    cpr.Background = exitBrush;
                }
                if ((2 * (Data.M - 2) + 2 * (Data.N - 2)) - (y - 1) == Data.entryCP)
                {
                    cpl.Background = entryBrush;
                }
                else if ((2 * (Data.M - 2) + 2 * (Data.N - 2)) - (y - 1) == Data.exitCP)
                {
                    cpl.Background = exitBrush;
                }
                cprt.FontSize = cplt.FontSize = controlpointsize - 4;
                cprt.Foreground = cplt.Foreground = Brushes.White;
                cprt.Text = ((Data.M - 2) + y).ToString();
                cplt.Text = ((2 * (Data.M - 2) + 2 * (Data.N - 2)) - (y - 1)).ToString();
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

        private void DrawPath(double xstep, double ystep)
        {            
            if (Data.nav.path.Count>0)
            {
                foreach (NodeConnection nc in Data.nav.path)
                {
                    Arrow arr = new Arrow();
                    Point N1, N2;
                    N1 = getCanvasCoordinates(nc.From, xstep, ystep);
                    N2 = getCanvasCoordinates(nc.To, xstep, ystep);
                    double CPExtension = marginlarge - marginsmall-controlpointsize/2-1;
                    if (nc.ToCPoint)
                    {
                        if (nc.To.X == Data.M-1)
                        {
                            N2.X += CPExtension;
                        }
                        else if (nc.To.X == 0)
                        {
                            N2.X -= CPExtension;
                        }
                        else if (nc.To.Y == 0)
                        {
                            N2.Y -= CPExtension;
                        }
                        else if (nc.To.Y == Data.N-1)
                        {
                            N2.Y += CPExtension;
                        }
                    }
                    else if (nc.FromCPoint)
                    {
                        if (nc.From.X == Data.M-1)
                        {
                            N1.X += CPExtension;
                        }
                        else if (nc.From.X == 0)
                        {
                            N1.X -= CPExtension;
                        }
                        else if (nc.From.Y == 0)
                        {
                            N1.Y -= CPExtension;
                        }
                        else if (nc.From.Y == Data.N-1)
                        {
                            N1.Y += CPExtension;
                        }                        
                    }
                    
                    arr.X1 = N1.X;
                    arr.X2 = N2.X;
                    arr.Y1 = c.ActualHeight - N1.Y;
                    arr.Y2 = c.ActualHeight - N2.Y;
                    arr.Stroke = pathBrush;
                    arr.HeadHeight = pathArrowHeadHeight;
                    arr.HeadWidth = pathArrowHeadWidth;
                    arr.StrokeThickness = pathArrowThickness;
                    arr.IsHitTestVisible = false;
                    c.Children.Add(arr);
                }
            }            
        }

        private void DrawCurrentPosition(double xstep, double ystep)
        {
            if (Data.nav.currentPos.To != Data.nav.currentPos.From || (Data.nav.currentPos.ToCPoint || Data.nav.currentPos.FromCPoint))
            {
                Arrow arr = new Arrow();
                Point N1 = getCanvasCoordinates(Data.nav.currentPos.From, xstep, ystep);
                Point N2 = getCanvasCoordinates(Data.nav.currentPos.To, xstep, ystep);
                double CPExtension = (marginlarge - marginsmall - controlpointsize / 2 -1) ;
                if (Data.nav.currentPos.ToCPoint)
                {
                    if (Data.nav.currentPos.To.X == Data.M - 1)
                    {
                        N2.X += CPExtension;                        
                    }
                    else if (Data.nav.currentPos.To.X == 0)
                    {
                        N2.X -= CPExtension;                        
                    }
                    else if (Data.nav.currentPos.To.Y == 0)
                    {
                        N2.Y -= CPExtension;
                    }
                    else if (Data.nav.currentPos.To.Y == Data.N - 1)
                    {
                        N2.Y += CPExtension;
                    }
                    arr.X1 = N1.X;
                    arr.X2 = N2.X;
                    arr.Y1 = c.ActualHeight - N1.Y;
                    arr.Y2 = c.ActualHeight - N2.Y;
                }
                else if (Data.nav.currentPos.FromCPoint)
                {
                    if (Data.nav.currentPos.From.X == Data.M - 1)
                    {
                        N1.X += CPExtension;
                    }
                    else if (Data.nav.currentPos.From.X == 0)
                    {
                        N1.X -= CPExtension;
                    }
                    else if (Data.nav.currentPos.From.Y == 0)
                    {
                        N1.Y -= CPExtension;
                    }
                    else if (Data.nav.currentPos.From.Y == Data.N - 1)
                    {
                        N1.Y += CPExtension;
                    }
                    arr.X1 = N1.X;
                    arr.X2 = N2.X;
                    arr.Y1 = c.ActualHeight - N1.Y;
                    arr.Y2 = c.ActualHeight - N2.Y;
                }
                else
                {
                    if (Data.nav.currentPos.From.X == Data.nav.currentPos.To.X)
                    {
                        arr.X1 = N1.X;
                        arr.X2 = N2.X;
                        if (N1.Y > N2.Y)
                        {
                            arr.Y1 = c.ActualHeight - N1.Y + ystep * currentPosArrowLength / 2;
                            arr.Y2 = c.ActualHeight - N2.Y - ystep * currentPosArrowLength / 2;
                        }
                        else
                        {
                            arr.Y1 = c.ActualHeight - N1.Y - ystep * currentPosArrowLength / 2;
                            arr.Y2 = c.ActualHeight - N2.Y + ystep * currentPosArrowLength / 2;
                        }
                    }
                    else if (Data.nav.currentPos.From.Y == Data.nav.currentPos.To.Y)
                    {

                        if (N1.X < N2.X)
                        {
                            arr.X1 = N1.X + xstep * currentPosArrowLength / 2;
                            arr.X2 = N2.X - xstep * currentPosArrowLength / 2;
                        }
                        else
                        {
                            arr.X1 = N1.X - xstep * currentPosArrowLength / 2;
                            arr.X2 = N2.X + xstep * currentPosArrowLength / 2;
                        }
                        arr.Y1 = c.ActualHeight - N1.Y;
                        arr.Y2 = c.ActualHeight - N2.Y;
                    }
                }
                
                arr.Stroke = currentPosBrush;
                arr.HeadHeight = currentPosArrowHeadHeight;
                arr.HeadWidth = currentPosArrowHeadWidth;
                arr.StrokeThickness = currentPosArrowThickness;
                arr.IsHitTestVisible = false;
                c.Children.Add(arr);                
            }
        }

        private Point getCanvasCoordinates(Coord node, double xstep, double ystep)
        {
            Point p = new Point();
            p.X = marginlarge + xstep * (node.X);
            p.Y = marginlarge + ystep * (node.Y);
            return p;
        }

        void cp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border cp = (Border)sender;
            uint id = Convert.ToUInt32(((TextBlock)cp.Child).Text); 
            if (e.ChangedButton == MouseButton.Left)
            {
                //entry
                //MessageBox.Show("CP #" + id + " clicked.\n Made entry.");
                Data.entryCP = id;
            } else if(e.ChangedButton == MouseButton.Right){
                //exit
                //MessageBox.Show("CP #" + id + " clicked.\n Made exit.");
                Data.exitCP = id;
				Data.nav.updateCP(id);
            }            
            Data.vis.DrawField();
        }

        void mine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse circle = (Ellipse)e.OriginalSource;
            NodeConnection ml = Data.GetMineLocation(circle);
            NodeConnection ml2 = new NodeConnection(ml.From, ml.To);
            if (e.ChangedButton == MouseButton.Left)
            {                
                if (Data.nav.mines.Contains(ml))
                {
                    Data.nav.mines.Remove(ml);
                }
                else
                {
                    Data.nav.mines.Add(ml);
                }
                Data.db.UpdateProperty("MineCount");
            }
            else if(e.ChangedButton == MouseButton.Right)
            {
                if (Data.nav.currentPos == ml)
                {
                    Data.nav.currentPos = ml2;
                }
                else
                {
                    Data.nav.currentPos = ml;
                }
                Data.db.UpdateProperty("CurrentPosText");
            }
            Data.vis.DrawField();
           
        }

        public void DrawField()
        {
            if (c.Dispatcher.CheckAccess())
            {
                //we're on the main thread that means GO.
                //remove all objects
                c.Children.Clear();
                //add vertical lines
                double xstep = ((c.ActualWidth - marginlarge * 2) / (Data.M - 1)); //spacing between vertical lines
                double ystep = ((c.ActualHeight - marginlarge * 2) / (Data.N - 1)); //spacing between horizontal lines
                for (int x = 0; x < Data.M; x++)
                {
                    //Create line object and set parameters.
                    Line line = new Line();
                    line.Stroke = lineBrush;
                    line.StrokeThickness = lineThickness;
                    line.X1 = marginlarge + xstep * x;
                    if (x == 0 || x == Data.M - 1)
                    {
                        //shorter
                        line.Y1 = c.ActualHeight - (marginlarge - 0.5); //minus 0.5 to close gap
                        line.Y2 = marginlarge;
                    }
                    else
                    {
                        //longer
                        line.Y1 = c.ActualHeight - marginsmall;
                        line.Y2 = marginsmall;
                    }
                    line.X2 = line.X1;

                    //Add to canvas
                    c.Children.Add(line);
                }
                //add horizontal lines
                for (int y = 0; y < Data.N; y++)
                {
                    //Create line object and set parameters.
                    Line line = new Line();
                    line.Stroke = lineBrush;
                    line.StrokeThickness = lineThickness;
                    line.Y1 = c.ActualHeight - (marginlarge + ystep * y);
                    if (y == 0 || y == Data.N - 1)
                    {
                        //shorter
                        line.X1 = marginlarge - 0.5;
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
                DrawMines(Data.M - 1, Data.N, xstep, ystep);
                DrawMines(Data.M, Data.N - 1, xstep, ystep);
                DrawControlpoints(xstep, ystep);
                DrawPath(xstep, ystep);
                DrawCurrentPosition(xstep, ystep);
            }
            else
            {
                c.Dispatcher.Invoke(drawField, DispatcherPriority.Normal);
            }
        }
    }
}
