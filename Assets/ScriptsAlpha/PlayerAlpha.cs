using UnityEngine;

namespace ScriptsAlpha
{
    public class PlayerAlpha : MonoBehaviour
    {
        public Transform playerPositions;
        public Transform playerVisuals;
        public GameObject velocityParticles;

        public float movementSpeed = 10.0f;
        public float rotationSpeed = 10.0f;

        [Header("Audio")]
        public AudioSource hitSound;
        public AudioSource crashSound;

        private int _currentPosition;
        private float _currentRotation;
		private CircleCollider2D _hitbox;
		private float _offsetX;
		private float _offsetY;

        private bool _particlesActive;
        private float _increaseSpeedCooldown;

        public int hp = 2;

        private GameManagerAlpha _gameManager;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
			_hitbox = GetComponent<CircleCollider2D>();
        }

        private void Start()
        {
            velocityParticles.SetActive(false);
			_offsetX = _hitbox.offset.x;
			_offsetY = _hitbox.offset.y;
        }

        private void Update()
        {
            CheckVerticalMovement();
            CheckForWheelie();
        }

        public void LooseHp()
        {
            hitSound.Play();
            hp -= 1;
            _gameManager.HpFeedback(hp);
            if (hp <= 0)
            {
                crashSound.Play();
                _gameManager.GameOver();
                return;
            }
            _animator.SetTrigger("LooseHp");
        }

        private void CheckVerticalMovement()
        {
            bool up = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
            bool down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

            if (up)
                _currentPosition += 1;
            else if (down)
                _currentPosition -= 1;

            _currentPosition = Mathf.Clamp(_currentPosition, 0, playerPositions.childCount - 1);

            Transform positionTransform = playerPositions.GetChild(_currentPosition);
            transform.position = Vector3.Lerp(transform.position, positionTransform.position, movementSpeed * Time.deltaTime);
        }

        private void CheckForWheelie()
        {
            bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

            if (left)
            {
                _currentRotation += rotationSpeed * Time.deltaTime;
                if (!_particlesActive)
                {
                    _particlesActive = true;
                    velocityParticles.SetActive(true);
					_hitbox.offset = new Vector2(_offsetX -= 2f, _offsetY);
                }

                if (_increaseSpeedCooldown <= 0f)
                {
                    _gameManager.IncreaseSpeed();
                    _increaseSpeedCooldown = 0.2f;
                }
                _increaseSpeedCooldown -= Time.deltaTime;
            }
            else
            {
                _currentRotation -= (rotationSpeed * 2) * Time.deltaTime;
                if (_currentRotation < 0.1f && _particlesActive)
                {
                    _particlesActive = false;
                    velocityParticles.SetActive(false);
					_hitbox.offset = new Vector2(_offsetX += 2f, _offsetY);
                }
            }

            _currentRotation = Mathf.Clamp(_currentRotation, 0, 25.0f);
            playerVisuals.eulerAngles = new Vector3(0.0f, 0.0f, _currentRotation);
        }

        public void ResetPosition()
        {
            _currentPosition = 1;
        }
    }
}
