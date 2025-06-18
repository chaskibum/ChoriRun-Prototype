using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        _score = 0;
        InvokeRepeating(nameof(AddTimeScore), 0, 0.1f);
    }

    public void AddScore(int points = 1)
    {
        _score += points;
        scoreText.text = _score.ToString();
    }

    private void AddTimeScore()
    {
        _score++;
        scoreText.text = _score.ToString();
    }
}
