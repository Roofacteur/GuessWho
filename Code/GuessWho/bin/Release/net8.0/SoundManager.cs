using GuessWho;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static GuessWho.GameManager;

namespace GuessWho
{
    /// <summary>
    /// Gère l'ensemble des ressources sonores du jeu : musiques et effets sonores.
    /// </summary>
    public class SoundManager
    {
        #region Champs Privés

        private GameState previousState = GameState.None;
        private Music backgroundMusic;
        private string currentMusicPath = "";
        private bool isMusicPlaying = false;

        #endregion

        #region Propriétés Publiques

        public Sound flickSound = new();
        public Sound restartSound = new();
        public int MusicVolume = 40; // Pourcentage (0-100)
        public int SfxVolume = 40;   // Pourcentage (0-100)

        #endregion

        #region Chargement Dynamique

        /// <summary>
        /// Charge et joue la musique ou les effets en fonction de l'état du jeu.
        /// Optimisé pour éviter les rechargements inutiles.
        /// </summary>
        public void SoundsLoader(GameManager gameManager)
        {
            GameState state = gameManager.CurrentState;

            // Choix de la musique en fonction de l’état
            string newMusicPath;

            if (state == GameState.InGame)
            {
                newMusicPath = "assets/sfx/InGame2.mp3";
            }
            else if (state == GameState.Victory)
            {
                newMusicPath = "assets/sfx/Victory.mp3";
            }
            else if (new[] { GameState.Menu, GameState.Settings, GameState.Rules, GameState.Generation, GameState.Creating }.Contains(state))
            {
                newMusicPath = "assets/sfx/MainMenu.mp3";
            }
            else
            {
                newMusicPath = currentMusicPath;
            }


            // Si la musique est déjà en cours, mise à jour simple
            if (newMusicPath == currentMusicPath && IsMusicStreamPlaying(backgroundMusic))
            {
                UpdateMusic();
                return;
            }

            // Nettoyage de la musique précédente
            if (isMusicPlaying)
            {
                StopMusicStream(backgroundMusic);
                UnloadMusicStream(backgroundMusic);
                isMusicPlaying = false;
            }

            // Chargement de la nouvelle musique et effets
            backgroundMusic = LoadMusicStream(newMusicPath);
            flickSound = LoadSound("assets/sfx/flick.mp3");
            restartSound = LoadSound("assets/sfx/restart.mp3");

            SetMusicVolume(backgroundMusic, MusicVolume / 100f);
            PlayMusicStream(backgroundMusic);

            UpdateSFX();

            // Mise à jour des états internes
            isMusicPlaying = true;
            currentMusicPath = newMusicPath;
            previousState = state;
        }

        #endregion

        #region Mise à jour en temps réel

        /// <summary>
        /// Met à jour le flux musical en cours (appelé à chaque frame).
        /// </summary>
        public void UpdateMusic()
        {
            if (isMusicPlaying)
            {
                UpdateMusicStream(backgroundMusic);
                SetMusicVolume(backgroundMusic, MusicVolume / 100f);
            }
        }

        /// <summary>
        /// Met à jour le volume des effets sonores selon la configuration utilisateur.
        /// </summary>
        public void UpdateSFX()
        {
            SetSoundVolume(flickSound, SfxVolume / 100f);
            SetSoundVolume(restartSound, SfxVolume / 100f);
        }

        #endregion

        #region Arrêt & Libération

        /// <summary>
        /// Arrête la musique de fond et libère les ressources.
        /// </summary>
        public void StopMusic()
        {
            if (isMusicPlaying)
            {
                StopMusicStream(backgroundMusic);
                UnloadMusicStream(backgroundMusic);
                isMusicPlaying = false;
                currentMusicPath = "";
            }
        }

        /// <summary>
        /// Arrête les effets sonores actifs (utile lors des transitions ou coupures audio).
        /// </summary>
        public void StopSFX()
        {
            StopSound(flickSound);
            StopSound(restartSound);
        }

        #endregion
    }
}
