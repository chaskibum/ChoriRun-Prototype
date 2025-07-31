using System.Collections;
using UnityEngine;
public class BackgroundGroup : MonoBehaviour
{
    GameManagerBeta gameManager;
    float startSpeed;
    [Min(0),SerializeField] float Speed;
    [SerializeField] int MaxSpeed;
    [SerializeField] int SpaceBetweenVisuals = 0;
    [SerializeField] bool UseSameSpeedAsItems;
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
            while (timer < gameManager.GetTimeTillIncrese)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            // AudioManager.Instance.motorbikeSound.pitch += 0.02f;
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
