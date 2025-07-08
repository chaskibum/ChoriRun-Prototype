using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptsAlpha
{
    public class ObstacleAlpha : MonoBehaviour
    {
        public GameObject placeholder;

        private GameManagerAlpha _gameManager;
        private Animator _objectAnimator;

        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
            _objectAnimator = GetComponent<Animator>();
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
            if (!_gameManager.GetisPlayerInvincible)
            {
                _gameManager.GetPlayer.LooseHp();
                _gameManager.gameSpeed -= 3f;
            }
            else
            {
                _objectAnimator.Play("ThrowObjectAway",0,0);
            }

        }
    }
}
