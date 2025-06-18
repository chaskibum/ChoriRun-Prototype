using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // public int SpaceBetweenObstaclesGroup = 5;
    public float speed;
    [SerializeField] int maxSpeed;
    [SerializeField] float speedIncreaseAmount = 1;
    [SerializeField] int timeTillIncrease = 1;
    // [SerializeField] Transform ObstaclesContainer;
    public Transform livesContainer;
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
        while (!Mathf.Approximately(speed, maxSpeed))
        {
            float actualSpeed = speed;
            while (!Mathf.Approximately(speed, actualSpeed + speedIncreaseAmount))
            {
                speed = Mathf.MoveTowards(speed, actualSpeed + speedIncreaseAmount, Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(timeTillIncrease);
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

    public void HpFeedback(int hp)
    {
        livesContainer.GetChild(hp).gameObject.SetActive(false);
    }
}
