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
        public required string Gender, Hair, EyeColor, Beard, Mouth, Clothes, Glasses, Hat;
        public bool IsVisible = true;
        public Texture2D PortraitTexture;

        public bool MatchesCriteria(string criteria, string value)
        {
            return criteria switch
            {
                "gender" => Gender == value,
                "hair" => Hair == value,
                "eyeColor" => EyeColor == value,
                "beard" => Beard == value,
                "mouth" => Mouth == value,
                "clothes" => Clothes == value,
                "glasses" => Glasses == value,
                "hat" => Hat == value,
                _ => false
            };
        }

        public void Hide() => IsVisible = false;
        public void Reveal() => IsVisible = true;
    }
}
