using System;
using System.Collections.Generic;
using GuessWho;
using Raylib_cs; // Assurez-vous que Raylib est bien intégré au projet.

class Program
{
    static void Main(string[] args)
    {
        // Initialiser Raylib
        Raylib.InitWindow(800, 600, "Guess Who ? Game");
        Raylib.SetTargetFPS(60);

        // 1. Initialisation des données du jeu
        var names = new List<string> { "Alice", "Bob", "Charlie", "David", "Eva" }; // Liste des joueurs
        GameManager gameManager = new GameManager();
        gameManager.StartGame(names);

        // 2. Initialisation du board (interface d'affichage)
        Board board = new Board();

        // 3. Boucle de jeu
        while (!Raylib.WindowShouldClose())
        {
            // 3.1. Gestion des entrées (questions, clics, etc.)
            if (gameManager.IsMyTurn())
            {
                // Exemple : poser une question
                gameManager.AskQuestion("hair", "Blonde");

                // Exemple : faire une supposition
                if (gameManager.TimesGuessed < 3)
                {
                    gameManager.MakeGuess(gameManager.PlayerPortrait); // Supposition d'exemple
                }
            }

            // 3.2. Affichage de la scène
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            // Afficher le board avec les personnages
            board.Display(gameManager.RemainingCharacters);

            // Afficher des infos supplémentaires
            Raylib.DrawText("Guess Who? - Press Q to Quit", 10, 10, 20, Color.DarkGray);

            Raylib.EndDrawing();

            // 3.3. Quitter le jeu avec la touche Q
            if (Raylib.IsKeyPressed(KeyboardKey.Q))
            {
                gameManager.Quit();
            }
        }

        // Fermer Raylib à la fin
        Raylib.CloseWindow();
    }
}
