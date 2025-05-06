using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace GuessWho
{
    public class PortraitRenderer
    {
        private Dictionary<string, Texture2D> textures = new();
        public void LoadPotraitTextures(Portrait portrait)
        {
            LoadPortrait(portrait.Skin);
            LoadPortrait(portrait.Clothes);
            LoadPortrait(portrait.Logo);
            LoadPortrait(portrait.Eyebrows);
            LoadPortrait(portrait.Eyes);
            LoadPortrait(portrait.Beard);
            LoadPortrait(portrait.Glasses);
            LoadPortrait(portrait.Hair);
            LoadPortrait(portrait.Mouth);
            LoadPortrait(portrait.Gender);
        }

        private void LoadPortrait(string path)
        {
            if (!textures.ContainsKey(path))
            {
                Texture2D texture = LoadTexture(path);
                SetTextureFilter(texture, TextureFilter.Bilinear);
                textures[path] = texture;
            }

        }

        public void DrawPortrait(Portrait portrait, int x, int y, int size)
        {
            string[] layers = { portrait.Skin, portrait.Clothes, portrait.Logo, portrait.Eyes, portrait.Eyebrows, portrait.Glasses, portrait.Hair, portrait.Beard, portrait.Mouth, portrait.Gender };

            foreach (string path in layers)
            {
                if (textures.TryGetValue(path, out Texture2D texture))
                {
                    Rectangle gridDestination = new Rectangle(x, y, size, size);

                    DrawTexturePro(
                        texture,
                        new Rectangle(0, 0, texture.Width, texture.Height),
                        gridDestination,
                        Vector2.Zero,
                        0f,
                        portrait.IsEliminated ? new Color(0, 0, 0, 255) : Color.White
                    );
                }
            }
        }

        public void UnloadAll()
        {
            foreach (var tex in textures.Values)
            {
                   UnloadTexture(tex);
            }
            textures.Clear();
        }
    }

}
