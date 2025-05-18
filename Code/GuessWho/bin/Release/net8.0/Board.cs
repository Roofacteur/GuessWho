using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class Board
    {
        public Portrait[] Portraits;

        public Board(Portrait[] portraits)
        {
            Portraits = portraits;
        }

        public void Display()
        {
            foreach (var portrait in Portraits)
            {
                if (!portrait.IsEliminated)
                {
                    
                }
            }
        }

        public void EliminatePortraitsByQuestion(string attribute, string value)
        {
            foreach (var portrait in Portraits)
            {
                var dna = portrait.GetDNA();
                bool match = attribute switch
                {
                    "Skin" => portrait.Skin == value,
                    "Clothes" => portrait.Clothes == value,
                    "Logo" => portrait.Logo == value,
                    "Eyebrows" => portrait.Eyebrows == value,
                    "Eyes" => portrait.Eyes == value,
                    "Glasses" => portrait.Glasses == value,
                    "Hair" => portrait.Hair == value,
                    "Mouth" => portrait.Mouth == value,
                    _ => false
                };

                if (!match) portrait.IsEliminated = true;
            }
        }

        public void Reset()
        {
            foreach (var portrait in Portraits)
            {
                portrait.IsEliminated = false;
            }
        }
    }


}
