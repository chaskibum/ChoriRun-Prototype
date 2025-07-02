using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclesManager : MonoBehaviour
{
    public AudioSource hitSound;
    public AudioSource pickupSound;
    public float moveSpeed = 10.0f;
    public float maxSpeed = 30.0f;

    public Player player;
    public ObstacleType type;
    public ScoreManager scoreManager;
    private GameManager _gameManager;

    public enum ObstacleType
    {
        Obstacle,
        Ingredient,
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == ObstacleType.Obstacle)
            {
                player.ChangeHp();
                hitSound.Play();
            }
            else if (type == ObstacleType.Ingredient)
            {
                scoreManager.AddScore(50);
                pickupSound.Play();
                InstantiateObstacle();
            }
        }
        else if (other.CompareTag("Limit"))
        {
            InstantiateObstacle();
        }
    }

    // Creates the obstacle in a random Y position off-screen. 
    void InstantiateObstacle()
    {
        List<float> yPositions = new List<float> { -3.0f, -5.2f, -7.7f };
        System.Random rnd = new System.Random();
        float randomPosition = yPositions[rnd.Next(yPositions.Count)];
        
        var x = Random.Range(20.0f, 60.0f);
        var y = randomPosition;
        transform.position = new Vector3(x, y);
    }
    
    private void Move()
    {
        transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
    }

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        InstantiateObstacle();
        InvokeRepeating(nameof(IncreaseSpeed), 1.0f, 1.0f);
    }

    private void Update()
    {
        Move();
    }

    public void IncreaseSpeed()
    {
        if (moveSpeed < maxSpeed)
        {
            moveSpeed += 0.1f;
        }
    }

    /*private void MakeChori(IngredientType type)
    {
        CleanInventory();

        var potions = _playerInventory.GetPotions();

        int index = 1;
        foreach (var potion in potions)
        {
            GameObject go = Instantiate(inventorySlot, Vector3.zero, Quaternion.identity, transform);
            if (go)
            {
                if (go.TryGetComponent(out InventorySlot slot))
                {
                    slot.Setup(potion);
                    slot.keyNumber = index;
                }
            }
            index++;
        }
    }*/
    
    private void CleanInventory()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }
}
