using UnityEngine;

public class ItemGroupBehavior : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] Transform RareGroupContainer;
    [SerializeField] bool isRareGroup;
    [SerializeField] bool destroyOnLimit;
    ItemsManager itemsManager;
    GameManagerBeta gameManager;

    [Header("Obstacles")]
    [SerializeField] int ObstacleAmount;
    [SerializeField] int MaxObstaclesPerGroup;

    [Header("Ingredient")]
    [SerializeField] int IngredientsAmount;
    [SerializeField] int MaxIngredientsPerGroup;

    [Header("Bad Ingredient")]
    [SerializeField] int BadIngredientAmount;
    [SerializeField] int MaxBadIngredientsPerGroup;

    [Header("Power up")]
    [SerializeField] int PowerupsAmount;
    [SerializeField] int MaxPowerupPerGroup;
    void Awake()
    {
        itemsManager = FindFirstObjectByType<ItemsManager>();
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    void Start()
    {
        if (isRareGroup)
        {
            itemsManager.AddRareGroupToList(transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetGameStarted)
        {
            transform.position += Vector3.left * itemsManager.GetItemsGroupSpeed * Time.deltaTime;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("ObstacleGroup"))
        {
            SetPosition(transform.GetSiblingIndex());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Limit"))
        {
            if (!isRareGroup)
            {
                RefreshItemType();
                int index = Random.Range(4, transform.parent.childCount);
                transform.SetSiblingIndex(index);
                SetPosition(index);
            }
            else
            {
                itemsManager.ReturnRareGroupToParent(transform, RareGroupContainer);
                transform.gameObject.SetActive(false);
            }
        }
    }

    private void SetPosition(int index)
    {
        float GetPreviousRightEdge(int currentIndex)
        {
            Transform ChildTransform = transform.parent.GetChild(currentIndex - 1);
            var ChildBoxCollider = ChildTransform.GetComponent<BoxCollider2D>();

            return ChildTransform.localPosition.x + ChildBoxCollider.offset.x + (ChildBoxCollider.size.x / 2f);
        }
        Vector2 CalculateNewPosition(float PreviousGroupRightEdge)
        {
            var ChildBoxCollider = transform.parent.GetChild(index).GetComponent<BoxCollider2D>();
            float LeftEdge = (ChildBoxCollider.size.x / 2f) - ChildBoxCollider.offset.x;
            float newXPos = PreviousGroupRightEdge + itemsManager.GetSpaceBtwGroups + LeftEdge;

            return new Vector2(newXPos, 0);
        }

        if (index - 1 >= 0)
        {
            transform.localPosition = CalculateNewPosition(GetPreviousRightEdge(index));

        }
        else
        {
            transform.localPosition = CalculateNewPosition(GetPreviousRightEdge(transform.parent.childCount));

        }
    }
    public void RefreshItemType()
    {
        ObstacleAmount = 0;
        IngredientsAmount = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<ItemBehavior>().SetRandomItemType();
            child.GetComponent<ItemBehavior>().PickObstacleType();
        }
        if (destroyOnLimit)
        {
            Destroy(gameObject);
        }
    }
    public void IncrementObstacleCount()
    {
        ObstacleAmount++;
    }
    public void IncrementIngredientCount()
    {
        IngredientsAmount++;
    }
    public void IncrementBadIngredientCount()
    {
        BadIngredientAmount++;
    }
    public void IncrementPowerUpCount()
    {
        PowerupsAmount++;
    }
    #region Public variables
    public int GetObstacleAmount => ObstacleAmount;
    public int GetIngredientsAmount => IngredientsAmount;
    public int GetBadIngredientAmount => BadIngredientAmount;
    public int GetPowerupAmount => PowerupsAmount;
    public int GetMaxObstaclesPerGroup => MaxObstaclesPerGroup;
    public int GetMaxIngredientPerGroup => MaxIngredientsPerGroup;
    public int GetMaxBadIngredientPerGroup => MaxBadIngredientsPerGroup;
    public int GetMaxPowerUpPerGroup => MaxPowerupPerGroup; 
    #endregion
}
