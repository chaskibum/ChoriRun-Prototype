using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        _score = 0;
        InvokeRepeating("AddTimeScore", 3, 0.1f);
    }


    private void AddTimeScore()
    {
        _score++;
        scoreText.text = _score.ToString();
    }
}
