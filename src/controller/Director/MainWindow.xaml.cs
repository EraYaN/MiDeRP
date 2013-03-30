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
            //Init draw class
            vis = new Visualization(fieldmapcanvas, M, N);
            nav = new Navigation(M,N);
        }

        private void TestDrawButton_Click(object sender, RoutedEventArgs e)
        {
            vis.DrawField();            
        }

        private void findPathButton_Click(object sender, RoutedEventArgs e)
        {            
            Navigation.loopNavigation(0,M*N-1);
        }
    }
}
