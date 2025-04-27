using System.Numerics;
using GuessWho;
using Raylib_cs;
using static GuessWho.GameManager;
using static Raylib_cs.Raylib;

public static class Program
{
    const int screenWidth = 3840;
    const int screenHeight = 800;
    const int BasePortraitSize = 300;

    public static void Main()
    {
        InitWindow(screenWidth, screenHeight, "Qui est-ce ?");
        SetTargetFPS(60);

        UIManager uIManager = new UIManager();
        GameManager gameManager = new();
        gameManager.Initialize();
        gameManager.CurrentState = GameState.Menu;

        Portrait[] player1Portraits = Array.Empty<Portrait>();
        Portrait[] player2Portraits = Array.Empty<Portrait>();
        PortraitRenderer renderer = new();

        while (!WindowShouldClose())
        {
            gameManager.Update();

            if (gameManager.CurrentState == GameState.Menu)
            {

                BeginDrawing();
                ClearBackground(Color.DarkGray);

                uIManager.UpdateMenu(gameManager);
                EndDrawing();
            }
            else if (gameManager.CurrentState == GameState.InGame)
            {
                BeginDrawing();
                ClearBackground(Color.DarkGray);

                uIManager.DrawPortraitGrid(player1Portraits, renderer, screenWidth, 100, 6, BasePortraitSize, 1, gameManager);
                uIManager.DrawPortraitGrid(player2Portraits, renderer, screenWidth, 100, 6, BasePortraitSize, 2, gameManager);

                string turnText = $"Tour du Joueur {gameManager.GetCurrentPlayer()}";
                DrawText(turnText, screenWidth / 2 - 150, 30, 40, Color.Black);

                if (IsKeyPressed(KeyboardKey.Space))
                    gameManager.NextTurn();

                EndDrawing();
            }


            if (gameManager.CurrentState == GameState.InGame && player1Portraits.Length == 0)
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
                    renderer.LoadTextures(p);
            }
        }

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
