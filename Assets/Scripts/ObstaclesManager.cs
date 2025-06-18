using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclesManager : MonoBehaviour
{
    // private int _objectsOnScreen = 0;
    // public float spawnLocation = 24;

    public float moveSpeed = 3.0f;
    // public float randomSpawnMin = 8.0f;
    // public float randomSpawnMax = 12.0f;

    public Player player;
    public ObstacleType type;
    public ScoreManager scoreManager;

    public enum ObstacleType
    {
        Obstacle,
        Ingredient,
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == ObstacleType.Obstacle)
                player.ChangeHp();
            else if (type == ObstacleType.Ingredient)
            {
                scoreManager.AddScore(50);
                InstantiateObstacle();
            }
        }
        else if (other.CompareTag("Limit"))
        {
            InstantiateObstacle();
        }
    }

    // Creates the obstacle in a random Y position off-screen. 
    void InstantiateObstacle()
    {
        List<float> yPositions = new List<float> { -3.0f, -5.2f, -7.7f };
        System.Random rnd = new System.Random();
        float randomPosition = yPositions[rnd.Next(yPositions.Count)];
        
        var x = Random.Range(20.0f, 60.0f);
        var y = randomPosition;
        transform.position = new Vector3(x, y);
    }

    /*// Instantiates the obstacle with a randomized spawn timer.
    IEnumerator RepeatObstacle()
    {
        while (true)
        {
            InstantiateObstacle();
            float randomWait = Random.Range(randomSpawnMin, randomSpawnMax);
            yield return new WaitForSeconds(randomWait);
        }
    }*/
    
    private void Move()
    {
        transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
    }
    
    private void Start()
    {
        // StartCoroutine(RepeatObstacle());
        InstantiateObstacle();
    }

    private void Update()
    {
        Move();
    }
}
