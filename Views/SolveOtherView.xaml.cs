using System;
using System.Collections.Generic;
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
using WordleSolver.ViewModel;

namespace WordleSolver.Views
{
    /// <summary>
    /// Interaction logic for SolveOtherView.xaml
    /// </summary>
    public partial class SolveOtherView : UserControl
    {
        private readonly SolveOtherViewModel _vm;

        public SolveOtherView()
        {
            InitializeComponent();
            _vm = new SolveOtherViewModel();
            DataContext = _vm;
        }
    }
}
