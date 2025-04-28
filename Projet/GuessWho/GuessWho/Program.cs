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
        Portrait[] player1Portraits = Array.Empty<Portrait>();
        Portrait[] player2Portraits = Array.Empty<Portrait>();
        PortraitRenderer renderer = new();

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
                    List<string> names = LoadNames(Path.Combine(Directory.GetCurrentDirectory(), "assets", "names.txt"));
                    if (names.Count < 48)
                    {
                        Console.WriteLine("Erreur : Il n'y a pas assez de noms dans le fichier !");
                        break;
                    }

                    names = names.OrderBy(_ => new Random().Next()).ToList();

                    PortraitGenerator generator = new();
                    Portrait[] allPortraits = generator.GeneratePortraits(48);

                    for (int i = 0; i < allPortraits.Length; i++)
                        allPortraits[i].Name = names[i];

                    player1Portraits = allPortraits.Take(24).ToArray();
                    player2Portraits = allPortraits.Skip(24).Take(24).ToArray();

                    foreach (Portrait p in allPortraits)
                        renderer.LoadPotraitTextures(p);

                    portraitsGenerated = true;

                }

                Rectangle player1Zone = new Rectangle(0, 0, GetScreenWidth() / 2, GetScreenHeight());
                Rectangle player2Zone = new Rectangle(GetScreenWidth() / 2, 0, GetScreenWidth() / 2, GetScreenHeight());

                uIManager.DrawPortraitGrid(player1Portraits, renderer, player1Zone, 100, 6, BasePortraitSize, 1, gameManager);
                uIManager.DrawPortraitGrid(player2Portraits, renderer, player2Zone, 100, 6, BasePortraitSize, 2, gameManager);

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
        renderer.UnloadAll();
        CloseWindow();
    }
    public static List<string> LoadNames(string filePath)
    {
        try
        {
            string fileContent = File.ReadAllText(filePath);
            return fileContent.Split(',').Select(name => name.Trim()).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur de lecture du fichier : " + ex.Message);
            return new List<string>();


        }
    }
}
