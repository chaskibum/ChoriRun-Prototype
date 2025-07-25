using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
public class GameManagerBeta : MonoBehaviour
{
    private static readonly int GameStarted = Animator.StringToHash("GameStarted");
    [Header("Properties")]
    [SerializeField] float SpeedIncreseAmount = 1;
    [SerializeField] int TimeTillIncrese = 1;
    public bool isGamePaused;

    [Header("Probabilitys")]
    [SerializeField] float obstacleProbability = 0;
    [SerializeField] float ingredientProbability = 0;
    [SerializeField] float badIngredientProbability = 0;
    [SerializeField] float powerUpProbability = 0;
    [Header("Variables")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Transform livesContainer;
    [SerializeField] Transform ingredientsContainer;
    [SerializeField] Animator ChoriPanAnimator;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] bool gameOver; 
    [SerializeField] Animator animator;
    [SerializeField] bool fastLoopActivated = false;
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

    float _timeScoreRepeatRate;
    int _score;
    bool _hasAllIngredients;
    ItemsManager itemsManager;
    PauseMenu pauseMenu;
    bool gameStarted;
    UnityEvent onRestartGame = new UnityEvent();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        itemsManager = FindFirstObjectByType<ItemsManager>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }
    public void StartGame()
    {
        InvokeRepeating(nameof(TimeScore), 0, _timeScoreRepeatRate);
        livesContainer.gameObject.SetActive(true);
        mainMenuPanel.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        gameStarted = true;
        pauseMenu.OnStart();
        animator.SetBool(GameStarted, true);
    }
    public void SortItems(Transform objectToSortContainer, int SpaceBetweenObjects = 0, bool RandomIndex = false)
    {
        float currentX = 0f;
        if (RandomIndex)
        {
            foreach (Transform ObjectInContainer in objectToSortContainer)
            {
                ObjectInContainer.SetSiblingIndex(Random.Range(0, objectToSortContainer.childCount));
            }
        }

        for (int i = 0; i < objectToSortContainer.childCount; i++)
        {
            Transform ObjectInContainer = objectToSortContainer.GetChild(i);

            BoxCollider2D objectCollider = ObjectInContainer.GetComponent<BoxCollider2D>();

            float obstacleCenter = currentX + (objectCollider.size.x / 2f) - objectCollider.offset.x;

            ObjectInContainer.localPosition = new Vector2(obstacleCenter, 0);

            currentX += objectCollider.size.x + SpaceBetweenObjects;
        }
    }

    public void Restart()
    {
        onRestartGame?.Invoke();
        //Resetear puntos
    }
    public void TimeScore()
    {
        if (itemsManager.GetItemsGroupSpeed < itemsManager.GetMaxItemsGroupSpeed)
        {
            _score++;
            _timeScoreRepeatRate = 0.5f;
        }
        else
        {
            _score++;
            _timeScoreRepeatRate = 0.2f;
        }

        scoreText.text = _score.ToString();
    }

    public void AddScore(int points = 1)
    {
        _score += points;
        scoreText.text = _score.ToString();
    }

    public void HpFeedback(int hp)
    {
        livesContainer.GetChild(hp).gameObject.SetActive(false);
    }

    void TurnMainMenuOn()
    {
        mainMenuPanel.SetActive(true);
    }

    public void ChoriFeedback(int ingredient)
    {
        if (ingredientsContainer.GetChild(0).gameObject.GetComponent<Image>().color == Color.white &&
            ingredient == 0)
            ingredientsContainer.GetChild(4).gameObject.GetComponent<Image>().color = Color.white;
        ingredientsContainer.GetChild(ingredient).gameObject.GetComponent<Image>().color = Color.white;
        ingredientSound.Play();

        foreach (Transform child in ingredientsContainer)
        {
            if (child.gameObject.GetComponent<Image>().color == Color.black)
            {
                _hasAllIngredients = false;
                break;
            }

            _hasAllIngredients = true;
        }

        if (_hasAllIngredients)
        {
            MakeChori();
            _hasAllIngredients = false;
        }
    }

    void MakeChori()
    {
        foreach (Transform child in ingredientsContainer)
        {
            child.gameObject.GetComponent<Image>().color = Color.black;
        }

        ChoriPanAnimator.Play("ChoriPanCompleted", 0, 0);
        AnimatorStateInfo stateInfo = ChoriPanAnimator.GetCurrentAnimatorStateInfo(0);
        Invoke("AddScoreAfterAnim", stateInfo.length);
    }

    void AddScoreAfterAnim()
    {
        AddScore(250);
        choriSound.Play();
    }

    public void BackToMenu()
    {
        gameStarted = false;
        // player.GetComponent<CircleCollider2D>().enabled = false;
        animator.SetBool(GameStarted, false);
        CancelInvoke();
        StopAllCoroutines();
        livesContainer.gameObject.SetActive(false);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Invoke("TurnMainMenuOn", stateInfo.length);
        ingredientsContainer.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        motorbikeSound.Stop();
        BackToMenuMusic();
    }
    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        motorbikeSound.Stop();
        ingredientsContainer.gameObject.SetActive(false);
        StopMusic();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    #region Public variables
    public float GetObstacleProbability => obstacleProbability;
    public float GetIngredientProbability => ingredientProbability;
    public float GetBadIngredientProbability => badIngredientProbability;
    public float GetPowerupProbability => powerUpProbability;
    public float GetItemsGroupSpeed => itemsManager.GetItemsGroupSpeed;
    public float GetSpeedIncreseAmount => SpeedIncreseAmount;
    public float GetTimeTillIncrese => TimeTillIncrese;
    public UnityEvent GetOnRestartEvent => onRestartGame;
    #endregion
    
    #region Audio

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
        
        #endregion

}
