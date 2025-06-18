using UnityEngine;

public class ObstacleGroupBehavior : MonoBehaviour
{
    GameManager _gameManager;
    void Awake() 
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }
    
    void Update()
    {
        transform.position += Vector3.left * (_gameManager.speed * Time.deltaTime);
    }

    /*private void OnTriggerEnter2D(Collider2D other) 
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
    }*/
}
