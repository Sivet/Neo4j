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

namespace Neo4j_VisualTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Field field;
        Row row;
        Herbicide herbicide;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            field = new Field(Field_textBox.Text);
            row = new Row(int.Parse(Row_textBox.Text));
            herbicide = new Herbicide(Herbicide_textBox.Text);

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
