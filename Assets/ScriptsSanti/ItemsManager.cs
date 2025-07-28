using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] Transform ItemGroupContainer;
    [SerializeField] float ItemsSpeed;
    float SpeedTarget = 0;
    float startItemsSpeed;
    float originalValue = 0;
    [SerializeField] int MaxItemsSpeed;
    [Min(1), SerializeField] int SpaceBetweenItemsGroup = 5;

    [Header("Rare Items Group")]
    [SerializeField] Transform RareItemsGroupContainer;
    [SerializeField] float TimeTillNextRareGroup;
    List<Transform> RareGroupsList = new List<Transform>();
    [SerializeField] List<Sprite> ObstaclesSprites;
    [SerializeField] List<Sprite> IngredientsSprites;
    [SerializeField] List<Sprite> BadIngredientSprites;
    [SerializeField] List<Sprite> PowerupSprites;
    [SerializeField] List<Sprite> PowerupAnimationSprites;
    GameManagerBeta gameManager;
    AudioManager audioManager;
    Coroutine SpeedCoroutine;
    Coroutine ReturnToDefault;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startItemsSpeed = ItemsSpeed;
        gameManager.GetOnstartEvent?.AddListener(OnStart);
        gameManager.GetOnRestartEvent?.AddListener(OnRestart);
        gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);
    }
    public void SortItemsGroup()
    {
        gameManager.SortItems(ItemGroupContainer, SpaceBetweenItemsGroup, true);
    }
    void RestartItems()
    {
        ItemsSpeed = startItemsSpeed;
        foreach (Transform rareGroup in RareGroupsList)
        {
            rareGroup.parent = RareItemsGroupContainer;
            rareGroup.gameObject.SetActive(false);
        }
        foreach (Transform ItemsGroup in ItemGroupContainer)
        {
            ItemsGroup.GetComponent<ItemGroupBehavior>().RefreshItemType();
        }
    }
    IEnumerator AddRareGroupToRun()
    {
        while (true)
        {
            if (RareItemsGroupContainer.childCount > 0)
            {
                yield return new WaitForSeconds(TimeTillNextRareGroup);
                Transform GroupToAdd = RareItemsGroupContainer.GetChild(Random.Range(0, RareItemsGroupContainer.childCount));
                Transform WhereToAdd = ItemGroupContainer.GetChild(Random.Range(4, ItemGroupContainer.childCount));
                GroupToAdd.parent = ItemGroupContainer;
                GroupToAdd.SetSiblingIndex(WhereToAdd.GetSiblingIndex());
                GroupToAdd.position = WhereToAdd.position;
                GroupToAdd.gameObject.SetActive(true);
                yield return new WaitUntil(() => !GroupToAdd.gameObject.activeSelf);
            }
            yield return null;
        }
    }
    IEnumerator GraduallyIncreaseSpeed()
    {
        while (ItemsSpeed < MaxItemsSpeed)
        {
            float timer = 0;
            while (timer < gameManager.GetTimeTillIncrese)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            SpeedTarget = ItemsSpeed + gameManager.GetSpeedIncreseAmount;

            while (!Mathf.Approximately(ItemsSpeed, SpeedTarget))
            {
                ItemsSpeed = Mathf.MoveTowards(ItemsSpeed, SpeedTarget, Time.deltaTime);
                yield return null;
            }
        }
    }
    void StartAllCoroutines()
    {
        SpeedCoroutine = StartCoroutine(GraduallyIncreaseSpeed());
        StartCoroutine(AddRareGroupToRun());
    }

    public void ChangeItemsSpeed(int SpeedToAdd, float DurationTime)
    {
        if (ReturnToDefault != null)
        {
            ItemsSpeed = originalValue;
            StopCoroutine(ReturnToDefault);
        }
        ItemsSpeed = SpeedTarget;
        originalValue = ItemsSpeed;
        ItemsSpeed += SpeedToAdd;
        ReturnToDefault = StartCoroutine(ReturnToDefaultValue(originalValue, DurationTime));
    }
    IEnumerator ReturnToDefaultValue(float ValueBeforeChange, float DurationTime)
    {

        StopCoroutine(SpeedCoroutine);

        yield return new WaitForSeconds(DurationTime);

        while (!Mathf.Approximately(ItemsSpeed, ValueBeforeChange))
        {
            ItemsSpeed = Mathf.MoveTowards(ItemsSpeed, ValueBeforeChange, Time.deltaTime * 3);

            yield return null;
        }

        SpeedCoroutine = StartCoroutine(GraduallyIncreaseSpeed());

        ReturnToDefault = null;
    }
    public void ReturnRareGroupToParent(Transform RareGroup, Transform RareGroupContainer)
    {
        StartCoroutine(ReturnGroupToParent(RareGroup, RareGroupContainer));
    }
    IEnumerator ReturnGroupToParent(Transform RareGroup, Transform RareGroupContainer)
    {
        float timer = 0;
        while (timer < TimeTillNextRareGroup + 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        RareGroup.parent = RareGroupContainer;
    }
    void OnStart()
    {
        ItemsSpeed = startItemsSpeed;
        SortItemsGroup();
        RestartItems();
        StartAllCoroutines();
        foreach (Transform rareGroup in RareItemsGroupContainer)
        {
            RareGroupsList.Add(rareGroup);
        }
    }

    void OnRestart()
    {
        StopAllCoroutines();
        RestartItems();
        gameManager.SortItems(ItemGroupContainer, SpaceBetweenItemsGroup, true);
        ReturnToDefault = null;
        StartAllCoroutines();
    }

    void OnQuit()
    {
        StopAllCoroutines();
    }
    #region Public variables
    public Transform GetObstaclesContainer => ItemGroupContainer;
    public float GetItemsGroupSpeed => ItemsSpeed;
    public float GetMaxItemsGroupSpeed => MaxItemsSpeed;
    public float GetSpaceBtwGroups => SpaceBetweenItemsGroup;
    public List<Sprite> GetObstaclesSprites => ObstaclesSprites;
    public List<Sprite> GetIngredientsSprites => IngredientsSprites;
    public List<Sprite> GetBadIngredientsSprites => BadIngredientSprites;
    public List<Sprite> GetPowerUpSprites => PowerupSprites;
    public List<Sprite> GetPowerUpAnimationSprites => PowerupAnimationSprites;
    #endregion
}
