using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace WordleSolver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IReadOnlyList<string> AllWords { get; private set; }
        public static IReadOnlyList<string> CommonWords { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AllWords = File.ReadAllLines("wordfreq\\all5letter.txt");
            CommonWords = File.ReadAllLines("wordfreq\\common5letter.txt");
        }
    }

}
