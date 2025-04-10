using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace GuessWho
{
    public class Program
    {
        static void Main() {
            InitWindow(800, 600, "Hello Raylib!");
            while (!WindowShouldClose())
            {
                BeginDrawing();
                ClearBackground(Color.RayWhite);
                EndDrawing();
            }
            CloseWindow();
        }
        
    }
       

}
