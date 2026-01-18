using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordleSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            this.KeyDown += MainWindow_KeyDown;
            this.TextInput += MainWindow_TextInput;
        }

        private void MainWindow_TextInput(object sender, TextCompositionEventArgs e)
        {
            string text = e.Text.ToUpper();

            if (text.Length == 1 && char.IsLetter(text[0]))
            {
                _vm.KeyPressed(text);
            }

            e.Handled = true;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                _vm.KeyPressed("Enter");
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
                _vm.KeyPressed("Delete");
        }

        private void OnScreenKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is KeyboardKey key)
                _vm.KeyPressed(key.Letter);
        }
    }
}