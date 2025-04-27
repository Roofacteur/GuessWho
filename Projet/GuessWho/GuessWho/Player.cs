using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class Player
    {
        public Board Board { get; private set; }
        public Portrait TargetPortrait { get; private set; }
        public Portrait SelectedGuess { get; set; }  // Le portrait que le joueur pense être celui de l'adversaire
        public int Id { get; private set; }

        public Player(Portrait[] portraits, int id)
        {
            Id = id;
            Board = new Board(portraits);
            TargetPortrait = SelectRandomPortrait(portraits);
            SelectedGuess = null;
        }

        private Portrait SelectRandomPortrait(Portrait[] portraits)
        {
            var random = new Random();
            return portraits[random.Next(portraits.Length)];
        }

        public void Reset()
        {
            TargetPortrait = SelectRandomPortrait(Board.Portraits);
            SelectedGuess = null;
            Board.Reset();
        }

        public bool MakeGuess(Portrait guess)
        {
            SelectedGuess = guess;
            return TargetPortrait == guess;
        }
    }

}
