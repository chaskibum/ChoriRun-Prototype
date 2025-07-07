using ScriptsAlpha;
using UnityEngine;

public class PowerupAlpha : MonoBehaviour
{
    public GameObject placeholder;

    private GameManagerAlpha _gameManager;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManagerAlpha>();
    }

    private void Start()
    {
        // placeholder.SetActive(false);
        // transform.GetChild(Random.Range(1, 4)).gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        placeholder.SetActive(false);
        _gameManager.ActivatePowerUp();
    }
}
