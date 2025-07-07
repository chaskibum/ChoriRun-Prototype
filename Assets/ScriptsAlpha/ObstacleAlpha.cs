using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptsAlpha
{
    public class ObstacleAlpha : MonoBehaviour
    {
        public GameObject placeholder;

        private GameManagerAlpha _gameManager;
    
        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
        }

        private enum Type
        {
            Oil,
            Cone,
            Hole
        }

        private void Start()
        {
            placeholder.SetActive(false);
            transform.GetChild(Random.Range(1, 4)).gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _gameManager.GetPlayer.LooseHp();
            _gameManager.gameSpeed -= 3f;
        }
    }
}
