using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSanti : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float SpeedIncreseAmount = 1;
    [SerializeField] int TimeTillIncrese = 1;
    public bool isGamePaused;

    [Header("Obstacles")]
    [SerializeField] Transform ObstaclesContainer;
    public float ObstaclesSpeed;
    float startObstacleSpeed;
    [SerializeField] int MaxObstaclesSpeed;
    public int SpaceBetweenObstaclesGroup = 5;
    [Header("Rare Group Obstacles")]
    [SerializeField] Transform RareObstaclesContainer;
    [SerializeField] float TimeTillNextRareGroup;
    List<Transform> RareGroupsList = new List<Transform>();

    [Header("Background")]
    [SerializeField] Transform ForegroundContainer;
    [SerializeField] Transform BackgroundContainer;
    public float BackgroundSpeed;
    float startBackgroundSpeed;
    [SerializeField] int MaxBackgroundSpeed;

    [Header("Probabilitys")]
    public float obstacleProbability = 0;
    public float ingredientProbability = 0;
    public float powerUpProbability = 0;




    PlayerSanti player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerSanti>();
        startBackgroundSpeed = BackgroundSpeed;
        startObstacleSpeed = ObstaclesSpeed;
        SortForeground();
        SortBackground();
        SortObstacles();
        StartCoroutine(GraduallyIncreaseBackgroundSpeed());
        StartCoroutine(GraduallyIncreaseObstacleSpeed());
        StartCoroutine(AddRareGroupToRun());
        foreach (Transform rareGroup in RareObstaclesContainer)
        {
            RareGroupsList.Add(rareGroup);
        }
        if (SpaceBetweenObstaclesGroup == 0)
            {
                SpaceBetweenObstaclesGroup = 1;
            }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SortItems(Transform objectToSortContainer, int SpaceBetweenObjects = 0, bool RandomIndex = true)
    {
        float currentX = 0f;
        if (RandomIndex)
        {
            foreach (Transform ObjectInContainer in objectToSortContainer)
            {
                ObjectInContainer.SetSiblingIndex(Random.Range(0, objectToSortContainer.childCount));
            }            
        }

        for (int i = 0; i < objectToSortContainer.childCount; i++)
        {
            Transform ObjectInContainer = objectToSortContainer.GetChild(i);

            BoxCollider2D objectCollider = ObjectInContainer.GetComponent<BoxCollider2D>();

            float obstacleCenter = currentX + (objectCollider.size.x / 2f) - objectCollider.offset.x;

            ObjectInContainer.localPosition = new Vector2(obstacleCenter, 0);

            currentX += objectCollider.size.x + SpaceBetweenObjects;
        }
    }
    IEnumerator GraduallyIncreaseObstacleSpeed()
    {
        while (ObstaclesSpeed < MaxObstaclesSpeed)
        {
            float ActualSpeed = ObstaclesSpeed;
            while (!Mathf.Approximately(ObstaclesSpeed, ActualSpeed + SpeedIncreseAmount))
            {
                ObstaclesSpeed = Mathf.MoveTowards(ObstaclesSpeed, ActualSpeed + SpeedIncreseAmount, Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(TimeTillIncrese);
        }
    }
    IEnumerator GraduallyIncreaseBackgroundSpeed()
    {
        while (BackgroundSpeed < MaxBackgroundSpeed)
        {
            float ActualSpeed = BackgroundSpeed;
            while (!Mathf.Approximately(BackgroundSpeed, ActualSpeed + SpeedIncreseAmount))
            {
                BackgroundSpeed = Mathf.MoveTowards(BackgroundSpeed, ActualSpeed + SpeedIncreseAmount, Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(TimeTillIncrese);
        }
    }
    IEnumerator AddRareGroupToRun()
    {
        while (true)
        {
            if (RareObstaclesContainer.childCount > 0)
            {
                yield return new WaitForSeconds(TimeTillNextRareGroup);
                Transform GroupToAdd = RareObstaclesContainer.GetChild(Random.Range(0, RareObstaclesContainer.childCount));
                Transform WhereToAdd = ObstaclesContainer.GetChild(Random.Range(4, ObstaclesContainer.childCount));
                GroupToAdd.parent = ObstaclesContainer;
                GroupToAdd.SetSiblingIndex(WhereToAdd.GetSiblingIndex());
                GroupToAdd.position = WhereToAdd.position;
                GroupToAdd.gameObject.SetActive(true);
                yield return new WaitUntil(() => !GroupToAdd.gameObject.activeSelf);
            }
            yield return null;
        }

    }
    void SortForeground()
    {
        SortItems(ForegroundContainer, 0, false);
    }
    void SortBackground()
    {
        SortItems(BackgroundContainer, 0, false);
    }
    void SortObstacles()
    {
        SortItems(ObstaclesContainer, SpaceBetweenObstaclesGroup);
    }
    public void Restart()
    {
        StopAllCoroutines();
        ObstaclesSpeed = startObstacleSpeed;
        BackgroundSpeed = startBackgroundSpeed;
        SortForeground();
        SortBackground();
        SortObstacles();
        player.PositionOnRestart();
        foreach (Transform rareGroup in RareGroupsList)
        {
            rareGroup.parent = RareObstaclesContainer;
            rareGroup.gameObject.SetActive(false);
        }
        foreach (Transform ObstaclesGroup in ObstaclesContainer)
        {
            ObstaclesGroup.GetComponent<ObstacleGroupBehaviorSanti>().RefreshObstacles();
        }
        StartCoroutine(GraduallyIncreaseBackgroundSpeed());
        StartCoroutine(GraduallyIncreaseObstacleSpeed());
        StartCoroutine(AddRareGroupToRun());
        //Resetear puntos
    }

}
