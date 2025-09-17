using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DogsMovement : MonoBehaviour
{
    [Header("Tranforms")]
    [SerializeField] Transform PlayerPosition;
    [SerializeField] Transform StartPosition;
    [Header("Properties")]
    [SerializeField] int ChaseDuration;
    [Space(10)]
    [SerializeField] float StartSpeed;
    [SerializeField] float EndSpeed;
    [Space(10)]
    [SerializeField] float DistanceFromPlayer;
    [SerializeField] float DistanceFromPlayerOnEnd;
    bool CanStopChase;
    Coroutine ChaseCoroutine;
    Coroutine StopChaseCoroutine;
    BoxCollider2D boxCollider;
    GameManagerBeta gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);
        gameManager.GetOnstartEvent?.AddListener(OnPlay);
        gameManager.GetOnRestartEvent?.AddListener(OnRestart);
    }

    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartChase();
        }
    }
    */


    public void StartChase()
    {
        if (ChaseCoroutine != null)
        {
            StopCoroutine(ChaseCoroutine);
            ChaseCoroutine = StartCoroutine(Chase(DistanceFromPlayer, StartSpeed, true));

        }
        else ChaseCoroutine = StartCoroutine(Chase(DistanceFromPlayer, StartSpeed, true));

        if (StopChaseCoroutine == null) StopChaseCoroutine = StartCoroutine(WaitToStop());
        else
        {
            StopCoroutine(StopChaseCoroutine);
            StopChaseCoroutine = StartCoroutine(WaitToStop());
        }

        CanStopChase = true;
    }

    public void StopChase(float SpeedMultiplier = 1, bool IgnoreBool = false)
    {
        if (!CanStopChase && !IgnoreBool) return;
        StopCoroutine(ChaseCoroutine);
        ChaseCoroutine = StartCoroutine(Chase(DistanceFromPlayerOnEnd, EndSpeed * SpeedMultiplier, false));
        CanStopChase = false;
    }

    IEnumerator Chase(float DistanceFromPlayer, float Speed, bool ColliderActive)
    {
        while (true)
        {
            float TargetPositionX = PlayerPosition.position.x - DistanceFromPlayer;
            float TargetPositionY = PlayerPosition.position.y;

            float newX = Mathf.Lerp(transform.position.x, TargetPositionX, Speed * Time.deltaTime);
            float newY = Mathf.Lerp(transform.position.y, TargetPositionY, StartSpeed * Time.deltaTime);

            Vector3 TargetPosition = new Vector3(newX, newY);

            transform.position = TargetPosition;
            if (Mathf.Abs(transform.position.x - TargetPositionX) < 6f) boxCollider.enabled = ColliderActive;
            yield return null;
        }
    }

    IEnumerator WaitToStop()
    {
        yield return new WaitForSeconds(ChaseDuration);
        StopChase();
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer CollisionChildSpriteRenderer(int index = 0) { return other.transform.GetChild(index).GetComponent<SpriteRenderer>(); }
        if (other.CompareTag("Cone"))
        {
            StartCoroutine(ChangeSortingLayerForTime(CollisionChildSpriteRenderer(), 0.5f, "UI", CollisionChildSpriteRenderer().sortingLayerName));
            other.GetComponent<Animator>().SetTrigger("Throw");
            AudioManager.Instance.PlayClip(AudioManager.AudioList.DestroyObstacleSound, true);
        }
    }

    void OnPlay()
    {
        StartChase();
    }

    void OnRestart()
    {
        StopAllCoroutines();
        StartChase();
        transform.position = StartPosition.position;
    }

    void OnQuit()
    {
        StopAllCoroutines();
        StopChase();
    }
}
