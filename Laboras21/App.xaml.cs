using Laboras21.Classes;
using Laboras21.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Laboras21
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            PInvoke.SetMessageBoxCallback((title, text) =>
            {
                Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    StyledMessageDialog.Show(text, title);
                });
            });
        }
    }
}
