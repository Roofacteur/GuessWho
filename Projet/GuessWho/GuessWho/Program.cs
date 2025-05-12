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
        InitWindow(1870, 1000, "Guess who ?");
        InitAudioDevice();
        CenterWindow(1890, 1100, false);
        SetTargetFPS(60);

        GameManager gameManager = new GameManager();

        gameManager.Initialize();
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
                        InitWindow(1870, 1000, "Guess who ?");
                        CenterWindow(1890, 1100, false);
                        SetTargetFPS(60);

                    }
                    else
                    {
                        InitWindow(3740, 1000, "Guess who ?");
                        SetTargetFPS(60);
                        CenterWindow(3760, 1100, true);
                    }
                }
                else
                {
                    InitWindow(1870, 1000, "Guess who ?");
                    CenterWindow(1890, 1100, false);
                    SetTargetFPS(60);
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
                {
                    gameManager.StateSelectingPortrait = true;
                    gameManager.portraitsGenerated = false;
                    gameManager.Generate();
                }

            }
            else if (gameManager.StateSelectingPortrait)
            {
                if (IsKeyPressed(KeyboardKey.Space))
                    gameManager.NextTurn();
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
