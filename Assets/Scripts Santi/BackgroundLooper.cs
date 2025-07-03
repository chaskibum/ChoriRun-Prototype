using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    GameManagerSanti gameManager;
    [SerializeField] bool SpeedFromObstacles;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerSanti>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SpeedFromObstacles)
        {
            transform.position += Vector3.left * gameManager.ObstaclesSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.left * gameManager.BackgroundSpeed * Time.deltaTime;
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Limit"))
        {
            int index = transform.GetSiblingIndex();
            float GetPreviousRightEdge(int currentIndex)
            {
                Transform ChildTransform = transform.parent.GetChild(currentIndex - 1);
            
                return ChildTransform.localPosition.x + ChildTransform.GetComponent<BoxCollider2D>().offset.x + (ChildTransform.GetComponent<BoxCollider2D>().size.x / 2f);
            }
            Vector2 CalculateNewPosition(float PreviousRightEdge)
            {
                float newXPos = PreviousRightEdge + (transform.parent.GetChild(index).GetComponent<BoxCollider2D>().size.x / 2f) - transform.parent.GetChild(index).GetComponent<BoxCollider2D>().offset.x;
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
    }
}
