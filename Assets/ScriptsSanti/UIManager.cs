using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] Transform livesContainer;
    [SerializeField] Transform ingredientsContainer;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Transform IngredientsPosition;
    [SerializeField] Transform AnimatorsContainer;
    [SerializeField] Color DisableColor;

    [Header("Text")]
    [SerializeField] TMP_Text scoreText;

    [Header("Animators")]
    [SerializeField] Animator ChoriPanAnimator;
    [SerializeField] Animator CameraAnimator;

    List<int> IndexList = new();
    bool _hasAllIngredients;
    GameManagerBeta gameManager;
    ItemsManager itemsManager;
    PlayerBeta player;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        itemsManager = FindFirstObjectByType<ItemsManager>();
        player = FindFirstObjectByType<PlayerBeta>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.GetOnRestartEvent?.AddListener(OnRestart);
        gameManager.GetOnstartEvent?.AddListener(OnStart);
        gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);

        foreach (Transform Child in ingredientsContainer)
        {
            Child.gameObject.GetComponent<Image>().color = DisableColor;
        }
    }
    public void OnPlayButtonPresed()
    {
        CameraAnimator.SetBool("GameStarted", true);
        mainMenuPanel.gameObject.SetActive(false);
        AnimatorStateInfo stateInfo = CameraAnimator.GetCurrentAnimatorStateInfo(0);
        Invoke("StartGame", stateInfo.length - 0.3f);
    } 
    void StartGame()
    {
        gameManager.GetOnstartEvent?.Invoke();
    }
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
    public void HpFeedback(int hp)
    {
        livesContainer.GetChild(hp).gameObject.SetActive(false);
    }

    public void ChoriFeedback(int ingredient)
    {
        Image IngredientImage (int index){ return ingredientsContainer.GetChild(index).gameObject.GetComponent<Image>(); }

        if (IngredientImage(0).color == Color.white && ingredient == 0) ingredientsContainer.GetChild(4).gameObject.GetComponent<Image>().color = Color.white;
            
        IngredientImage(ingredient).color = Color.white;

        foreach (Transform child in ingredientsContainer)
        {
            if (child.gameObject.GetComponent<Image>().color == DisableColor)
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
            child.gameObject.GetComponent<Image>().color = DisableColor;
        }
        ingredientsContainer.gameObject.SetActive(false);
        ChoriPanAnimator.Play("ChoriPanCompleted", 0, 0);
        AnimatorStateInfo stateInfo = ChoriPanAnimator.GetCurrentAnimatorStateInfo(0);
        Invoke("AddScoreAfterAnim", stateInfo.length);
    }

    void AddScoreAfterAnim()
    {
        ingredientsContainer.gameObject.SetActive(true);
        gameManager.AddScoreChoriPan();
        Invoke("ClearIndexList", 0.3f);
    }
    void ClearIndexList()
    {
        IndexList.Clear();
    }

    public void BackToMenu()
    {
        gameManager.GetOnQuitButtonEvent?.Invoke();
    }

    void TurnMainMenuOn()
    {
        player.ResetPosition();
        gameManager.ResetScore();
        itemsManager.SortItemsGroup();
        mainMenuPanel.SetActive(true);
        gameManager.SetGameStarted(false);
    }
    
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void StartRowOfActivates()
    {
        StartCoroutine(RowOfActivates(livesContainer.gameObject, scoreText.gameObject, ingredientsContainer.gameObject));
    }
    IEnumerator RowOfActivates(GameObject livesContainer, GameObject ScoreText, GameObject ingredientsContainer)
    {
        List<GameObject> gameObjects = new List<GameObject> { livesContainer, ScoreText, ingredientsContainer };
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
        while (true)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(true);
                AnimatorStateInfo stateInfo = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                yield return new WaitForSeconds(stateInfo.length - 0.2f);
            }
            yield break;
        }
    }
    public void PickupIngredient(Vector3 StartPos, int PositionIndex, int ScoreToAdd, Sprite sprite)
    {
        Sprite IngredientSprite = sprite;
        bool goToScore;

        if (PositionIndex == 0 && IndexList.Contains(PositionIndex)) PositionIndex = 4;


        Vector3 TargetPostion = IngredientsPosition.GetChild(PositionIndex).transform.position;
        IngredientAnimation AnimationScript = AnimatorsContainer.GetChild(0)?.GetComponent<IngredientAnimation>();

        if (!IndexList.Contains(PositionIndex)) goToScore = false;
        else
        {
            Vector3 ScorePosition = IngredientsPosition.GetChild(IngredientsPosition.childCount - 1).transform.position;
            TargetPostion = ScorePosition;
            goToScore = true;
        }

        AnimationScript.AnimateObject(IngredientSprite, StartPos, TargetPostion, goToScore);
        StartCoroutine(CheckReachedTargetPos(AnimationScript, PositionIndex, ScoreToAdd, goToScore));
        
        if (!IndexList.Contains(PositionIndex)) AddToIndexList(PositionIndex);
    }

    public void AddToIndexList(int indexToAdd)
    {
        IndexList.Add(indexToAdd);
    }

    IEnumerator CheckReachedTargetPos(IngredientAnimation AnimationScript, int SpriteIndex, int ScoreToAdd, bool isScoreTarget = false)
    {
        while (true)
        {
            if (Vector3.Distance(AnimationScript.transform.position, AnimationScript.GetTargetPos) < 0.1f)
            {
                if (!isScoreTarget) ChoriFeedback(SpriteIndex);
                gameManager.AddScore(ScoreToAdd);

                yield break;
            }
            yield return null;
        }
    }

    void ResetLivesAndIngredientsContainer()
    {
        foreach (Transform Child in ingredientsContainer)
        {
            Child.gameObject.GetComponent<Image>().color = DisableColor;
        }
        foreach (Transform Child in livesContainer)
        {
            Child.gameObject.SetActive(true);
        }
    }

    void OnStart()
    {
        StartRowOfActivates();
    }

    void OnRestart()
    {
        ChoriPanAnimator.Rebind();
        CancelInvoke();
        ClearIndexList();
        StopAllCoroutines();
        ResetLivesAndIngredientsContainer();
    }

    void OnQuit()
    {
        CancelInvoke();
        StopAllCoroutines();
        ResetLivesAndIngredientsContainer();
        ClearIndexList();

        livesContainer.gameObject.SetActive(false);
        ingredientsContainer.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        CameraAnimator.SetBool("GameStarted", false);

        AnimatorStateInfo stateInfo = CameraAnimator.GetCurrentAnimatorStateInfo(0);
        Invoke("TurnMainMenuOn", stateInfo.length);
    }
}
