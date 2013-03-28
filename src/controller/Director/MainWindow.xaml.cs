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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        public MainWindow()
        {
            InitializeComponent();
            //TODO make test draw method.
        }

        private void TestDrawButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Draw to Canvas "fieldmap"
            Line line = new Line();
            line.Stroke = Brushes.Black;
            line.X1 = rand.Next(1, (int)Math.Floor(fieldmap.ActualWidth));
            line.Y1 = rand.Next(1, (int)Math.Floor(fieldmap.ActualHeight));
            line.X2 = rand.Next(1, (int)Math.Floor(fieldmap.ActualWidth));
            line.Y2 = rand.Next(1, (int)Math.Floor(fieldmap.ActualHeight));
            fieldmap.Children.Add(line);
        }
    }
}
