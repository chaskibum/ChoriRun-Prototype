using UnityEngine;

public class ObstacleGroupBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameManager GameManager;
    void Awake() 
    {
        GameManager = FindFirstObjectByType<GameManager>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * GameManager.Speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Limit"))
        {
            int index = transform.GetSiblingIndex();
            if (index - 1 >= 0)
            {
                transform.position = new Vector2(transform.parent.GetChild(index - 1).transform.position.x + GameManager.SpaceBetweenObstaclesGroup + transform.parent.GetChild(index - 1).GetComponent<BoxCollider2D>().bounds.size.x, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.parent.GetChild(transform.parent.childCount - 1).transform.position.x + GameManager.SpaceBetweenObstaclesGroup + transform.parent.GetChild(transform.parent.childCount - 1).GetComponent<BoxCollider2D>().bounds.size.x, transform.position.y);
            }
        }
    }
}
