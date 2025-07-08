using UnityEngine;

namespace ScriptsAlpha
{
    public class BadIngredientAlpha : MonoBehaviour
    {
        private GameManagerAlpha _gameManager;
        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
        }


        private void Start()
        {
            // placeholder.SetActive(false);
            // _child = Random.Range(1, 5);
            // transform.GetChild(_child).gameObject.SetActive(true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _gameManager.AddScore(-200);
            Destroy(gameObject);
        }
    }
}
