using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform livesContainer;
    public Transform ingredientsContainer;
    public GameObject gameOverText;
    public GameObject restartButton;
    public AudioSource music;

    public Transform background;
    
    private float _moveSpeed = 10.0f;
    private const float MaxSpeed = 30.0f;
    
    void Start()
    {
        music.volume = 1;
        gameOverText.SetActive(false);
        restartButton.SetActive(false);
    }

    /*private void Update()
    {
        Move();
    }
    
    private void Move()
    {
        transform.Translate(Vector3.left * (_moveSpeed * Time.deltaTime));
    }*/

    public void RestartGame()
    {
        SceneManager.LoadScene("LevelFinal");
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        music.volume = 0.2f;
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
    }

    public void HpFeedback(int hp)
    {
        livesContainer.GetChild(hp).gameObject.SetActive(false);
    }
    
    
    
    /*public void IncreaseSpeed()
    {
        if (_moveSpeed < MaxSpeed)
        {
            _moveSpeed += 0.1f;
        }
    }*/
}
