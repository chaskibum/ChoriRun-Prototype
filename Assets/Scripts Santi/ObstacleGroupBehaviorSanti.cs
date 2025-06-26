using UnityEngine;

public class ObstacleGroupBehaviorSanti : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameManagerSanti GameManager;
    public int ObstacleAmount;
    public int IngredientsAmount;
    public int MaxObstaclesPerGroup;
    public int MaxIngredientsPerGroup;
    [SerializeField] bool isRandomizable;
    [SerializeField] bool destroyOnLimit;
    void Awake() 
    {
        GameManager = FindFirstObjectByType<GameManagerSanti>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * GameManager.Speed * Time.deltaTime;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Limit"))
        {
            int index = transform.GetSiblingIndex();
            if (index - 1 >= 0)
            {
                float previousRightEdge = transform.parent.GetChild(index - 1).localPosition.x + transform.parent.GetChild(index - 1).GetComponent<BoxCollider2D>().offset.x + (transform.parent.GetChild(index - 1).GetComponent<BoxCollider2D>().size.x /2f) ;

                float newXPos = previousRightEdge + GameManager.SpaceBetweenObstaclesGroup + (transform.parent.GetChild(index).GetComponent<BoxCollider2D>().size.x / 2f) - transform.parent.GetChild(index).GetComponent<BoxCollider2D>().offset.x;


                transform.localPosition = new Vector2( newXPos, 0);

                RefreshObstacles();
            }
            else
            {
                float previousRightEdge = transform.parent.GetChild(transform.parent.childCount - 1).localPosition.x + transform.parent.GetChild(transform.parent.childCount - 1).GetComponent<BoxCollider2D>().offset.x + (transform.parent.GetChild(transform.parent.childCount - 1).GetComponent<BoxCollider2D>().size.x /2f) ;

                float newXPos = previousRightEdge + GameManager.SpaceBetweenObstaclesGroup + (transform.parent.GetChild(index).GetComponent<BoxCollider2D>().size.x / 2f) - transform.parent.GetChild(index).GetComponent<BoxCollider2D>().offset.x;


                transform.localPosition = new Vector2( newXPos, 0);
                
                RefreshObstacles();
            }
        }
    }

    private void RefreshObstacles()
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
