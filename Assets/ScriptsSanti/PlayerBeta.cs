using System.Collections;
using UnityEngine;
public class PlayerBeta : MonoBehaviour
{
    [SerializeField] Transform playerPositions;
    [SerializeField] Transform playerVisuals;
    [SerializeField] GameObject velocityParticles;
    [SerializeField] int Speed = 1;
    [SerializeField] float rotationSpeed = 10.0f;
    float _currentRotation;
    CircleCollider2D _hitbox;
    float _offsetX;
    float _offsetY;
    bool _particlesActive;
    [SerializeField] float PowerUpDuration;
    Vector2 startPos;
    int laneToBe = 1;
    public int hp = 2;
    ItemsManager itemsManager;
    GameManagerBeta gameManager;
    Animator _animator;

    [Header("Audio")]
    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioSource crashSound;
    bool isInvincible;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        gameManager.GetOnRestartEvent.AddListener(OnRestart);
    }
    void Update()
    {
        Movement();
        CheckForWheelie();
    }

    void Movement()
    {
        bool up = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        if (up && !gameManager.isGamePaused)
        {
            laneToBe++;
        }
        else if (down && !gameManager.isGamePaused)
        {
            laneToBe--;
        }
        laneToBe = Mathf.Clamp(laneToBe, 0, playerPositions.childCount - 1);
        Transform positionTransform = playerPositions.GetChild(laneToBe);
        transform.position = Vector3.Lerp(transform.position, positionTransform.position, Speed * Time.deltaTime);
    }
    void CheckForWheelie()
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
            //Aumentar velocidad
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

    public void LooseHp()
    {
        hitSound.Play();
        hp -= 1;
        gameManager.HpFeedback(hp);
        if (hp <= 0)
        {
            crashSound.Play();
            gameManager.GameOver();
            return;
        }
        _animator.SetTrigger("LooseHp");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        bool animationStarted = false;
        switch (tag)
        {
            case "Chorizo":
                gameManager.ChoriFeedback(0);
                gameManager.AddScore(50);
                break;
            case "Lettuce":
                gameManager.ChoriFeedback(0);
                gameManager.AddScore(50);
                break;
            case "Tomato":
                gameManager.ChoriFeedback(0);
                gameManager.AddScore(50);
                break;
            case "Bread":
                gameManager.ChoriFeedback(0);
                gameManager.AddScore(50);
                break;
            case "VeganChori":
                gameManager.AddScore(-200);
                break;
        }
        if (!isInvincible)
        {
            switch (tag)
            {
                case "Oil": 
                    LooseHp();
                    itemsManager.ChangeItemsSpeed(10);
                    break;
                case "Pothole":
                    LooseHp();
                    itemsManager.ChangeItemsSpeed(5);
                    break;
                case "Cone":
                    LooseHp();
                    itemsManager.ChangeItemsSpeed(5);
                    break;
            }
        }
        else if (isInvincible)
        {
            switch (tag)
            {
                case "Cone":
                    other.GetComponent<Animator>().SetTrigger("Throw");
                    animationStarted = true;
                    break;
            }
        }

        if (!animationStarted)
        {
            other.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
    }
    IEnumerator DisablePowerUpAfterTime()
    {
        yield return new WaitForSeconds(PowerUpDuration);
        isInvincible = false;
    }
    void OnRestart()
    {
        transform.position = startPos;
        laneToBe = 1;
    }
}
