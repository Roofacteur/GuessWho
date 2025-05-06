using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class Player
    {
        public Board Board { get; private set; }
        public Raylib_cs.Rectangle Zone; // Spécifier car conflit entre "System.Drawing.Rectangle" et "Raylib_cs.Rectangle"
        public Portrait TargetPortrait;
        public Portrait SelectedGuess { get; set; }
        public int Id { get; private set; }
        public Player(Portrait[] portraits, int id)
        {
            Id = id;
            Board = new Board(portraits);
        }

        public void Reset()
        {
            Board.Reset();
        }

        public bool MakeGuess(Portrait guess)
        {
            SelectedGuess = guess;
            return TargetPortrait == guess;
        }
    }

}
