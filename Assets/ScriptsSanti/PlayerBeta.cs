using System.Collections;
using UnityEngine;
public class PlayerBeta : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] int Speed = 1;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float PowerUpDuration;
    [SerializeField] bool isInvincible = false;

    [Header("Variables")]
    [SerializeField] Transform playerPositions;
    [SerializeField] Transform playerVisuals;
    [SerializeField] GameObject velocityParticles;
    [SerializeField] GameObject PowerupVisual;
    CircleCollider2D _hitbox;
    Vector2 startPos;
    float _currentRotation;
    float _offsetX;
    float _offsetY;
    private float _animSpeed = 1f;
    bool _particlesActive;
    int laneToBe = 1;
    int hp = 2;
    bool onWheelie;
    bool enableWheelie;
    ItemsManager itemsManager;
    GameManagerBeta gameManager;
    UIManager uIManager;
    Animator animator;
    Coroutine DisablePowerUpCoroutine;
    
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        uIManager = FindFirstObjectByType<UIManager>();
        animator = GetComponent<Animator>();
        _hitbox = GetComponent<CircleCollider2D>();
        itemsManager = FindFirstObjectByType<ItemsManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        gameManager.GetOnRestartEvent?.AddListener(OnRestart);
        gameManager.GetOnstartEvent?.AddListener(OnRestart);
        gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);
        _offsetX = _hitbox.offset.x;
        _offsetY = _hitbox.offset.y;
        InvokeRepeating("AccelerateAnimation", 0f, 1f);
    }
    void Update()
    {
        if (gameManager.GetGameStarted)
        {
            Movement();
            CheckForWheelie();
        }
    }

    void Movement()
    {
        bool up;
        bool down;
        if (!onWheelie)
        {
            up = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            down = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        }
        else
        {
            up = false;
            down = false;
        }

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
        bool left;

        if (enableWheelie) left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        else left = false;

        if (left)
        {
            _currentRotation += rotationSpeed * Time.deltaTime;
            if (!_particlesActive)
            {
                _particlesActive = true;
                velocityParticles.SetActive(true);
                _hitbox.offset = new Vector2(_offsetX -= 2f, _offsetY);
            }
            gameManager.IncreseSpeedOnWheelie(true);
            onWheelie = true;

        }
        else
        {
            gameManager.IncreseSpeedOnWheelie(false);
            _currentRotation -= rotationSpeed * 2 * Time.deltaTime;
            if (_currentRotation < 10) onWheelie = false;
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
        AudioManager.Instance.PlayClip(AudioManager.AudioList.GetHitSound, false, 0.4f);
        hp -= 1;
        uIManager.HpFeedback(hp);
        if (hp <= 0)
        {
            gameManager.GameOver();
            return;
        }
        animator.SetTrigger("LooseHp");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        SpriteRenderer CollisionItemSpriteRenderer = other.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Vector3 CollisionPosition = other.transform.position;
        
        bool isObstacle = false;

        switch (tag)
        {
            case "Bread":
                uIManager.PickupIngredient(CollisionPosition, 0, 50, CollisionItemSpriteRenderer.sprite);
                break;
            case "Chorizo":
                uIManager.PickupIngredient(CollisionPosition, 1, 50, CollisionItemSpriteRenderer.sprite);
                break;
            case "Lettuce":
                uIManager.PickupIngredient(CollisionPosition, 2, 50, CollisionItemSpriteRenderer.sprite);
                break;
            case "Tomato":
                uIManager.PickupIngredient(CollisionPosition, 3, 50, CollisionItemSpriteRenderer.sprite);
                break;
            case "VeganChori":
                uIManager.AddToIndexList(5);
                uIManager.PickupIngredient(CollisionPosition, 5, -200, CollisionItemSpriteRenderer.sprite);
                animator.SetTrigger("EatVeganChori");
                AudioManager.Instance.PlayClip(AudioManager.AudioList.PuajSound);
                break; 
            case "PowerUp1":
                if (DisablePowerUpCoroutine != null)
                {
                    StopCoroutine("ActivatePowerupWarning");
                    StopCoroutine(DisablePowerUpCoroutine);
                }
                else
                {
                    isInvincible = true;

                    AudioManager.Instance.PlayClip(AudioManager.AudioList.PowerUpSound, false, 1f, false);
                    AudioManager.Instance.motorbikeSound.pitch += 0.5f;
                    animator.SetFloat("IncreaseSpeed", _animSpeed + 0.5f);
                }

                PowerupVisual.SetActive(true);

                itemsManager.ChangeItemsSpeed(5, PowerUpDuration);

                DisablePowerUpCoroutine = StartCoroutine(DisablePowerUpAfterTime());
                break;
        }
        if (!isInvincible)
        {
            switch (tag)
            {
                case "Oil":
                    LooseHp();
                    itemsManager.ChangeItemsSpeed(-3, 2);
                    isObstacle = true;
                    break;
                case "Pothole":
                    LooseHp();
                    itemsManager.ChangeItemsSpeed(-2, 2);
                    isObstacle = true;
                    break;
                case "Cone":
                    LooseHp();
                    itemsManager.ChangeItemsSpeed(-2, 2);
                    isObstacle = true;
                    break;
            }
        }
        else if (isInvincible)
        {
            switch (tag)
            {
                case "Cone":
                    StartCoroutine(ChangeSortingLayerForTime(CollisionItemSpriteRenderer, 0.5f, "UI", CollisionItemSpriteRenderer.sortingLayerName));
                    other.GetComponent<Animator>().SetTrigger("Throw");
                    AudioManager.Instance.PlayClip(AudioManager.AudioList.DestroyObstacleSound, true);
                    isObstacle = true;
                    break;
                case "Oil":
                    isObstacle = true;
                    break;
                case "Pothole":
                    isObstacle = true;
                    break;
            }
        }
        other.GetComponent<ItemBehavior>().StopPowerUpAnimation();
        if (!isObstacle)
        {
            other.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
    }
    IEnumerator ChangeSortingLayerForTime(SpriteRenderer Renderer, float DelayedTime, string Layer, string DefaultLayer)
    {
        float timer = 0;
        Renderer.sortingLayerName = Layer;
        while (timer < DelayedTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Renderer.sortingLayerName = DefaultLayer;
    }
    IEnumerator DisablePowerUpAfterTime()
    {
        StartCoroutine("ActivatePowerupWarning");
        yield return new WaitForSeconds(PowerUpDuration);
        StopCoroutine("ActivatePowerupWarning");
        PowerupVisual.SetActive(false);
        isInvincible = false;
        DisablePowerUpCoroutine = null;
        AudioManager.Instance.motorbikeSound.pitch -= 0.5f;
        AudioManager.Instance.StopClip();
        animator.SetFloat("IncreaseSpeed", _animSpeed - 0.5f);
    }
    IEnumerator ActivatePowerupWarning()
    {
        yield return new WaitForSeconds(PowerUpDuration - 2);
        while (true)
        {
            PowerupVisual.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            PowerupVisual.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }
    void OnRestart()
    {
        ResetPosition();
        StopAllCoroutines();
        DisablePowerUpCoroutine = null;
        AudioManager.Instance.motorbikeSound.pitch -= 0.5f;
        AudioManager.Instance.StopClip();
        PowerupVisual.SetActive(false);
        isInvincible = false;
        laneToBe = 1;
        hp = 2;
        enableWheelie = false;
        InvokeActiveWheelie();
        animator.Rebind();
    }
    void OnQuit()
    {
        _hitbox.enabled = false;
        DisablePowerUpCoroutine = null;
        AudioManager.Instance.motorbikeSound.pitch -= 0.5f;
        AudioManager.Instance.StopClip();
    }
    public void ResetPosition()
    {
        transform.position = startPos;
    }
    public void InvokeActiveWheelie()
    {
        Invoke("ActivateWheelie", .5f);
    }
    void ActivateWheelie()
    {
        enableWheelie = true;
    }

    void AccelerateAnimation()
    {
        if (_animSpeed > 4) return;
        
        _animSpeed += 0.01f;
        animator.SetFloat("IncreaseSpeed", _animSpeed);
    }

    public void ResetAnimationSpeed()
    {
        _animSpeed = 1f;
        animator.SetFloat("IncreaseSpeed", _animSpeed);
    }
}
