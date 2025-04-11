using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class Board
    {
        public void Display(List<Character> characters)
        {
            foreach (var c in characters)
            {
                if (c.IsVisible)
                {
                    // DrawTexture(c.PortraitTexture, posX, posY, WHITE);
                }
            }
        }

        public Character GetClickedCharacter(int mouseX, int mouseY)
        {
            // Retourner le personnage cliqué (à implémenter selon le placement)
            return null;
        }
    }
}
