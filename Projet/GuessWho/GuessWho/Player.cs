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
        public Portrait SelectedGuess { get; set; }
        public int Id { get; private set; }
        public Player(Portrait[] portraits, int id)
        {
            Id = id;
            Board = new Board(portraits);
            TargetPortrait = null;
            SelectedGuess = null;
        }

        public void Reset()
        {
            TargetPortrait = null;
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
