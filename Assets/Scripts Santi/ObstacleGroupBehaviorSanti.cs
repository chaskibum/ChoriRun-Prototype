using UnityEngine;

public class ObstacleGroupBehaviorSanti : MonoBehaviour
{
     // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameManagerSanti gameManager;
    public int ObstacleAmount;
    public int IngredientsAmount;
    public int MaxObstaclesPerGroup;
    public int MaxIngredientsPerGroup;
    [SerializeField] Transform RareGroupContainer;
    [SerializeField] bool isRandomizable;
    [SerializeField] bool isRareGroup;
    [SerializeField] bool destroyOnLimit;
    void Awake() 
    {
        gameManager = FindFirstObjectByType<GameManagerSanti>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * gameManager.ObstaclesSpeed * Time.deltaTime;
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
                int index = Random.Range(4, transform.parent.childCount);
                transform.SetSiblingIndex(index);
                SetPosition(index);
            }
            else
            {
                transform.parent = RareGroupContainer;
                transform.gameObject.SetActive(false);
            }
            RefreshObstacles();
        }
    }

    private void SetPosition(int index)
    {
        float GetPreviousRightEdge(int currentIndex)
        {
            Transform ChildTransform = transform.parent.GetChild(currentIndex - 1);

            return ChildTransform.localPosition.x + ChildTransform.GetComponent<BoxCollider2D>().offset.x + (ChildTransform.GetComponent<BoxCollider2D>().size.x / 2f);
        }
        Vector2 CalculateNewPosition(float PreviousRightEdge)
        {
            float newXPos = PreviousRightEdge + gameManager.SpaceBetweenObstaclesGroup + (transform.parent.GetChild(index).GetComponent<BoxCollider2D>().size.x / 2f) - transform.parent.GetChild(index).GetComponent<BoxCollider2D>().offset.x;


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

    public void RefreshObstacles()
    {
        if (isRandomizable)
        {
            ObstacleAmount = 0;
            IngredientsAmount = 0;
            foreach (Transform child in transform)
            {
                child.GetComponent<ObstacleBehaviorSanti>().SetRandomCollisionType();
                child.GetComponent<ObstacleBehaviorSanti>().PickObstacleType();
            }
        }
        else if (destroyOnLimit)
        {
            Destroy(gameObject);
        }
    }
}
