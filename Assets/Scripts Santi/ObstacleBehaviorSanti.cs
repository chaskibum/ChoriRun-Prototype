using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviorSanti : MonoBehaviour
{
    GameManagerSanti gameManager;
    ObstacleGroupBehaviorSanti obstacleGroupBehavior;
    [SerializeField] CollisionType collisionType;
    [SerializeField] ObstacleType obstacleType;
    [SerializeField] IngredientType ingredientType;
    [SerializeField] List<Sprite> ObstaclesSprites;
    [SerializeField] List<Sprite> IngredientsSprites;
    bool RefreshSubType = false;
    bool RefreshColissionType = false;
    int RandomInt = 0;
    float randomValue(float total)
    {
        float Value = Random.Range(0f, total);
        return Value;
    } 

    enum CollisionType
    {
        Obstacle,
        Ingredient,
        PowerUp,
        Random
    }
    enum ObstacleType
    {
        Oil,
        Pothole,
        Cone,
        Random
    }
    enum IngredientType
    {
        Chorizo,
        Lettuce,
        Bread,
        Tomato,
        Random
    }
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerSanti>();
        obstacleGroupBehavior = GetComponentInParent<ObstacleGroupBehaviorSanti>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (collisionType == CollisionType.Random)
        {
            RefreshColissionType = true;
        }
        else if (obstacleType == ObstacleType.Random || ingredientType == IngredientType.Random)
        {
            RefreshSubType = true;
        }
        PickObstacleType();
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RandomObstacleType()
    {
        if (randomValue(gameManager.obstacleProbability + gameManager.ingredientProbability) <= gameManager.obstacleProbability)
        {
            if (obstacleGroupBehavior.ObstacleAmount < obstacleGroupBehavior.MaxObstaclesPerGroup)
            {
                collisionType = CollisionType.Obstacle;
            }
            else
            {
                float TotalProb = gameManager.ingredientProbability + gameManager.powerUpProbability;

                float CollectableProb = gameManager.ingredientProbability / TotalProb * 100;

                float PowerUpProb = gameManager.powerUpProbability / TotalProb * 100;

                if (randomValue(CollectableProb + PowerUpProb) <= CollectableProb)
                {
                    collisionType = CollisionType.Ingredient;
                }
                else
                {
                    collisionType = CollisionType.Ingredient; //Cambiar a powerup cuando hayan
                }
            }

        }
        else if (randomValue(gameManager.obstacleProbability + gameManager.ingredientProbability) <= gameManager.obstacleProbability + gameManager.ingredientProbability)
        {
            if (obstacleGroupBehavior.IngredientsAmount < obstacleGroupBehavior.MaxIngredientsPerGroup)
            {
                collisionType = CollisionType.Ingredient;
            }
            else
            {
                float TotalProb = gameManager.obstacleProbability + gameManager.powerUpProbability;

                float FirstProb = gameManager.obstacleProbability / TotalProb * 100;

                float SecondProb = gameManager.powerUpProbability / TotalProb * 100;

                if (randomValue(gameManager.obstacleProbability + gameManager.ingredientProbability) <= FirstProb)
                {
                    collisionType = CollisionType.Obstacle;
                }
                else
                {
                    collisionType = CollisionType.Obstacle; //Cambiar a powerup cuando hayan
                }
            }

        }
        else
        {
            // collisionType = CollisionType.PowerUp;
        }

        switch (collisionType)
        {

            case CollisionType.Obstacle:

                RandomInt = Random.Range(0, ObstaclesSprites.Count);

                obstacleType = (ObstacleType)RandomInt;

                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ObstaclesSprites[(int)obstacleType];

                transform.tag = obstacleType.ToString();

                obstacleGroupBehavior.ObstacleAmount++;
                break;

            case CollisionType.Ingredient:

                RandomInt = Random.Range(0, IngredientsSprites.Count);

                ingredientType = (IngredientType)RandomInt;

                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = IngredientsSprites[(int)ingredientType];

                transform.tag = ingredientType.ToString();

                obstacleGroupBehavior.IngredientsAmount++;

                break;

            case CollisionType.PowerUp:
                Debug.Log("Soy un powerup, voy a elegir cual");
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;

        }
    }
    public void PickObstacleType()
    {
        if (collisionType == CollisionType.Random)
        {
            RandomObstacleType();
            return;
        }
        else if (collisionType == CollisionType.Obstacle)
        {
            if (obstacleType == ObstacleType.Random)
            {
                RandomInt = Random.Range(0, ObstaclesSprites.Count);
                obstacleType = (ObstacleType)RandomInt;
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ObstaclesSprites[(int)obstacleType];
            transform.tag = obstacleType.ToString();
        }
        else if (collisionType == CollisionType.Ingredient)
        {
            if (ingredientType == IngredientType.Random)
            {
                RandomInt = Random.Range(0, IngredientsSprites.Count);
                ingredientType = (IngredientType)RandomInt;
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = IngredientsSprites[(int)ingredientType];
            transform.tag = ingredientType.ToString();
        }
    }
    public void SetRandomCollisionType()
    {
        if (RefreshColissionType)
        {
            collisionType = CollisionType.Random;
        }
        else if(RefreshSubType)
        {
            obstacleType = ObstacleType.Random;
            ingredientType = IngredientType.Random;
        }
    }
}
