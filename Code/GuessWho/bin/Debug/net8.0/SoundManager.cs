using GuessWho;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static GuessWho.GameManager;

public class SoundManager
{
    private GameState previousState = GameState.None;
    private Music backgroundMusic;
    public Sound flickSound = new Sound();
    public Sound restartSound = new Sound();
    public bool isMusicPlaying = false;
    public int MusicVolume = 40;
    public int SfxVolume = 40;
    private string currentMusicPath = "";

    public void SoundsLoader(GameManager gamemanager)
    {
        GameState state = gamemanager.CurrentState;
        string newMusicPath = "";
        string flickSoundPath = "assets/sfx/flick.mp3";
        string restartSoundPath = "assets/sfx/restart.mp3";

        switch (state)
        {
            case GameState.InGame:
                newMusicPath = "assets/sfx/InGame2.mp3";
                break;
            case GameState.Menu:
            case GameState.Options:
                newMusicPath = "assets/sfx/MainMenu.mp3";
                break;
            case GameState.Generation:
            case GameState.Creating:
            case GameState.Victory:
                newMusicPath = "assets/sfx/MainMenu.mp3";
                break;
        }

        if (newMusicPath == currentMusicPath && IsMusicStreamPlaying(backgroundMusic))
        {
            UpdateMusic();
            return;
        }

        if (isMusicPlaying)
        {
            StopMusicStream(backgroundMusic);
            UnloadMusicStream(backgroundMusic);
            isMusicPlaying = false;
        }

        backgroundMusic = LoadMusicStream(newMusicPath);
        flickSound = LoadSound(flickSoundPath);
        restartSound = LoadSound(restartSoundPath);

        SetMusicVolume(backgroundMusic, MusicVolume / 100.0f);
        PlayMusicStream(backgroundMusic);

        UpdateSFX();

        isMusicPlaying = true;
        currentMusicPath = newMusicPath;
        previousState = state;
    }

    public void UpdateMusic()
    {
        if (isMusicPlaying)
        {
            UpdateMusicStream(backgroundMusic);
            SetMusicVolume(backgroundMusic, MusicVolume / 100.0f);
        }
    }

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
    public void UpdateSFX()
    {
        SetSoundVolume(flickSound, SfxVolume / 100.0f);
        SetSoundVolume(restartSound, SfxVolume / 100.0f);
    }
    public void StopSFX()
    {
        StopSound(flickSound);
        StopSound(restartSound);
    }
}
