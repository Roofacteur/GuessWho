using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessWho
{
    public class Character
    {
        public int Id;
        public required string Name;
        public required string Skin, Hair, Eyes, Eyebrows, Beard, Mouth, Clothes, Logo, Glasses;
        public bool IsVisible = true;
        public Texture2D PortraitTexture;

        public bool MatchesCriteria(string criteria, string value)
        {
            return criteria switch
            {
                "skin" => Skin == value,
                "hair" => Hair == value,
                "eyes" => Eyes == value,
                "eyebrows" => Eyebrows == value,
                "beard" => Beard == value,
                "mouth" => Mouth == value,
                "clothes" => Clothes == value,
                "logo" => Logo == value,
                "glasses" => Glasses == value,
                _ => false
            };
        }

        public void Hide() => IsVisible = false;
        public void Reveal() => IsVisible = true;
    }
}
