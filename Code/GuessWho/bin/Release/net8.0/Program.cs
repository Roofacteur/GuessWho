using GuessWho;
using Raylib_cs;
using static GuessWho.GameManager;
using static Raylib_cs.Raylib;
using System.Runtime.InteropServices;
using System.Numerics;

public static class Program
{

    // Déclarations de la WinAPI (Attention ne fonctionne que sur Windows) qui permettent de centrer les fenêtres sur l'écran
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

        // Fenêtre initiale
        InitWindow(1870, 1000, "Guess who ?");
        SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
        InitAudioDevice();
        SetTargetFPS(60);

        // Centrer la fenêtre sur l'écran
        CenterWindow(1890, 1100, false);

        GameManager gameManager = new GameManager();
        gameManager.Initialize();
        // Enregistrement du dernier état
        GameState lastState = gameManager.CurrentState;

        while (!WindowShouldClose())
        {
            gameManager.Update(gameManager);
            BeginDrawing();
            UpdateWindowSize(gameManager, windowSizes, windowSizesInGame, ref lastState);

            if (gameManager.CurrentState == GameState.Generation)
            {
                // Regénérer des portraits d'exemple
                if (IsKeyPressed(KeyboardKey.R))
                    gameManager.generatedExample = false;
            }
            else if (gameManager.CurrentState == GameState.InGame)
            {
                // Passer le tour
                if (IsMouseButtonPressed(MouseButton.Right))
                {
                    gameManager.NextTurn();
                    gameManager.uIManager.MoveMouse(gameManager);
                    PlaySound(gameManager.soundManager.flickSound);
                }      

                // Recommencer une partie et regénérer des portraits
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

        // Déchargement des textures
        gameManager.renderer.UnloadAll();
        gameManager.uIManager.UnloadAll();
        CloseWindow();
    }
    static void UpdateWindowSize(GameManager gameManager, Vector2 windowSizes, Vector2 windowSizesInGame, ref GameState lastState)
    {
        if (gameManager.CurrentState != lastState)
        {
            CloseWindow();
            if (gameManager.userHasDualScreen)
            {
                
                if (gameManager.CurrentState != GameState.InGame)
                {
                    // Dans les fenêtres hors jeu
                    InitWindow((int)windowSizes.X, (int)windowSizes.Y, "Guess who ?");
                    SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
                    SetTargetFPS(60);
                    CenterWindow(1890, 1100, false);

                }
                else
                {
                    // Dans le jeu
                    InitWindow((int)windowSizesInGame.X, (int)windowSizesInGame.Y, "Guess who ?");
                    SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
                    SetTargetFPS(60);
                    CenterWindow(3760, 1100, true);
                }
            }
            else
            {
                // Toutes les fenêtres
                InitWindow((int)windowSizes.X, (int)windowSizes.Y, "Guess who ?");
                SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
                SetTargetFPS(60);
                CenterWindow(1890, 1100, false);
            }

            lastState = gameManager.CurrentState;
        }
    }
    static void CenterWindow(int windowWidth, int windowHeight, bool dualScreen = false)
    {
        IntPtr hwnd = GetWindowHandle(); 
        int screenWidth = GetMonitorWidth(0); 
        int screenHeight = GetMonitorHeight(0);

        if (dualScreen)
        {
            screenWidth += GetMonitorWidth(1);
        }

        int posX = (screenWidth - windowWidth) / 2; 
        int posY = (screenHeight - windowHeight) / 2;

        SetWindowPos(hwnd, IntPtr.Zero, posX, posY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
    }

}
