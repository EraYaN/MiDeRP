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
        const int M = 5;
        const int N = 5;        
        //Random rand = new Random();
        Visualization vis;
        Navigation nav;
        Communications com;
  
        public MainWindow()
        {
            InitializeComponent();            
            
        }

        private void TestDrawButton_Click(object sender, RoutedEventArgs e)
        {
            vis.DrawField();            
        }

        private void findPathButton_Click(object sender, RoutedEventArgs e)
        {            
            Navigation.loopNavigation(0,M*N-1);
        }

        private void startInitButton_Click(object sender, RoutedEventArgs e)
        {
            //Init classes
            vis = new Visualization(fieldmapcanvas, M, N);
            nav = new Navigation(M, N);
            com = new Communications("COM1", 9600);
            //enable buttons
            findPathButton.IsEnabled = true;
            TestDrawButton.IsEnabled = true;
            startInitButton.IsEnabled = false;
            destroyButton.IsEnabled = true;
        }

        private void destroyButton_Click(object sender, RoutedEventArgs e)
        {
            findPathButton.IsEnabled = false;
            TestDrawButton.IsEnabled = false;
            startInitButton.IsEnabled = true;
            destroyButton.IsEnabled = false;
            nav = null;
            com = null;
            vis = null;
        }

        private void minCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse circle = (Ellipse)e.OriginalSource;
            vis.DrawField();
        }
                
    }
}
