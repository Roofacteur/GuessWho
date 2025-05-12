using GuessWho;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static GuessWho.GameManager;

public class SoundManager
{
    private GameState previousState = GameState.None;
    private Music backgroundMusic;
    private bool isMusicPlaying = false;
    private string currentMusicPath = "";

    public void SoundsLoader(GameManager gamemanager)
    {
        GameState state = gamemanager.CurrentState;
        string newMusicPath = "";

        switch (state)
        {
            case GameState.InGame:
                newMusicPath = "assets/sfx/InGame.mp3";
                break;
            case GameState.Menu:
            case GameState.Options:
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
        PlayMusicStream(backgroundMusic);
        isMusicPlaying = true;
        currentMusicPath = newMusicPath;
        previousState = state;
    }

    public void UpdateMusic()
    {
        if (isMusicPlaying)
            UpdateMusicStream(backgroundMusic);
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
}
