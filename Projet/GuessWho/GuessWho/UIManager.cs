using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static GuessWho.GameManager;

namespace GuessWho
{
    public class UIManager
    {
        public void DrawMenu()
        {
            // Boutons : Jouer, Instructions, Quitter
        }

        public void DrawBoard(Board board)
        {
            board.Display();
        }

        public void DrawQuestionPanel(string[] availableQuestions)
        {
            // Interface pour choisir une question
            // Afficher les questions disponibles dans une interface cliquable
        }

        public void DrawEndScreen(GameState state, int winner)
        {
            if (state == GameState.Victory)
            {
                Raylib.DrawRectangle(0, 0, 1280, 720, Color.Green);
                Raylib.DrawText($"Victoire du joueur {winner} !", 400, 300, 40, Color.Black);
                Raylib.DrawText("Appuyez sur R pour recommencer", 360, 360, 20, Color.Black);
            }
            else if (state == GameState.Defeat)
            {
                Raylib.DrawRectangle(0, 0, 1280, 720, Color.Red);
                Raylib.DrawText($"Défaite du joueur {winner}...", 400, 300, 40, Color.White);
                Raylib.DrawText("Appuyez sur R pour recommencer", 360, 360, 20, Color.White);
            }
        }
    }


}
