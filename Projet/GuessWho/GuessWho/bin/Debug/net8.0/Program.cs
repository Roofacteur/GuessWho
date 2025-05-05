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
        InitWindow(1800, 800, "Guess who ?");
        SetTargetFPS(60);

        GameManager gameManager = new GameManager();

        gameManager.Initialize();
        GameState lastState = gameManager.CurrentState;

        while (!WindowShouldClose())
        {
            gameManager.Update(gameManager);
            BeginDrawing();

            // Ouverture et fermeture de fenêtres

            if (gameManager.CurrentState != lastState)
            {
                CloseWindow();

                if (gameManager.CurrentState != GameState.InGame)
                {
                    InitWindow(1800, 800, "Guess Who");
                    SetTargetFPS(60);
                    CenterWindow(1800, 1000, false);
                   
                }
                else if (gameManager.CurrentState == GameState.InGame)
                {
                    InitWindow(3740, 900, "Guess who ?");
                    CenterWindow(3760, 1000, true);
                }

                SetTargetFPS(60);
                lastState = gameManager.CurrentState;
            }
            else if (gameManager.CurrentState == GameState.Generation)
            {
                if (IsKeyPressed(KeyboardKey.R))
                    gameManager.generatedExample = false;
            }

            else if (gameManager.CurrentState == GameState.InGame)
            {
                if (IsKeyPressed(KeyboardKey.Space))
                    gameManager.NextTurn();

                if (IsKeyPressed(KeyboardKey.R))
                    gameManager.portraitsGenerated = false;
            }
            EndDrawing();
            
        }

        gameManager.renderer.UnloadAll();
        gameManager.uIManager.UnloadAll();
        CloseWindow();
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
