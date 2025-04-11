using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class GameManager
    {
        public List<Character> AllCharacters = new();
        public List<Character> RemainingCharacters = new();
        public Character PlayerPortrait;
        public Character OpponentPortrait;
        public bool IsPlayerTurn = true;
        public int TimesGuessed = 0;

        public void StartGame(List<string> names)
        {
            AllCharacters = PortraitGenerator.GenerateCharacters(names);
            RemainingCharacters = new List<Character>(AllCharacters);
            AssignPortraits();
        }

        public void AssignPortraits()
        {
            Random rng = new();
            PlayerPortrait = AllCharacters[rng.Next(AllCharacters.Count)];
            do
            {
                OpponentPortrait = AllCharacters[rng.Next(AllCharacters.Count)];
            } while (OpponentPortrait == PlayerPortrait);
        }

        public void AskQuestion(string criteria, string value)
        {
            bool match = OpponentPortrait.MatchesCriteria(criteria, value);
            EliminatePortraits(criteria, value, match);
            SwitchTurn();
        }

        public void MakeGuess(Character guess)
        {
            TimesGuessed++;
            if (guess == OpponentPortrait)
            {
                Console.WriteLine("🎉 Correct! You won.");
            }
            else
            {
                Console.WriteLine("❌ Incorrect guess.");
                SwitchTurn();
            }
        }

        public void EliminatePortraits(string criteria, string value, bool keepMatching)
        {
            foreach (var c in RemainingCharacters)
            {
                if (c.IsVisible && c.MatchesCriteria(criteria, value) != keepMatching)
                {
                    c.Hide();
                }
            }
        }

        public void SwitchTurn() => IsPlayerTurn = !IsPlayerTurn;
        public bool IsMyTurn() => IsPlayerTurn;
        public void Restart() => StartGame(AllCharacters.Select(c => c.Name).ToList());
        public void Quit() => Environment.Exit(0);
    }

}
