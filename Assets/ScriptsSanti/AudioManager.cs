using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource motorbikeSound;
    public AudioSource ingredientSound;
    public AudioSource choriSound;
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioSource startLoop;
    public AudioSource mainMenuSong;
    public AudioSource startSong;
    public AudioSource songLoop;
    public AudioSource melodyLoop;
    public AudioSource fastMelodyLoop;
    [SerializeField] bool fastLoopActivated = false;
    GameManagerBeta gameManager;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    void Start()
    {
        gameManager.GetOnstartEvent?.AddListener(OnStart);
        gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);
    }
    public void ChangeMusicVolume()
    {
        mixer.SetFloat("MusicVolume", musicSlider.value);
    }

    public void ChangeSfxVolume()
    {
        mixer.SetFloat("SFXVolume", sfxSlider.value);
    }

    IEnumerator FadeMusic(AudioSource audioSource, bool fadeIn)
    {
        if (fadeIn)
        {
            while (audioSource.volume < 1)
            {
                audioSource.volume += 0.02f;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= 0.02f;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void StartSong()
    {
        startSong.volume = 1f;
        startSong.Play();
        startLoop.Stop();
    }

    public void StartSongLoop()
    {
        songLoop.volume = 1f;
        melodyLoop.volume = 1f;
        songLoop.Play();
        melodyLoop.Play();
        fastMelodyLoop.Play();
    }

    public void FastLoop()
    {
        StartCoroutine(FadeMusic(melodyLoop, false));
        StartCoroutine(FadeMusic(fastMelodyLoop, true));
        fastLoopActivated = true;
    }

    public void CueMusic()
    {
        if (!startLoop.isPlaying) startLoop.Play();

        StartCoroutine(FadeMusic(startLoop, true));
        StartCoroutine(FadeMusic(mainMenuSong, false));
        Invoke("StartSong", (startLoop.clip.length - startLoop.time));
        Invoke("StartSongLoop", startSong.clip.length + (startLoop.clip.length - startLoop.time));
    }

    public void StopMusic()
    {
        CancelInvoke("StartSongLoop");
        CancelInvoke("StartSong");
        startLoop.Stop();
        mainMenuSong.Stop();
        startSong.Stop();
        songLoop.Stop();
        melodyLoop.Stop();
        fastMelodyLoop.Stop();
    }

    public void BackToMenuMusic()
    {
        mainMenuSong.Play();
        StartCoroutine(FadeMusic(mainMenuSong, true));
        StartCoroutine(FadeMusic(startLoop, false));
        StartCoroutine(FadeMusic(startSong, false));
        StartCoroutine(FadeMusic(songLoop, false));
        StartCoroutine(FadeMusic(melodyLoop, false));
        StartCoroutine(FadeMusic(fastMelodyLoop, false));
    }
    void OnStart()
    {
        CueMusic();
    }
    public void OnRestartButtonPressed()
    {
        CueMusic();
    }
    void OnQuit()
    {
        motorbikeSound.Stop();
        BackToMenuMusic();
    }
}
