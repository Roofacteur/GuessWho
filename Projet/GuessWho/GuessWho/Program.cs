using System.Numerics;
using GuessWho;
using Raylib_cs;
using static GuessWho.GameManager;
using static Raylib_cs.Raylib;

public static class Program
{
    const int BasePortraitSize = 300;
    static Texture2D backgroundMenu;
    static Texture2D backgroundInGame;
    public static void Main()
    {
        InitWindow(1800, 800, "Qui est-ce ?");
        SetTargetFPS(60);

        bool portraitsGenerated = false;

        UIManager uIManager = new UIManager();
        GameManager gameManager = new();
        gameManager.Initialize();

        GameState lastState = gameManager.CurrentState; // Suivi de l'ancien état

        backgroundMenu = LoadTexture("assets/backgrounds/MenuBackground.png");

        while (!WindowShouldClose())
        {
            gameManager.Update();

            BeginDrawing();

            if (gameManager.CurrentState == GameState.InGame)
            {
                ClearBackground(Color.White);
                DrawTexture(backgroundInGame, 0, 0, Color.White);
            }
            else if (gameManager.CurrentState == GameState.Menu)
            {
                ClearBackground(Color.White);
                DrawTexture(backgroundMenu, 0, 0, Color.White);
            }

            if (gameManager.CurrentState != lastState)
            {
                if (gameManager.CurrentState == GameState.Menu)
                {
                    CloseWindow();
                    InitWindow(1800, 800, "Qui est-ce ?");
                    SetTargetFPS(60);
                    //ClearBackground(Color.White);
                    //DrawTexture(backgroundMenu, 0, 0, Color.White);

                }
                else if (gameManager.CurrentState == GameState.InGame)
                {
                    CloseWindow();
                    InitWindow(3740, 900, "Qui est-ce ?");
                    backgroundInGame = LoadTexture("assets/backgrounds/GameBackground.png");
                    SetTargetFPS(60);
                    //ClearBackground(Color.White);
                    //DrawTexture(backgroundInGame, 0, 0, Color.White);

                }
                lastState = gameManager.CurrentState;
            }

            // Gestion de l'état du jeu
            if (gameManager.CurrentState == GameState.Menu)
            {
                uIManager.UpdateMenu(gameManager);
            }
            else if (gameManager.CurrentState == GameState.InGame)
            {
                if (!portraitsGenerated)
                {
                    gameManager.Generate();
                    portraitsGenerated = true;
                }


                Rectangle player1Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
                Rectangle player2Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());

                uIManager.DrawPortraitGrid(gameManager.player1.Board.Portraits, gameManager.renderer, player1Zone, 100, 6, BasePortraitSize, 1, gameManager);
                uIManager.DrawPortraitGrid(gameManager.player2.Board.Portraits, gameManager.renderer, player2Zone, 100, 6, BasePortraitSize, 2, gameManager);


                string turnText = $"Le joueur {gameManager.GetCurrentPlayer()} pose une question...";

                if (gameManager.GetCurrentPlayer() == 1)
                {   
                    DrawText(turnText, GetScreenWidth() / 2 + 100, 30, 40, Color.Black);
                }
                else
                {
                    DrawText(turnText, 100, 30, 40, Color.Black);
                }

                if (IsKeyPressed(KeyboardKey.Space))
                    gameManager.NextTurn();
            }

            EndDrawing();
        }

        UnloadTexture(backgroundMenu);
        UnloadTexture(backgroundInGame);
        gameManager.renderer.UnloadAll();
        CloseWindow();
    }
    
}
