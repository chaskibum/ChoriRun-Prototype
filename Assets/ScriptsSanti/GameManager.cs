using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class GameManagerBeta : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float SpeedIncreseAmount = 1;
    [SerializeField] float TimeTillIncrese = 1;
    [SerializeField] float TimeTillIncreseOnWheelie;
    float StartValueTimeTillIncrese;
    public bool isGamePaused;

    [Header("Probabilities")]
    [SerializeField] float obstacleProbability = 0;
    [SerializeField] float ingredientProbability = 0;
    [SerializeField] float badIngredientProbability = 0;
    [SerializeField] float powerUpProbability = 0;

    float timeScoreRepeatRate = 0.5f;
    int score;
    bool gameStarted = false;
    bool StartFastLoop = false;

    ItemsManager itemsManager;
    UIManager uiManager;
    PlayerBeta player;

    UnityEvent onRestartGame = new UnityEvent();
    UnityEvent onStartGame = new UnityEvent();
    UnityEvent onQuitButton = new UnityEvent();
    
    void Awake()
    {
        itemsManager = FindFirstObjectByType<ItemsManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        player = FindFirstObjectByType<PlayerBeta>();
    }
    void Start()
    {
        onStartGame?.AddListener(StartGame);
        onQuitButton?.AddListener(OnQuit);
        StartValueTimeTillIncrese = TimeTillIncrese;
    }

    void StartGame()
    {
        player.GetComponent<CircleCollider2D>().enabled = true;
        player.InvokeActiveWheelie();
        player.ResetAnimationSpeed();
        // InvokeRepeating("TimeScore", 0, timeScoreRepeatRate);
        StartCoroutine("TimeScore");
        gameStarted = true;
        onRestartGame.AddListener(ResetScore);
        Cursor.visible = false;
    }

    public void SortItems(Transform objectToSortContainer, int SpaceBetweenObjects = 0, bool RandomIndex = false)
    {
        float currentX = 0f;
        if (RandomIndex)
        {
            foreach (Transform ObjectInContainer in objectToSortContainer)
            {
                BoxCollider2D objectCollider = ObjectInContainer.GetComponent<BoxCollider2D>();

                objectCollider.enabled = false;

                ObjectInContainer.SetSiblingIndex(Random.Range(0, objectToSortContainer.childCount));
                
                objectCollider.enabled = true;
            }
        }

        for (int i = 0; i < objectToSortContainer.childCount; i++)
        {
            Transform ObjectInContainer = objectToSortContainer.GetChild(i);

            BoxCollider2D objectCollider = ObjectInContainer.GetComponent<BoxCollider2D>();

            float obstacleCenter = currentX + (objectCollider.size.x / 2f) - objectCollider.offset.x;

            ObjectInContainer.localPosition = new Vector2(obstacleCenter, 0);

            currentX += objectCollider.size.x + SpaceBetweenObjects;

            objectCollider.enabled = true;
        }
    }

    public void RestartButtonPressed()
    {
        onRestartGame?.Invoke();
        uiManager.StartRowOfActivates();
        AudioManager.Instance.OnRestartButtonPressed();
        player.ResetAnimationSpeed();
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        AudioManager.Instance.motorbikeSound.volume = 0f;
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayClip(AudioManager.AudioList.GameOverMelody);
        uiManager.GameOver();
        Cursor.visible = true;
    }
    void OnQuit()
    {
        CancelInvoke();
        Cursor.visible = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void IncreseSpeedOnWheelie(bool isWheeling = false)
    {
        if (isWheeling)
        {
            TimeTillIncrese = TimeTillIncreseOnWheelie;
        }
        else
        {
            TimeTillIncrese = StartValueTimeTillIncrese;
        }
    }
    public void SetGameStarted(bool state)
    {
        gameStarted = state;
    }

    #region  Score
        /*void TimeScore()
        {
            if (player.onWheelie)
            {
                score++;
                timeScoreRepeatRate = 0.1f;
            }
            if (itemsManager.GetItemsGroupSpeed < itemsManager.GetMaxItemsGroupSpeed)
            {
                score++;
                timeScoreRepeatRate = 0.5f;
            }
            else
            {
                score++;
                timeScoreRepeatRate = 0.2f;
            }

            uiManager.UpdateScore(score);
        }*/

        IEnumerator TimeScore()
        {
            while (true)
            {
                if (itemsManager.GetItemsGroupSpeed >= itemsManager.GetMaxItemsGroupSpeed)
                {
                    score++;
                    timeScoreRepeatRate = 0.1f;
                }
                else
                {
                    if (player.onWheelie)
                    {
                        score++;
                        timeScoreRepeatRate = 0.2f;
                    }
                    else
                    {
                        score++;
                        timeScoreRepeatRate = 0.5f;
                    }
                }

                uiManager.UpdateScore(score);
                yield return new WaitForSeconds(timeScoreRepeatRate);
            }
        }

        public void AddScore(int points = 1)
        {
            score += points;
            uiManager.UpdateScore(score);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.IngredientSound, true, 0.4f);
        }

        public void AddScoreChoriPan()
        {
            AddScore(250);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ChoriSound);
            if (score > 10000) AudioManager.Instance.PlayFastLoop();
        }
        public void ResetScore()
        {
            score = 0;
            uiManager.UpdateScore(score);
        }

    #endregion

    #region Public variables
    public float GetObstacleProbability => obstacleProbability;
    public float GetIngredientProbability => ingredientProbability;
    public float GetBadIngredientProbability => badIngredientProbability;
    public float GetPowerupProbability => powerUpProbability;
    public float GetItemsGroupSpeed => itemsManager.GetItemsGroupSpeed;
    public float GetSpeedIncreseAmount => SpeedIncreseAmount;
    public float GetTimeTillIncrese => TimeTillIncrese;
    public UnityEvent GetOnRestartEvent => onRestartGame;
    public UnityEvent GetOnstartEvent => onStartGame;
    public UnityEvent GetOnQuitButtonEvent => onQuitButton;
    public bool GetGameStarted => gameStarted;
    #endregion
}
