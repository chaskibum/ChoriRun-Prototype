using System.Collections;
using UnityEngine;
public class BackgroundGroup : MonoBehaviour
{
    GameManagerBeta gameManager;
    float startSpeed;
    [SerializeField] float Speed;
    [SerializeField] int MaxSpeed;
    [SerializeField] bool UseSameSpeedAsObstacles;
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    void Start()
    {
        startSpeed = Speed;
        gameManager.SortItems(transform);
        StartCoroutine(GraduallyIncreaseSpeed());
        gameManager.GetOnRestartEvent.AddListener(OnRestart);
    }
    // Update is called once per frame
    void Update()
    {
        if (UseSameSpeedAsObstacles)
        {
            transform.position += Vector3.left * gameManager.GetItemsGroupSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.left * Speed * Time.deltaTime;
        }
    }
    IEnumerator GraduallyIncreaseSpeed()
    {
        while (Speed < MaxSpeed)
        {
            float ActualSpeed = Speed;
            while (!Mathf.Approximately(Speed, ActualSpeed + gameManager.GetSpeedIncreseAmount))
            {
                Speed = Mathf.MoveTowards(Speed, ActualSpeed + gameManager.GetSpeedIncreseAmount, Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(gameManager.GetTimeTillIncrese);
        }
    }

    void OnRestart()
    {
        StopAllCoroutines();
        Speed = startSpeed;
        StartCoroutine(GraduallyIncreaseSpeed());
    }
}
