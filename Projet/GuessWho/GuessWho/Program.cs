using System;
using System.Collections.Generic;
using GuessWho;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

public static class Program
{
    const int screenWidth = 1700;
    const int screenHeight = 800;
    const int BasePortraitSize = 200;

    public static void Main()
    {
        InitWindow(screenWidth, screenHeight, "Qui est-ce ?");
        SetTargetFPS(60);

        // Charger les noms
        List<string> names = LoadNames(Path.Combine(Directory.GetCurrentDirectory(), "assets", "names.txt"));

        if (names.Count < 48)
        {
            Console.WriteLine("Erreur : Il n'y a pas assez de noms dans le fichier !");
            return;
        }

        Random r = new();
        names = names.OrderBy(x => r.Next()).ToList();

        // Générer les 48 portraits
        PortraitGenerator generator = new();
        Portrait[] allPortraits = generator.GeneratePortraits(48);

        for (int i = 0; i < allPortraits.Length; i++)
            allPortraits[i].Name = names[i];

        // Séparer les portraits pour chaque joueur
        Portrait[] player1Portraits = allPortraits.Take(24).ToArray();
        Portrait[] player2Portraits = allPortraits.Skip(24).Take(24).ToArray();

        PortraitRenderer renderer = new();
        foreach (var p in allPortraits) renderer.LoadTextures(p);

        int cols = 6;
        int portraitSize = BasePortraitSize;

        GameManager gameManager = new();
        gameManager.Initialize();


        while (!WindowShouldClose())
        {
            gameManager.Update();

            BeginDrawing();
            ClearBackground(Color.RayWhite);

            // J1 à gauche
            DrawPortraitGrid(player1Portraits, renderer, screenWidth, 100, cols, portraitSize, 1, gameManager);

            // J2 à droite
            DrawPortraitGrid(player2Portraits, renderer, screenWidth, 100, cols, portraitSize, 2, gameManager);

            // Tour actuel
            string turnText = $"Tour du Joueur {gameManager.GetCurrentPlayer()}";
            DrawText(turnText, screenWidth / 2 - 150, 30, 40, Color.Black);



            if (IsKeyPressed(KeyboardKey.Space))
                gameManager.NextTurn();

            EndDrawing();
        }

        renderer.UnloadAll();
        CloseWindow();
    }

    static void DrawPortraitGrid(Portrait[] portraits, PortraitRenderer renderer, int screenWidth, int startY, int cols, int originalSize, int playerId, GameManager gameManager)
    {
        int size = originalSize / 2;
        int spacing = 20;
        int gridWidth = cols * (size + spacing);

        // Position horizontale selon le joueur
        int startX = (playerId == 1)
            ? (screenWidth / 4) - (gridWidth / 2)
            : (3 * screenWidth / 4) - (gridWidth / 2);

        for (int i = 0; i < portraits.Length; i++)
        {
            int row = i / cols;
            int col = i % cols;
            int x = startX + col * (size + spacing);
            int y = startY + row * (size + spacing);

            Rectangle rect = new(x, y, size, size);

            // Nom sous le portrait
            if (!string.IsNullOrEmpty(portraits[i].Name))
                DrawText(portraits[i].Name, x, y + size + 5, 16, Color.Black);

            // Clic actif pour le joueur concerné
            if (gameManager.GetCurrentPlayer() == playerId &&
                CheckCollisionPointRec(GetMousePosition(), rect) &&
                IsMouseButtonPressed(MouseButton.Left))
            {
                portraits[i].IsEliminated = !portraits[i].IsEliminated;
            }

            renderer.Draw(portraits[i], x, y, size);
        }
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

