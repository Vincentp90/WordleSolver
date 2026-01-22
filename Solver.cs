using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordleSolver
{
    public class Solver
    {
        private readonly Random _random = new Random();
        private readonly int _wordLength = 5;

        private int _iteration = 0;
        private char[] _correctChars;
        private HashSet<char> _unknownChars = new HashSet<char>() {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'ä', 'ö', 'ü', 'ß', 'é'
        };
        private HashSet<char> _presentChars = new HashSet<char>();
        private List<IEnumerable<Tile>> _guessHistory = new List<IEnumerable<Tile>>();

        public Solver(int wordLength)
        {
            _wordLength = wordLength;
            _correctChars = new char[wordLength];
        }

        public string Iterate(IEnumerable<Tile>? madeGuessTiles = null)
        {
            string guess;
            if (_iteration > 0 && madeGuessTiles != null)
            {
                _guessHistory.Add(madeGuessTiles);
                RemoveUnknowns(madeGuessTiles);
                SetCorrectChars(madeGuessTiles);
            }

            switch (_iteration)
            {
                case 0:
                    guess = FirstGuess();
                    break;
                case 1:
                case 2:
                    guess = EliminateUnknownsGuess();
                    break;
                case 3:
                case 4:
                case 5:
                    guess = GuessCorrectWord();
                    break;
                default:
                    guess = "error";
                    break;
            }

            _iteration++;
            return guess;
        }

        private void SetCorrectChars(IEnumerable<Tile> madeGuessTiles)
        {
            for(int i = 0; i < _wordLength; i++)
            {
                if (madeGuessTiles.ElementAt(i).State == TileState.Correct)
                    _correctChars[i] = CharAt(madeGuessTiles, i);
            }
        }

        private void RemoveUnknowns(IEnumerable<Tile> madeGuessTiles)
        {
            for (int i = 0; i < _wordLength; i++)
            {
                var letter = CharAt(madeGuessTiles, i);
                if (madeGuessTiles.ElementAt(i).State == TileState.Absent && _unknownChars.Contains(letter))
                    _unknownChars.Remove(letter);
                if((madeGuessTiles.ElementAt(i).State == TileState.Present || madeGuessTiles.ElementAt(i).State == TileState.Correct) && 
                    !_presentChars.Contains(letter))
                    _presentChars.Add(letter);
            }
        }

        private char CharAt(IEnumerable<Tile> madeGuessTiles, int index) => madeGuessTiles.ElementAt(index).Letter.ToLower()[0];

        private string FirstGuess()
        {
            int index = _random.Next(App.CommonWords.Count);
            string word = App.CommonWords[index];

            // We want a word with 5 unique characters
            while (word.Distinct().Count() != _wordLength)
            {
                index++;
                if (index >= App.CommonWords.Count)
                    index = 0;
                word = App.CommonWords[index];
            }

            return word;
        }

        private string EliminateUnknownsGuess()
        {
            // Find word with most unknowns
            int maxUnkowns = 0;
            int maxWeight = 0;
            string guess = App.CommonWords[0];
            foreach (var word in App.CommonWords)
            {
                int unknowns = word.Distinct().Count(_unknownChars.Contains);
                // Find word with 1st priority: most unknown characters
                if (unknowns > maxUnkowns)
                {
                    maxUnkowns = unknowns;
                    maxWeight = 0;// Reset weight, we recalculate in next if
                    guess = word;
                }
                // 2nd priority: highest weight of unknown characters
                if (unknowns == maxUnkowns)
                {
                    int weight = GetWeight(word.Where(_unknownChars.Contains));
                    if (weight > maxWeight)
                    {
                        maxWeight = weight;
                        guess = word;
                    }
                }
            }
            return guess;
        }

        private int GetWeight(IEnumerable<char> word)
        {
            int weight = 0;
            foreach (char c in word)
            {
                weight += App.CharWeights[c];
            }
            return weight;
        }

        private string GuessCorrectWord()
        {
            var filteredWords = App.CommonWords.Where(w => _presentChars.All(w.Contains));

            // If a word has a character that is unpresent (= not unknown and not correct) remove it
            filteredWords = filteredWords.Where(w => !w.Any(x => !_unknownChars.Contains(x) && !_correctChars.Contains(x)));

            foreach (var word in filteredWords)
            {
                bool isCorrect = true;
                for (int i = 0; i < _wordLength; i++)
                {
                    if(_correctChars[i] != default(char) && _correctChars[i] != word[i])
                    {
                        isCorrect = false;
                        break;
                    }
                }
                if (isCorrect)
                    isCorrect = VerifyWithHistory(word);
                if (isCorrect)
                    return word;
            }
            return "error";
        }

        private bool VerifyWithHistory(string guess)
        {
            // Chars should not be in a position that was marked as present            
            foreach(var oldGuess in _guessHistory)
            {
                for (int i = 0; i < _wordLength; i++)
                {
                    if (oldGuess.ElementAt(i).State == TileState.Present && CharAt(oldGuess, i) == guess[i])
                        return false;
                }
            }
            // Chars should not be in a position that was marked as absent (only possible if appears twice in a guess, otherwise we catch this with earlier logic)
            foreach (var oldGuess in _guessHistory)
            {
                for (int i = 0; i < _wordLength; i++)
                {
                    if (oldGuess.ElementAt(i).State == TileState.Absent && CharAt(oldGuess, i) == guess[i])
                        return false;
                }
            }
            return true;
        }
    }
}
