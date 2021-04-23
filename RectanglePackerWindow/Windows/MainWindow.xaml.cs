using Microsoft.WindowsAPICodePack.Dialogs;
using RectanglePacker;
using RectanglePacker.Organisation;
using RectanglePackerWindow.Model;
using RectanglePackerWindow.Updates;
using RectanglePackerWindow.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace RectanglePackerWindow.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRectangleProvider _rectangleProvider;
        private Dictionary<int, Canvas> _tileMap;

        public MainWindow()
        {
            InitializeComponent();
            UpdateChecker.Instance.UpdateAvailable += UpdateChecker_UpdateAvailable;

            MinWidth = 850;
            MinHeight = 450;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"SampleData\SampleSizes.json"))
            {
                ImportRectangles(@"SampleData\SampleSizes.json");
            }
        }

        private void ImportRectangles(string filePath)
        {
            try
            {
                UIRectangleProvider provider = new UIRectangleProvider();
                provider.LoadRectangles(filePath, true);
                SetRectangleProvider(provider);
            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        private void ImportImages(List<string> filePaths)
        {
            try
            {
                UIGraphicProvider provider = new UIGraphicProvider();
                provider.LoadRectangles(filePaths, true);
                SetRectangleProvider(provider);
            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        private void SetRectangleProvider(IRectangleProvider provider)
        {
            _rectangleProvider = provider;
            _rectangleProvider.RectanglePositioned += RectangleProvider_RectanglePositioned;
            Reset();
        }

        private void GenerateRectanglesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RectangleGeneratorWindow rgw = new RectangleGeneratorWindow();
            if (rgw.ShowDialog() ?? false)
            {
                SetRectangleProvider(rgw.RectangleProvider);
            }
        }

        private void ImportRectanglesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
            {
                dlg.Filters.Add(new CommonFileDialogFilter("JSON Files", "json"));
                dlg.Title = "Import Rectangles";
                if (dlg.ShowDialog(WindowUtils.GetActiveWindowHandle()) == CommonFileDialogResult.Ok)
                {
                    ImportRectangles(dlg.FileName);
                }
            }
        }

        private void ImportImagesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
            {
                dlg.Filters.Add(new CommonFileDialogFilter("Image Files", "bmp, jpg, jpeg, png"));
                dlg.Title = "Import Images";
                dlg.Multiselect = true;
                if (dlg.ShowDialog(WindowUtils.GetActiveWindowHandle()) == CommonFileDialogResult.Ok)
                {
                    ImportImages(new List<string>(dlg.FileNames));
                }
            }
        }

        private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _container.IsEnabled && _tileMap != null && _tileMap.Count > 0;
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
            {
                dlg.Title = "Export Tiles";
                dlg.IsFolderPicker = true;
                if (dlg.ShowDialog(WindowUtils.GetActiveWindowHandle()) == CommonFileDialogResult.Ok)
                {
                    ExportCanvases(dlg.FileName);
                }
            }
        }

        private void ExportCanvases(string directoryName)
        {
            try
            {
                foreach (int tileIndex in _tileMap.Keys)
                {
                    _tileMap[tileIndex].Save(string.Format(@"{0}\Tile{1}.png", directoryName, tileIndex));
                }
            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        private void ExitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PackCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Reset();

            int width = _widthSpinner.Value;
            int height = _heightSpinner.Value;
            int maxTiles = _tilesSpinner.Value;
            PackingFillMode fillMode = (PackingFillMode)_packingModeCombo.SelectedIndex;
            PackingOrderMode orderMode = (PackingOrderMode)_sortModeCombo.SelectedIndex;
            PackingOrder order = (PackingOrder)_sortOrderCombo.SelectedIndex;
            PackingGroupMode groupMode = (PackingGroupMode)_groupByCombo.SelectedIndex;

            _tileMap = new Dictionary<int, Canvas>();

            SetWindowEnabled(false);
            new Thread(() => Pack(width, height, maxTiles, fillMode, orderMode, order, groupMode)).Start();
        }

        private void SetWindowEnabled(bool enabled)
        {
            _container.IsEnabled = enabled;
            WindowUtils.EnableCloseButton(this, enabled);
        }

        private void Pack(int tileWidth, int tileHeight, int maxTiles, PackingFillMode fillMode, PackingOrderMode orderMode, PackingOrder order, PackingGroupMode groupMode)
        {
            try
            {
                _rectangleProvider.Pack(tileWidth, tileHeight, maxTiles, fillMode, orderMode, order, groupMode);
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(new Action(() => MessageWindow.ShowError(e.Message)));
            }
            finally
            {
                Dispatcher.Invoke(new Action(() => _timeLabel.Content = _rectangleProvider.GetProcessingTime().ToString()));
                Dispatcher.Invoke(new Action(() => SetWindowEnabled(true)));
            }
        }

        private void RectangleProvider_RectanglePositioned(object sender, RectanglePositionedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => PaintRectangle(e)));
            }
            else
            {
                PaintRectangle(e);
            }
        }

        private void PaintRectangle(RectanglePositionedEventArgs e)
        {
            if (!_tileMap.ContainsKey(e.TileIndex))
            {
                _tileMap[e.TileIndex] = new Canvas
                {
                    Width = e.TileWidth,
                    Height = e.TileHeight,
                    Margin = new Thickness(20, 20, 20, 20)
                };
                _canvasContainer.Children.Add(_tileMap[e.TileIndex]);
            }

            if (e.Rectangle is UIRectangle uiRectangle)
            {
                Rectangle r = uiRectangle.RectangleShape;
                _unsortedPanel.Children.Remove(r);
                r.Margin = new Thickness(uiRectangle.MappedX, uiRectangle.MappedY, 0, 0);
                _tileMap[e.TileIndex].Children.Add(r);
            }
            else if (e.Rectangle is UIGraphic uiGraphic)
            {
                Image r = uiGraphic.Image;
                _unsortedPanel.Children.Remove(r);
                r.Margin = new Thickness(uiGraphic.MappedX, uiGraphic.MappedY, 0, 0);
                _tileMap[e.TileIndex].Children.Add(r);
            }
        }

        private void ResetCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            _canvasContainer.Children.Clear();
            if (_tileMap != null)
            {
                foreach (Canvas c in _tileMap.Values)
                {
                    c.Children.Clear();
                }
                _tileMap.Clear();
            }

            _unsortedPanel.Children.Clear();
            _rectangleProvider.ResetRectangles();

            foreach (IRectangle r in _rectangleProvider.Rectangles)
            {
                if (r is UIRectangle uiRect)
                {
                    _unsortedPanel.Children.Add(uiRect.RectangleShape);
                }
                else if (r is UIGraphic uiGraphic)
                {
                    _unsortedPanel.Children.Add(uiGraphic.Image);
                }
            }
        }

        private void GitHubCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("https://github.com/lahm86/RectanglePacker");
        }

        private void UpdateChecker_UpdateAvailable(object sender, UpdateEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => UpdateChecker_UpdateAvailable(sender, e));
            }
            else
            {
                _updateAvailableMenu.Visibility = Visibility.Visible;
            }
        }

        private void CheckForUpdateCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (UpdateChecker.Instance.CheckForUpdates())
                {
                    ShowUpdateWindow();
                }
                else
                {
                    MessageWindow.ShowMessage("The current version of Rectangle Packer is up to date.");
                }
            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }

        private void ShowUpdateCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowUpdateWindow();
        }

        private void ShowUpdateWindow()
        {
            new UpdateAvailableWindow().ShowDialog();
        }

        private void AboutCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
    }
}