using GuessWho;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;
using static Raylib_cs.Raylib;
using static GuessWho.GameManager;

public static class Program
{
    #region Import WinAPI

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("raylib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetWindowHandle();

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;

    #endregion

    #region Méthode principale

    public static void Main()
    {
        Vector2 windowSizeDefault = new Vector2(1870, 1000);
        Vector2 windowSizeInGame = new Vector2(3740, 1000);
        int frameCounter = 0;

        SetTraceLogLevel(TraceLogLevel.All);
        InitWindow((int)windowSizeDefault.X, (int)windowSizeDefault.Y, "Guess who ?");
        SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
        InitAudioDevice();
        SetTargetFPS(60);

        CenterWindow((int)windowSizeDefault.X, (int)windowSizeDefault.Y);

        GameManager gameManager = new GameManager();
        gameManager.Initialize();

        GameState lastState = gameManager.CurrentState;

        while (!WindowShouldClose())
        {
            gameManager.Update(gameManager);
            BeginDrawing();

            UpdateWindowSize(gameManager, windowSizeDefault, windowSizeInGame, ref lastState);

            // Recentrage automatique pendant les premières frames (évite les erreurs d’affichage)
            if (frameCounter < 5)
            {
                CenterWindow((int)windowSizeDefault.X, (int)windowSizeDefault.Y);
                frameCounter++;
            }

            switch (gameManager.CurrentState)
            {
                case GameState.Generation:
                    if (IsKeyPressed(KeyboardKey.R))
                        gameManager.generatedExample = false;
                    break;

                case GameState.InGame:
                    if (IsMouseButtonPressed(MouseButton.Right))
                    {
                        gameManager.NextTurn();
                        gameManager.uIManager.MoveMouse(gameManager);
                        PlaySound(gameManager.soundManager.flickSound);
                    }

                    if (IsKeyPressed(KeyboardKey.R))
                    {
                        gameManager.StateSelectingPortrait = true;
                        gameManager.portraitsGenerated = false;
                        gameManager.Generate();
                        PlaySound(gameManager.soundManager.restartSound);
                    }
                    break;
            }

            EndDrawing();
        }

        // Nettoyage des ressources système
        gameManager.renderer.UnloadAll();
        gameManager.uIManager.UnloadAll();
        CloseWindow();
    }

    #endregion

    #region Méthodes utilitaires

    /// <summary>
    /// Met à jour dynamiquement la taille et la position de la fenêtre en fonction de l’état du jeu.
    /// </summary>
    static void UpdateWindowSize(GameManager gameManager, Vector2 sizeDefault, Vector2 sizeInGame, ref GameState lastState)
    {
        if (gameManager.CurrentState != lastState)
        {
            Vector2 newSize = gameManager.CurrentState == GameState.InGame && gameManager.userHasDualScreen
                ? sizeInGame
                : sizeDefault;

            SetWindowSize((int)newSize.X, (int)newSize.Y);

            bool useDualScreen = gameManager.userHasDualScreen && gameManager.CurrentState == GameState.InGame;
            CenterWindow((int)newSize.X, (int)newSize.Y, useDualScreen);

            lastState = gameManager.CurrentState;
        }
    }

    /// <summary>
    /// Centre la fenêtre principale sur l’écran ou sur un double écran si précisé.
    /// </summary>
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

    #endregion
}
