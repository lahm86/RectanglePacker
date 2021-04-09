using RectanglePackerWindow.Model;
using RectanglePackerWindow.Utilities;
using System.Windows;

namespace RectanglePackerWindow.Windows
{
    /// <summary>
    /// Interaction logic for RectangleGeneratorWindow.xaml
    /// </summary>
    public partial class RectangleGeneratorWindow : Window
    {
        public UIRectangleProvider RectangleProvider { get; private set; }

        public RectangleGeneratorWindow()
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.TidyMenu(this);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            RectangleProvider = new UIRectangleProvider();
            RectangleProvider.LoadRectangles(new RandomSizeGenerator
            {
                Count = _countSpinner.Value,
                MinimimWidth = _minWidthSpinner.Value,
                MaximimWidth = _maxWidthSpinner.Value,
                MinimimHeight = _minHeightSpinner.Value,
                MaximimHeight = _maxHeightSpinner.Value,
                SquaresOnly = _sqauresCheck.IsChecked ?? false
            }.Generate());
            DialogResult = true;
        }
    }
}