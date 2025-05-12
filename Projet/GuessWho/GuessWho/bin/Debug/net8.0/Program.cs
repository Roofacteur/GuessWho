using GuessWho;
using Raylib_cs;
using static GuessWho.GameManager;
using static Raylib_cs.Raylib;
using System.Runtime.InteropServices;

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
        // Fenêtre initiale
        InitWindow(1870, 1000, "Guess who ?");
        InitAudioDevice();

        //Centrer la fenêtre sur l'écran
        CenterWindow(1890, 1100, false);
        SetTargetFPS(60);

        GameManager gameManager = new GameManager();
        gameManager.Initialize();
        //Enregistrement du dernier état
        GameState lastState = gameManager.CurrentState;

        while (!WindowShouldClose())
        {
            gameManager.Update(gameManager);
            BeginDrawing();

            if (gameManager.CurrentState != lastState)
            {
                CloseWindow();
                if(gameManager.userHasDualScreen)
                {
                    if (gameManager.CurrentState != GameState.InGame)
                    {
                        // Dans les fenêtres hors jeu
                        InitWindow(1870, 1000, "Guess who ?");
                        CenterWindow(1890, 1100, false);
                        SetTargetFPS(60);

                    }
                    else
                    {
                        // Dans le jeu
                        InitWindow(3740, 1000, "Guess who ?");
                        SetTargetFPS(60);
                        CenterWindow(3760, 1100, true);
                    }
                }
                else
                {
                    // Toutes les fenêtres
                    InitWindow(1870, 1000, "Guess who ?");
                    CenterWindow(1890, 1100, false);
                    SetTargetFPS(60);
                }             

                SetTargetFPS(60);
                lastState = gameManager.CurrentState;
            }
            else if (gameManager.CurrentState == GameState.Generation)
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

        gameManager.renderer.UnloadAll();
        gameManager.uIManager.UnloadAll();
        CloseWindow();
    }

    static void CenterWindow(int windowWidth, int windowHeight, bool dualScreen = false)
    {
        IntPtr hwnd = GetWindowHandle(); // Récupère le handle de la fenêtre
        int screenWidth = GetMonitorWidth(0); // Largeur de l'écran principal
        int screenHeight = GetMonitorHeight(0); // Hauteur de l'écran principal

        if (dualScreen)
        {
            screenWidth += GetMonitorWidth(1); // Ajoute la largeur du second écran si dualScreen est activé
        }

        int posX = (screenWidth - windowWidth) / 2; // Calcule la position X centrée
        int posY = (screenHeight - windowHeight) / 2; // Calcule la position Y centrée

        // Positionne la fenêtre au centre sans modifier sa taille ni son ordre Z
        SetWindowPos(hwnd, IntPtr.Zero, posX, posY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
    }

}
