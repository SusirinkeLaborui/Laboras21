using Laboras21.Classes;
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
        static StyledMessageDialog()
        {
            PInvoke.SetMessageBoxCallback((title, text) =>
                {
                    Show(text, title);
                });
        }

        public StyledMessageDialog(string message, string title, MessageBoxButton buttons)
        {
            InitializeComponent();

            Title = title;
            messageTextBlock.Text = message;

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    VisualStateManager.GoToElementState(this.LayoutRoot, "StateOK", false);
                    break;
                case MessageBoxButton.YesNo:
                    VisualStateManager.GoToElementState(this.LayoutRoot, "StateYesNo", false);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        public static bool? Show(string message, string title, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            var dialog = new StyledMessageDialog(message, title, buttons);
            return dialog.ShowDialog();
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
