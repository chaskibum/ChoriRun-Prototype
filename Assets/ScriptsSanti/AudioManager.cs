using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioMixer mixer;
    public AudioSource sfxSource;
    
    public AudioSource mainMenuSong;
    public AudioSource startLoop;
    public AudioSource startSong;
    public AudioSource songLoop;
    public AudioSource melodyLoop;
    public AudioSource fastMelodyLoop;
    public AudioSource motorbikeSound;

    public enum AudioList
    {
        IngredientSound,
        ChoriSound,
        PowerUpSound,
        DestroyObstacleSound,
        GetHitSound,
        GameOverMelody,
        Splat,
        PuajSound,
        ButtonPressed,
    }
    
	[SerializeField] List<AudioClip> audioClips;
    [SerializeField] bool fastLoopActivated = false;
    private GameManagerBeta _gameManager;
    
    // Makes the audio manager a singleton
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManagerBeta>();
        
        // Makes the audio manager a singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("AudioManager is a Singleton: An instance already exists");
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayClip(AudioList clip, bool changePitch = false, float volume = 1f, bool oneShot = true)
    {
        sfxSource.volume = volume;
        sfxSource.pitch = changePitch ? Random.Range(0.8f, 1.2f) : 1f;
        if (oneShot)
        {
            sfxSource.PlayOneShot(audioClips[(int)clip]);
        }
        else
        {
            sfxSource.clip = audioClips[(int)clip];
            sfxSource.Play();
        }
        sfxSource.loop = oneShot;
    }

    public void StopClip()
    {
        sfxSource.Stop();
        sfxSource.loop = false;
    }
    
    void Start()
    {
        _gameManager.GetOnstartEvent?.AddListener(OnStart);
        _gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);
    }
    
    public void ChangeMusicVolume()
    {
        mixer.SetFloat("MusicVolume", musicSlider.value);
    }

    public void ChangeSfxVolume()
    {
        mixer.SetFloat("SFXVolume", sfxSlider.value);
        PlayClip(AudioList.ButtonPressed, true, 1f, false);
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

    public void PlayFastLoop()
    {
        StartCoroutine(FadeMusic(melodyLoop, false));
        StartCoroutine(FadeMusic(fastMelodyLoop, true));
        fastLoopActivated = true;
    }

    public void CueMusic()
    {
        mainMenuSong.volume = 1f;
        startSong.volume = 1f;
        songLoop.volume = 1f;
        melodyLoop.volume = 1f;
        fastMelodyLoop.volume = 0f;
        
        if (!startLoop.isPlaying) startLoop.Play();

        StartCoroutine(FadeMusic(startLoop, true));
        StartCoroutine(FadeMusic(mainMenuSong, false));
        
        StartCoroutine(RealTimeCoroutine("StartSong", (startLoop.clip.length - startLoop.time) - 0.05f));
        StartCoroutine(RealTimeCoroutine("StartSongLoop", startSong.clip.length + (startLoop.clip.length - startLoop.time) - 0.05f));
    }
    
    // Plays the song despite the game being paused
    IEnumerator RealTimeCoroutine(string function, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Invoke(function, 0);
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        startLoop.Stop();
        mainMenuSong.Stop();
        startSong.Stop();
        songLoop.Stop();
        melodyLoop.Stop();
        fastMelodyLoop.Stop();
    }

    public void BackToMenuMusic()
    {
        StopMusic();
        mainMenuSong.Play();
        startLoop.volume = 0f;
        startLoop.Play();
        StartCoroutine(FadeMusic(mainMenuSong, true));
    }
    
    void OnStart()
    {
        CueMusic();
        motorbikeSound.Play();
        InvokeRepeating("IncreaseMotorPitch", 0, 1f);
    }
    
    public void OnRestartButtonPressed()
    {
        motorbikeSound.pitch = 1f;
        StopMusic();
        CueMusic();
    }
    
    void OnQuit()
    {
        motorbikeSound.Stop();
        BackToMenuMusic();
    }

    void IncreaseMotorPitch()
    {
        if (motorbikeSound.pitch > 2.2f) return;
        motorbikeSound.pitch += 0.005f;
    }

    public void PlayButtonPressed()
    {
        PlayClip(AudioList.ButtonPressed, true);
    }

    public void PlayOpenMenuSound()
    {
        PlayClip(AudioList.Splat);
    }
}
