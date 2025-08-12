using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class GameManagerBeta : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float SpeedIncreseAmount = 1;
    [SerializeField] float TimeTillIncrese = 1;
    [SerializeField] float TimeTillIncreseOnWheelie;

    [Header("Probabilities")]
    [SerializeField] float obstacleProbability = 0;
    [SerializeField] float ingredientProbability = 0;
    [SerializeField] float badIngredientProbability = 0;
    [SerializeField] float powerUpProbability = 0;

    int score;

    float timeScoreRepeatRate = 0.5f;
    float StartFOV;
    float StartValueTimeTillIncrese;

    bool gameStarted = false;
    public bool isGamePaused;

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
        StartFOV = Camera.main.orthographicSize;
    }

    void StartGame()
    {
        player.GetComponent<CapsuleCollider2D>().enabled = true;
        player.InvokeActiveWheelie();
        player.ResetAnimationSpeed();
        // InvokeRepeating("TimeScore", 0, timeScoreRepeatRate);
        StartCoroutine("TimeScore");
        gameStarted = true;
        onRestartGame.AddListener(ResetScore);
        Cursor.visible = false;
    }


    public void RestartButtonPressed()
    {
        onRestartGame?.Invoke();
        uiManager.StartRowOfActivates();
        AudioManager.Instance.OnRestartButtonPressed();
        StopCoroutine(ChangeCameraFOV());
        player.ResetAnimationSpeed();
        Camera.main.orthographicSize = 8;
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
        StopCoroutine("TimeScore");
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
    public IEnumerator ChangeCameraFOV()
    {
        float TargetFOV = 9;
        while (player.IsPlayerInvincible)
        {
            if (Camera.main.orthographicSize != TargetFOV) Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, TargetFOV, 2 * Time.deltaTime);
            yield return null;
        }
        while (Camera.main.orthographicSize != StartFOV)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, StartFOV, Time.deltaTime);
            yield return null;
        }
    }

    #region  Score

    IEnumerator TimeScore()
    {
        while (true)
        {
            if (itemsManager.GetItemsGroupSpeed >= itemsManager.GetMaxItemsGroupSpeed)
            {
                timeScoreRepeatRate = 0.1f;
            }
            else
            {
                if (player.IsPlayerWheeling || player.IsPlayerInvincible)
                {
                    timeScoreRepeatRate = 0.2f;
                }
                else
                {
                    timeScoreRepeatRate = 0.5f;
                }
            }
            score++;
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
    public int GetScore => score;
    #endregion
}
