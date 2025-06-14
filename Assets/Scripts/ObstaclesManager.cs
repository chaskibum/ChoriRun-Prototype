using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclesManager : MonoBehaviour
{
    private int _objectsOnScreen = 0;

    public float moveSpeed = 3.0f;
    public float randomSpawnMin = 8.0f;
    public float randomSpawnMax = 12.0f;

    public Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.ChangeHp();
    }

    // Creates the obstacle in a random Y position off-screen. 
    void InstantiateObstacle()
    {
        var x = 12.0f;
        var y = Random.Range(-1.0f, -4.0f);
        transform.position = new Vector3(x, y);
    }

    // Instantiates the obstacle with a randomized spawn timer.
    IEnumerator RepeatObstacle()
    {
        while (true)
        {
            InstantiateObstacle();
            float randomWait = Random.Range(randomSpawnMin, randomSpawnMax);
            yield return new WaitForSeconds(randomWait);
        }
    }
    
    private void Move()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }
    
    private void Start()
    {
        StartCoroutine(RepeatObstacle());
    }

    private void Update()
    {
        Move();
    }
}
