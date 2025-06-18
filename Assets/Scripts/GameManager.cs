using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // public int SpaceBetweenObstaclesGroup = 5;
    public float Speed;
    [SerializeField] int MaxSpeed;
    [SerializeField] float SpeedIncreseAmount = 1;
    [SerializeField] int TimeTillIncrese = 1;
    // [SerializeField] Transform ObstaclesContainer;
    public GameObject gameOverText;
    public GameObject restartButton;
    
    void Start()
    {
        // SortObstacles();
        gameOverText.SetActive(false);
        restartButton.SetActive(false);
        StartCoroutine(GraduallyIncreaseSpeed());
    }

    /*void SortObstacles()
    {
        for (int i = 0; i < ObstaclesContainer.childCount; i++)
        {
            if (i - 1 >= 0)
            {
                ObstaclesContainer.GetChild(i).transform.position = new Vector2(ObstaclesContainer.GetChild(i - 1).transform.position.x + SpaceBetweenObstaclesGroup + ObstaclesContainer.GetChild(i).GetComponent<BoxCollider2D>().bounds.size.x, ObstaclesContainer.GetChild(i).transform.position.y);
            }
        }
    }*/
    
    IEnumerator GraduallyIncreaseSpeed()
    {
        while (Speed != MaxSpeed)
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
    
    public void RestartGame()
    {
        SceneManager.LoadScene("LevelFinal");
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
    }
}
