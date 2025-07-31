using UnityEngine;

namespace ScriptsAlpha
{
    public class PowerupAlpha : MonoBehaviour
    {
        public GameObject placeholder;

        private GameManagerAlpha _gameManager;

        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            placeholder.SetActive(false);
            // _gameManager.ActivatePowerUp();
        }
    }
}
