using System.Windows;
using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GameBoardViewModel();
        }
    }
}
