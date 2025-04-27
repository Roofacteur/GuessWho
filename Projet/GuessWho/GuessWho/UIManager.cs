using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using static GuessWho.GameManager;

namespace GuessWho
{
    public class UIManager
    {
        public static UIManager Instance { get; } = new UIManager();
        private string[] menuLabels = { "Jouer", "Options", "Quitter" };
        public void UpdateMenu(GameManager gamemanager)
        {
            Vector2 mouse = GetMousePosition();

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (CheckCollisionPointRec(mouse, GetButtonRect(i)))
                    {
                        if (i == 0) gamemanager.ChangeState(GameState.InGame);
                        if (i == 1) TraceLog(TraceLogLevel.Info, "Options – à implémenter");
                        if (i == 2) CloseWindow();
                    }
                }
            }

            DrawMenu();
        }
        public void DrawMenu()
        {

            for (int i = 0; i < menuLabels.Length; i++)
            {
                Rectangle btn = GetButtonRect(i);
                bool hover = CheckCollisionPointRec(GetMousePosition(), btn);
                Color color = hover ? Color.SkyBlue : Color.LightGray;

                DrawRectangleRec(btn, color);

                int fontSize = 30;
                Vector2 textSize = MeasureTextEx(GetFontDefault(), menuLabels[i], fontSize, 1);
                Vector2 textPos = new Vector2(btn.X + (btn.Width - textSize.X) / 2, btn.Y + (btn.Height - textSize.Y) / 2);
                DrawText(menuLabels[i], (int)textPos.X, (int)textPos.Y, fontSize, Color.DarkBlue);
            }
        }

        public void DrawBoard(Board board)
        {
            board.Display();
        }

        public void DrawQuestionPanel(string[] availableQuestions)
        {
        }
        public void DrawPortraitGrid(Portrait[] portraits, PortraitRenderer renderer, int screenWidth, int startY, int cols, int originalSize, int playerId, GameManager gameManager)
        {
            int size = originalSize / 2;
            int spacing = 20;
            int gridWidth = cols * (size + spacing);
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

                if (!string.IsNullOrEmpty(portraits[i].Name))
                    DrawText(portraits[i].Name, x, y + size + 5, 16, Color.Black);

                if (gameManager.GetCurrentPlayer() == playerId &&
                    CheckCollisionPointRec(GetMousePosition(), rect) &&
                    IsMouseButtonPressed(MouseButton.Left))
                {
                    portraits[i].IsEliminated = !portraits[i].IsEliminated;
                }

                renderer.Draw(portraits[i], x, y, size);
            }
        }
        public void DrawEndScreen(GameState state, int winner)
        {
            if (state == GameState.Victory)
            {
                DrawRectangle(0, 0, 1280, 720, Color.Green);
                DrawText($"Victoire du joueur {winner} !", 400, 300, 40, Color.Black);
                DrawText("Appuyez sur R pour recommencer", 360, 360, 20, Color.Black);
            }
            else if (state == GameState.Defeat)
            {
                DrawRectangle(0, 0, 1280, 720, Color.Red);
                DrawText($"Défaite du joueur {winner}...", 400, 300, 40, Color.White);
                DrawText("Appuyez sur R pour recommencer", 360, 360, 20, Color.White);
            }
        }
        private Rectangle GetButtonRect(int index)
        {
            float width = 200;
            float height = 60;
            float spacing = 20;
            float totalHeight = (height + spacing) * 3 - spacing;
            float startY = (GetScreenHeight() - totalHeight) / 2;

            return new Rectangle(
                (GetScreenWidth() - width) / 2,
                startY + index * (height + spacing),
                width,
                height
            );
        }
    }


}
