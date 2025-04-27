using Raylib_cs;
using static Raylib_cs.Raylib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class PortraitRenderer
    {
        private Dictionary<string, Texture2D> textures = new();
        public void LoadTextures(Portrait portrait)
        {
            LoadTexture(portrait.Skin);
            LoadTexture(portrait.Clothes);
            LoadTexture(portrait.Logo);
            LoadTexture(portrait.Eyebrows);
            LoadTexture(portrait.Eyes);
            LoadTexture(portrait.Glasses);
            LoadTexture(portrait.Hair);
            LoadTexture(portrait.Mouth);
        }

        private void LoadTexture(string path)
        {
            if (!textures.ContainsKey(path))
            {
                Texture2D tex = Raylib.LoadTexture(path);
                SetTextureFilter(tex, TextureFilter.Bilinear);

                textures[path] = tex;
            }
        }

        public void Draw(Portrait portrait, int x, int y, int size)
        {
            var layers = new[] { portrait.Skin, portrait.Clothes, portrait.Logo, portrait.Eyebrows, portrait.Eyes, portrait.Glasses, portrait.Hair, portrait.Mouth };

            foreach (var path in layers)
            {
                if (textures.TryGetValue(path, out var texture))
                {
                    Rectangle gridDestination = new(x, y, size, size);
                    DrawTexturePro(texture,
                        new Rectangle(0, 0, texture.Width, texture.Height),
                        gridDestination,
                        Vector2.Zero,
                        0f,
                        portrait.IsEliminated ? new Color(0, 0, 0, 255) : Color.White);
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
