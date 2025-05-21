using GuessWho;
using Raylib_cs;
using static GuessWho.GameManager;
using static Raylib_cs.Raylib;
using System.Runtime.InteropServices;
using System.Numerics;

public static class Program
{
    // Import WinAPI : repositionnement de fenêtre (Windows uniquement)
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("raylib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetWindowHandle();

    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOZORDER = 0x0004;

    public static void Main()
    {
        Vector2 windowSizes = new Vector2(1870, 1000);
        Vector2 windowSizesInGame = new Vector2(3740, 1000);

        SetTraceLogLevel(TraceLogLevel.All);
        InitWindow(1870, 1000, "Guess who ?");
        SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
        InitAudioDevice();
        SetTargetFPS(60);

        CenterWindow(1890, 1100, false);

        GameManager gameManager = new GameManager();
        gameManager.Initialize();

        GameState lastState = gameManager.CurrentState;
        CenterWindow(1890, 1100, false);

        while (!WindowShouldClose())
        {
            gameManager.Update(gameManager);
            BeginDrawing();

            UpdateWindowSize(gameManager, windowSizes, windowSizesInGame, ref lastState);

            // Recentre pendant les 5 premières frames
            int frameCounter = 0;
            if (frameCounter < 5)
            {
                CenterWindow((int)windowSizes.X, (int)windowSizes.Y, false);
                frameCounter++;
            }

            if (gameManager.CurrentState == GameState.Generation)
            {
                // Regénère les portraits d’exemple
                if (IsKeyPressed(KeyboardKey.R))
                    gameManager.generatedExample = false;
            }
            else if (gameManager.CurrentState == GameState.InGame)
            {
                // Passe au tour suivant
                if (IsMouseButtonPressed(MouseButton.Right))
                {
                    gameManager.NextTurn();
                    gameManager.uIManager.MoveMouse(gameManager);
                    PlaySound(gameManager.soundManager.flickSound);
                }

                // Redémarre une partie
                if (IsKeyPressed(KeyboardKey.R))
                {
                    gameManager.StateSelectingPortrait = true;
                    gameManager.portraitsGenerated = false;
                    gameManager.Generate();
                    PlaySound(gameManager.soundManager.restartSound);
                }
            }

            EndDrawing();
        }

        // Libère les ressources
        gameManager.renderer.UnloadAll();
        gameManager.uIManager.UnloadAll();
        CloseWindow();
    }

    // Gère le redimensionnement dynamique de la fenêtre selon l'état du jeu
    static void UpdateWindowSize(GameManager gameManager, Vector2 windowSizes, Vector2 windowSizesInGame, ref GameState lastState)
    {
        if (gameManager.CurrentState != lastState)
        {
            if (gameManager.userHasDualScreen)
            {
                if (gameManager.CurrentState != GameState.InGame)
                    SetWindowSize((int)windowSizes.X, (int)windowSizes.Y);
                else
                    SetWindowSize((int)windowSizesInGame.X, (int)windowSizesInGame.Y);
            }
            else
            {
                SetWindowSize((int)windowSizes.X, (int)windowSizes.Y);
            }

            bool useDualScreenCentering = gameManager.userHasDualScreen && gameManager.CurrentState == GameState.InGame;

            int width = useDualScreenCentering ? (int)windowSizesInGame.X : (int)windowSizes.X;
            int height = (int)windowSizes.Y;
            CenterWindow(width, height, useDualScreenCentering);

            lastState = gameManager.CurrentState;
        }
    }

    // Centre la fenêtre à l’écran (avec option double écran)
    static void CenterWindow(int windowWidth, int windowHeight, bool useDualScreen = false)
    {
        IntPtr hwnd = GetWindowHandle();
        int screenWidth = GetMonitorWidth(0);
        int screenHeight = GetMonitorHeight(0);

        if (useDualScreen)
            screenWidth += GetMonitorWidth(1);

        int posX = (screenWidth - windowWidth) / 2;
        int posY = (int)((screenHeight - windowHeight) * 0.20);

        SetWindowPos(hwnd, IntPtr.Zero, posX, posY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
    }
}
