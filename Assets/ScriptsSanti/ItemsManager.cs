using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] Transform ItemGroupContainer;
    [SerializeField] float ItemsSpeed;
    float startItemsSpeed;
    [SerializeField] int MaxItemsSpeed;
    [Min(1), SerializeField] int SpaceBetweenItemsGroup = 5;

    [Header("Rare Items Group")]
    [SerializeField] Transform RareItemsGroupContainer;
    [SerializeField] float TimeTillNextRareGroup;
    List<Transform> RareGroupsList = new List<Transform>();
    GameManagerBeta gameManager;

    [Header("Items Sprites")]

    [SerializeField] List<Sprite> ObstaclesSprites;
    [SerializeField] List<Sprite> IngredientsSprites;
    [SerializeField] List<Sprite> BadIngredientSprites;
    [SerializeField] List<Sprite> PowerupSprites;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startItemsSpeed = ItemsSpeed;
        foreach (Transform rareGroup in RareItemsGroupContainer)
        {
            RareGroupsList.Add(rareGroup);
        }
        gameManager.SortItems(ItemGroupContainer, SpaceBetweenItemsGroup, true);
        StartAllCoroutines();
        gameManager.GetOnRestartEvent.AddListener(OnRestart);
    }
    // Update is called once per frame
    void Update()
    {

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
            float ActualSpeed = ItemsSpeed;
            while (!Mathf.Approximately(ItemsSpeed, ActualSpeed + gameManager.GetSpeedIncreseAmount))
            {
                ItemsSpeed = Mathf.MoveTowards(ItemsSpeed, ActualSpeed + gameManager.GetSpeedIncreseAmount, Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(gameManager.GetTimeTillIncrese);
        }
    }
    void StartAllCoroutines()
    {
        StartCoroutine(GraduallyIncreaseSpeed());
        StartCoroutine(AddRareGroupToRun());
    }

    void OnRestart()
    {
        StopAllCoroutines();
        gameManager.SortItems(ItemGroupContainer, SpaceBetweenItemsGroup, true);
        RestartItems();
        StartAllCoroutines();
    }

    public void ChangeItemsSpeed(int AmountToChange)
    {
        ItemsSpeed += AmountToChange;
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
    #endregion
}
