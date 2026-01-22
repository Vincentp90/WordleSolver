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
        public static IReadOnlyList<string> AllWords { get; private set; } = null!;
        public static IReadOnlyList<string> CommonWords { get; private set; } = null!;
        public static Dictionary<char, int> CharWeights { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AllWords = File.ReadAllLines("wordfreq\\all5letter.txt");
            CommonWords = File.ReadAllLines("wordfreq\\common5letter.txt");

            CharWeights = CalculateWeights(CommonWords);
        }

        private Dictionary<char, int> CalculateWeights(IReadOnlyList<string> CommonWords)
        {
            var weights = new Dictionary<char, int>();

            // Count occurence of each char
            foreach(var word in CommonWords)
            {
                foreach (var character in word)
                {
                    if(!weights.ContainsKey(character))
                    {
                        weights[character] = 1;
                    }
                    else
                    {
                        weights[character]++;
                    }
                }
            }

            // Divide all weights by lowest weight
            double lowest = Convert.ToDouble(weights.Min(w => w.Value));
            foreach (var key in weights.Keys)
            {
                weights[key] = Convert.ToInt32(Math.Round(weights[key] * lowest));
            }

            return weights;
        }
    }

}
