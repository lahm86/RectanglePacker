using RectanglePackerWindow.Utilities;
using System.Windows;

namespace RectanglePackerWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            WindowUtils.SetMenuAlignment();
        }
    }
}