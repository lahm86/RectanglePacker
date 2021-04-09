using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RectanglePackerWindow.Commands
{
    public static class WindowCommands
    {
        static WindowCommands()
        {
            GenerateRectangles.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Control));
            ImportRectangles.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            ImportImages.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
            Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            Exit.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));

            Pack.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            Reset.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift));

            GitHub.InputGestures.Add(new KeyGesture(Key.F1));
        }

        public static readonly RoutedUICommand GenerateRectangles = new RoutedUICommand();
        public static readonly RoutedUICommand ImportRectangles = new RoutedUICommand();
        public static readonly RoutedUICommand ImportImages = new RoutedUICommand();
        public static readonly RoutedUICommand Exit = new RoutedUICommand();

        public static readonly RoutedUICommand Pack = new RoutedUICommand();
        public static readonly RoutedUICommand Reset = new RoutedUICommand();
        public static readonly RoutedUICommand Save = new RoutedUICommand();

        public static readonly RoutedUICommand GitHub = new RoutedUICommand();
        public static readonly RoutedUICommand CheckForUpdate = new RoutedUICommand();
        public static readonly RoutedUICommand ShowUpdate = new RoutedUICommand();
        public static readonly RoutedUICommand About = new RoutedUICommand();
    }
}
