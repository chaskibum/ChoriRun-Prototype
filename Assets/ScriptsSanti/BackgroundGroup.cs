using System.Collections;
using UnityEngine;
public class BackgroundGroup : MonoBehaviour
{
    GameManagerBeta gameManager;
    float startSpeed;
    private float timeTillIncrease;
    [Min(0),SerializeField] float Speed;
    [SerializeField] int MaxSpeed;
    [SerializeField] int SpaceBetweenVisuals = 0;
    [SerializeField] bool UseSameSpeedAsItems;
    [SerializeField] private bool isSecondBackground;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    void Start()
    {
        startSpeed = Speed;
        gameManager.SortItems(transform, SpaceBetweenVisuals);
        gameManager.GetOnstartEvent?.AddListener(OnStart);
    }
    void OnStart()
    {
        Speed = startSpeed;
        StartCoroutine(GraduallyIncreaseSpeed());
        gameManager.GetOnRestartEvent.AddListener(OnRestart);
    }
    IEnumerator GraduallyIncreaseSpeed()
    {
        while (Speed < MaxSpeed)
        {
            float timer = 0;
            if (isSecondBackground)
            {
                timeTillIncrease = gameManager.GetTimeTillIncrese * 2 + 0.5f;
            }
            else
            {
                timeTillIncrease = gameManager.GetTimeTillIncrese;
            }
            while (timer < timeTillIncrease)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            float ActualSpeed = Speed;
            
            while (!Mathf.Approximately(Speed, ActualSpeed + gameManager.GetSpeedIncreseAmount))
            {
                Speed = Mathf.MoveTowards(Speed, ActualSpeed + gameManager.GetSpeedIncreseAmount, Time.deltaTime);
                yield return null;
            }
        }
    }

    void OnRestart()
    {
        StopAllCoroutines();
        Speed = startSpeed;
        StartCoroutine(GraduallyIncreaseSpeed());
    }

    public bool GetUseSameSpeedAsItems => UseSameSpeedAsItems;
    public float GetSpeed => Speed;
    public int GetSpaceBtwVisual => SpaceBetweenVisuals;
}
