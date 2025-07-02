using System.Collections;
using UnityEngine;

public class GameManagerSanti : MonoBehaviour
{
    public int SpaceBetweenObstaclesGroup = 5;
    public float Speed;
    [SerializeField] int MaxSpeed;
    [SerializeField] float SpeedIncreseAmount = 1;
    [SerializeField] int TimeTillIncrese = 1;
    [SerializeField] Transform ObstaclesContainer;
    public float obstacleProbability = 0;
    public float ingredientProbability = 0;
    public float powerUpProbability = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SortObstacles();
        StartCoroutine(GraduallyIncreaseSpeed());
    }
    
    void SortObstacles()
    {
        float currentX = 0f;
        for (int i = 0; i < ObstaclesContainer.childCount; i++)
        {
            float obstacleCenter = currentX + (ObstaclesContainer.GetChild(i).GetComponent<BoxCollider2D>().size.x / 2f) - ObstaclesContainer.GetChild(i).GetComponent<BoxCollider2D>().offset.x;
            ObstaclesContainer.GetChild(i).localPosition = new Vector2(obstacleCenter, 0);
            currentX += ObstaclesContainer.GetChild(i).GetComponent<BoxCollider2D>().size.x + SpaceBetweenObstaclesGroup;
        }
    }
    IEnumerator GraduallyIncreaseSpeed()
    {
        while (Speed < MaxSpeed)
        {
            float ActualSpeed = Speed;
            while (!Mathf.Approximately(Speed, ActualSpeed + SpeedIncreseAmount))
            {
                Speed = Mathf.MoveTowards(Speed, ActualSpeed + SpeedIncreseAmount, Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(TimeTillIncrese);
        }   
    }
}
