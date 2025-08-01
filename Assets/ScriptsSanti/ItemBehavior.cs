using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    GameManagerBeta gameManager;
    ItemGroupBehavior itemGroupBehavior;
    ItemsManager itemsManager;
    [SerializeField, Space(5)] ItemType itemType;
    [SerializeField, Space(5)] ObstacleType obstacleType;
    [SerializeField, Space(5)] IngredientType ingredientType;
    [SerializeField, Space(5)] BadIngredientType badIngredientType;
    [SerializeField, Space(5)] PowerUpType powerupType;

    bool refreshSubType = false;
    bool refreshColissionType = false;
    float obstacleProb;
    float ingredientProb;
    float badIngredientProb;
    float powerupProb;
    float randomFloatValue(float MaxValue)
    {
        float Value = Random.Range(0f, MaxValue);
        return Value;
    }
    int randomIntValue(int MaxValue)
    {
        int Value = Random.Range(0, MaxValue);
        return Value;
    }
    #region Enums
    enum ItemType
    {
        Obstacle,
        Ingredient,
        PowerUp,
        BadIngredient,
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
        Bread,
        Chorizo,
        Lettuce,
        Tomato,
        Random
    }
    enum BadIngredientType
    {
        VeganChori,
        Random
    }
    enum PowerUpType
    {
        PowerUp1,
        Random
    }
    #endregion
    Dictionary<ItemType, float> ProbabilityDic = new();
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        itemGroupBehavior = GetComponentInParent<ItemGroupBehavior>();
        itemsManager = FindFirstObjectByType<ItemsManager>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obstacleProb = GetAndAddToDic(ItemType.Obstacle, gameManager.GetObstacleProbability);
        ingredientProb = GetAndAddToDic(ItemType.Ingredient, gameManager.GetIngredientProbability);
        badIngredientProb = GetAndAddToDic(ItemType.BadIngredient, gameManager.GetBadIngredientProbability);
        powerupProb = GetAndAddToDic(ItemType.PowerUp, gameManager.GetPowerupProbability);

        SetRandomizationBools();

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    void SetRandomizationBools()
    {
        if (itemType == ItemType.Random)
        {
            refreshColissionType = true;
        }
        refreshSubType = IsRandomSubType(itemType);

        bool IsRandomSubType(ItemType type)
        {
            return  (type == ItemType.Ingredient && ingredientType == IngredientType.Random)
                 || (type == ItemType.Obstacle && obstacleType == ObstacleType.Random)
                 || (type == ItemType.BadIngredient && badIngredientType == BadIngredientType.Random)
                 || (type == ItemType.PowerUp && powerupType == PowerUpType.Random);
        }
    }

    float GetAndAddToDic(ItemType ProbItemType, float Probability)
    {
        ProbabilityDic.Add(ProbItemType, Probability);
        return Probability;
    }
    void RandomObstacleType()
    {
        float Probability = randomFloatValue(obstacleProb + ingredientProb + powerupProb + badIngredientProb);

        if (Probability <= obstacleProb)
        {
            if (itemGroupBehavior.GetObstacleAmount < itemGroupBehavior.GetMaxObstaclesPerGroup)
            {
                itemType = ItemType.Obstacle;
            }
            else
            {
                ItemType TypeSelected = RecalculateNewProbs(ProbabilityDic, obstacleProb);

                itemType = TypeSelected;
            }
        }
        else if (Probability <= obstacleProb + ingredientProb)
        {
            if (itemGroupBehavior.GetIngredientsAmount < itemGroupBehavior.GetMaxIngredientPerGroup)
            {
                itemType = ItemType.Ingredient;
            }
            else
            {
                ItemType TypeSelected = RecalculateNewProbs(ProbabilityDic, ingredientProb);

                itemType = TypeSelected;
            }

        }
        else if (Probability <= obstacleProb + ingredientProb + badIngredientProb)
        {
            if (itemGroupBehavior.GetBadIngredientAmount < itemGroupBehavior.GetMaxBadIngredientPerGroup)
            {
                itemType = ItemType.BadIngredient;
            }
            else
            {
                ItemType TypeSelected = RecalculateNewProbs(ProbabilityDic, badIngredientProb);

                itemType = TypeSelected;
            }
        }
        else
        {
            if (itemGroupBehavior.GetPowerupAmount < itemGroupBehavior.GetMaxPowerUpPerGroup)
            {
                itemType = ItemType.PowerUp;
            }
            else
            {
                ItemType TypeSelected = RecalculateNewProbs(ProbabilityDic, powerupProb);

                itemType = TypeSelected;
            }
        }

        switch (itemType)
        {
            case ItemType.Obstacle:

                obstacleType = (ObstacleType)randomIntValue(itemsManager.GetObstaclesSprites.Count);

                SetTagAndSprite(obstacleType.ToString(), (int)obstacleType, itemsManager.GetObstaclesSprites);

                itemGroupBehavior.IncrementObstacleCount();
                Debug.Log("Obstacle") ;
                break;

            case ItemType.Ingredient:

                ingredientType = (IngredientType)randomIntValue(itemsManager.GetIngredientsSprites.Count);

                SetTagAndSprite(ingredientType.ToString(), (int)ingredientType, itemsManager.GetIngredientsSprites);

                itemGroupBehavior.IncrementIngredientCount();
                Debug.Log("Ingredient");
                break;

            case ItemType.PowerUp:

                powerupType = (PowerUpType)randomIntValue(itemsManager.GetPowerUpSprites.Count);

                SetTagAndSprite(powerupType.ToString(), (int)powerupType, itemsManager.GetPowerUpSprites);

                itemGroupBehavior.IncrementPowerUpCount();

                StartCoroutine("AnimatePowerup");
                Debug.Log("PowerUp");
                break;
            case ItemType.BadIngredient:

                badIngredientType = (BadIngredientType)randomIntValue(itemsManager.GetBadIngredientsSprites.Count);

                SetTagAndSprite(badIngredientType.ToString(), (int)badIngredientType, itemsManager.GetBadIngredientsSprites);

                itemGroupBehavior.IncrementBadIngredientCount();
                Debug.Log("Badingredient");
                break;
        }
    }

    ItemType RecalculateNewProbs(Dictionary<ItemType, float> ProbabilityList, float ProbabilityToExclude)
    {
        float TotalProb = 0;
        float MaxValue = 0;

        Dictionary<ItemType, float> NewProbsList = new();

        foreach (KeyValuePair<ItemType, float> Probability in ProbabilityList)
        {
            if (Probability.Value != ProbabilityToExclude && Probability.Key != ItemType.PowerUp)
            {
                TotalProb += Probability.Value;
            }
        }
        foreach (KeyValuePair<ItemType, float> Probability in ProbabilityList)
        {
            if (Probability.Value == ProbabilityToExclude)
            {
                NewProbsList.Add(Probability.Key, 0);
            }
            else if (Probability.Key != ItemType.PowerUp)
            {
                float NewProb = Probability.Value / TotalProb * (100 - powerupProb);
                NewProbsList.Add(Probability.Key, NewProb);
            }
            else
            {
                NewProbsList.Add(Probability.Key, Probability.Value);
            }
        }

        foreach (KeyValuePair<ItemType, float> NewProbs in NewProbsList)
        {
            MaxValue += NewProbs.Value;
        }

        float ProbabilityChoosed = 0;
        float RandomProb = randomFloatValue(MaxValue);
        foreach (KeyValuePair<ItemType, float> Probability in NewProbsList)
        {
            ProbabilityChoosed += Probability.Value;
            if (RandomProb <= ProbabilityChoosed && RandomProb > 0)
            {
                return Probability.Key;
            }
        }
        return ItemType.Random;
    }

    void SetTagAndSprite(string TagName, int index, List<Sprite> SpritesList)
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SpritesList[index];

        transform.tag = TagName.ToString();
    }

    public void PickObstacleType()
    {
        switch (itemType)
        {
            case ItemType.Random:

                RandomObstacleType();

                break;
            case ItemType.Obstacle:

                if (obstacleType == ObstacleType.Random)
                {
                    obstacleType = (ObstacleType)randomIntValue(itemsManager.GetObstaclesSprites.Count);
                }
                SetTagAndSprite(obstacleType.ToString(), (int)obstacleType, itemsManager.GetObstaclesSprites);

                break;
            case ItemType.Ingredient:

                if (ingredientType == IngredientType.Random)
                {
                    ingredientType = (IngredientType)randomIntValue(itemsManager.GetIngredientsSprites.Count);
                }
                SetTagAndSprite(ingredientType.ToString(), (int)ingredientType, itemsManager.GetIngredientsSprites);

                break;
            case ItemType.PowerUp:

                if (powerupType == PowerUpType.Random)
                {
                    powerupType = (PowerUpType)randomIntValue(itemsManager.GetPowerUpSprites.Count);
                }
                SetTagAndSprite(powerupType.ToString(), (int)powerupType, itemsManager.GetPowerUpSprites);

                StartCoroutine("AnimatePowerup");
                break;
            case ItemType.BadIngredient:

                if (badIngredientType == BadIngredientType.Random)
                {
                    badIngredientType = (BadIngredientType)randomIntValue(itemsManager.GetBadIngredientsSprites.Count);
                }
                SetTagAndSprite(badIngredientType.ToString(), (int)badIngredientType, itemsManager.GetBadIngredientsSprites);

                break;

        }
    }
    public void SetRandomItemType()
    {
        if (refreshColissionType)
        {
            itemType = ItemType.Random;
        }
        else if (refreshSubType)
        {
            obstacleType = ObstacleType.Random;
            ingredientType = IngredientType.Random;
            powerupType = PowerUpType.Random;
            badIngredientType = BadIngredientType.Random;
        }
        StopCoroutine("AnimatePowerup");
    }

    public void StopPowerUpAnimation()
    {
        StopCoroutine("AnimatePowerup");
    }

    IEnumerator AnimatePowerup()
    {
        int i = 0;
        List<Sprite> AnimationSprites = itemsManager.GetPowerUpAnimationSprites;
        while (true)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = AnimationSprites[i];
            i++;
            if (i > AnimationSprites.Count - 1) i = 0;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
