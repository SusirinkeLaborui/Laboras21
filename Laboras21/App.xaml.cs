using Laboras21.Classes;
using Laboras21.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
                    StyledMessageDialog.Show(text, title);
                });
        }
    }
}
