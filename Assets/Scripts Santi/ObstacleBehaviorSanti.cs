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
    bool refreshSubType = false;
    bool refreshColissionType = false;
    int randomInt = 0;
    float randomValue(float MaxValue)
    {
        float Value = Random.Range(0f, MaxValue);
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
            refreshColissionType = true;
        }
        else if (obstacleType == ObstacleType.Random || ingredientType == IngredientType.Random)
        {
            refreshSubType = true;
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

                float IngredientProb = gameManager.ingredientProbability / TotalProb * 100;

                float PowerUpProb = gameManager.powerUpProbability / TotalProb * 100;

                if (randomValue(IngredientProb + PowerUpProb) <= IngredientProb)
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

                if (randomValue(FirstProb + SecondProb) <= FirstProb)
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

                randomInt = Random.Range(0, ObstaclesSprites.Count);

                obstacleType = (ObstacleType)randomInt;

                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ObstaclesSprites[randomInt];

                transform.tag = obstacleType.ToString();

                obstacleGroupBehavior.ObstacleAmount++;
                break;

            case CollisionType.Ingredient:
                
                randomInt = Random.Range(0, IngredientsSprites.Count);

                ingredientType = (IngredientType)randomInt;

                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = IngredientsSprites[randomInt];

                transform.tag = ingredientType.ToString();

                obstacleGroupBehavior.IngredientsAmount++;
                break;

            case CollisionType.PowerUp:
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
                randomInt = Random.Range(0, ObstaclesSprites.Count);
                obstacleType = (ObstacleType)randomInt;
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ObstaclesSprites[(int)obstacleType];
            transform.tag = obstacleType.ToString();
        }
        else if (collisionType == CollisionType.Ingredient)
        {
            if (ingredientType == IngredientType.Random)
            {
                randomInt = Random.Range(0, IngredientsSprites.Count);
                ingredientType = (IngredientType)randomInt;
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = IngredientsSprites[(int)ingredientType]; 
            transform.tag = ingredientType.ToString();
        }
    }
    public void SetRandomCollisionType()
    {
        if (refreshColissionType)
        {
            collisionType = CollisionType.Random;
        }
        else if(refreshSubType)
        {
            obstacleType = ObstacleType.Random;
            ingredientType = IngredientType.Random;
        }
    }
}
