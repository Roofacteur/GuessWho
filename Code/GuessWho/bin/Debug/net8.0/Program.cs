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
        SetTraceLogLevel(TraceLogLevel.All);
        InitWindow(1870, 1000, "Guess who ?");
        SetWindowIcon(LoadImage("assets/icons/GuessWhoLogo.png"));
        InitAudioDevice();
        SetTargetFPS(60);

        // Centrer la fenêtre sur l'écran avec notre position préférée
        CenterWindow(1890, 1100, false);

        GameManager gameManager = new GameManager();
        gameManager.Initialize();
        // Enregistrement du dernier état
        GameState lastState = gameManager.CurrentState;

        // Re-centrer une fois de plus pour s'assurer que la position est correcte
        // avant d'entrer dans la boucle principale
        CenterWindow(1890, 1100, false);

        while (!WindowShouldClose())
        {
            gameManager.Update(gameManager);
            BeginDrawing();

            // Vérifier et mettre à jour la taille et la position de la fenêtre si nécessaire
            UpdateWindowSize(gameManager, windowSizes, windowSizesInGame, ref lastState);

            // Forcer le repositionnement de la fenêtre au début des premières frames
            int frameCounter = 0;
            if (frameCounter < 5)
            {
                CenterWindow((int)windowSizes.X, (int)windowSizes.Y, false);
                frameCounter++;
            }

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
            // Ne pas fermer la fenêtre, uniquement changer sa taille
            if (gameManager.userHasDualScreen)
            {
                if (gameManager.CurrentState != GameState.InGame)
                {
                    // Dans les fenêtres hors jeu
                    SetWindowSize((int)windowSizes.X, (int)windowSizes.Y);
                }
                else
                {
                    // Dans le jeu
                    SetWindowSize((int)windowSizesInGame.X, (int)windowSizesInGame.Y);
                }
            }
            else
            {
                // Toutes les fenêtres
                SetWindowSize((int)windowSizes.X, (int)windowSizes.Y);
            }

            // Déterminer comment centrer la fenêtre
            bool useDualScreenCentering = gameManager.userHasDualScreen && gameManager.CurrentState == GameState.InGame;

            // Recentrer la fenêtre après le redimensionnement
            int width = useDualScreenCentering ? (int)windowSizesInGame.X  : (int)windowSizes.X;
            int height = (int)windowSizes.Y;
            CenterWindow(width, height, useDualScreenCentering);

            lastState = gameManager.CurrentState;
        }
    }

    static void CenterWindow(int windowWidth, int windowHeight, bool useDualScreen = false)
    {
        IntPtr hwnd = GetWindowHandle();
        int screenWidth = GetMonitorWidth(0);
        int screenHeight = GetMonitorHeight(0);

        if (useDualScreen)
        {
            // Utiliser tous les écrans disponibles pour le centrage horizontal
            screenWidth += GetMonitorWidth(1);
        }

        int posX = (screenWidth - windowWidth) / 2;

        // Ajuster la position verticale pour que la fenêtre apparaisse encore plus haute
        // Utiliser 40% de la hauteur au lieu de 45% pour décaler encore plus vers le haut
        int posY = (int)((screenHeight - windowHeight) * 0.40);

        SetWindowPos(hwnd, IntPtr.Zero, posX, posY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
    }
}