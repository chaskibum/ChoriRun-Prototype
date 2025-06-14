using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverText;

    public void EndGame()
    {
        Time.timeScale = 0;
        gameOverText.SetActive(true);
    }
}
