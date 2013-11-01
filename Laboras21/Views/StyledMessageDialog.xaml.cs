using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace Laboras21.Views
{
    /// <summary>
    /// Interaction logic for StyledMessageDialog.xaml
    /// </summary>
    public partial class StyledMessageDialog : MetroWindow
    {
        public StyledMessageDialog(string message, string title)
        {
            InitializeComponent();

            Title = title;
            messageTextBlock.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        public static void Show(string message, string title)
        {
            var dialog = new StyledMessageDialog(message, title);
            dialog.ShowDialog();
        }
    }
}
