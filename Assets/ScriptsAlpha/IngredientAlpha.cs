using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptsAlpha
{
    public class IngredientAlpha : MonoBehaviour
    {
        public GameObject placeholder;

        private GameManagerAlpha _gameManager;
        private int _child;

        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
        }

        private enum Type
        {
            Bread,
            Chori,
            Lettuce,
            Tomato,
        }

        private void Start()
        {
            placeholder.SetActive(false);
            _child = Random.Range(1, 5);
            transform.GetChild(_child).gameObject.SetActive(true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _gameManager.AddScore(50);
            _gameManager.ChoriFeedback(_child - 1);
            Destroy(gameObject);
        }
    }
}
