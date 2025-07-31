using System.Collections;
using UnityEngine;

public class IngredientAnimation : MonoBehaviour
{
    [SerializeField] int Speed;
    [SerializeField] int ScaleSpeed;
    Vector3 TargetPos;
    Vector3 StartScale;
    Transform Parent;
    GameManagerBeta gameManager;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
    }
    void Start()
    {
        Parent = transform.parent;
        StartScale = transform.localScale;
        gameManager.GetOnRestartEvent?.AddListener(OnRestart);
    }
    
    public void AnimateObject(Sprite IngredientSprite, Vector3 StartPosition, Vector3 TargetPosition, bool ChangeScale = false)
    {
        gameObject.SetActive(true);
        transform.parent = null;
        transform.position = StartPosition;
        TargetPos = TargetPosition;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = IngredientSprite;
        StartCoroutine(MoveToTarget(TargetPosition));
        if (ChangeScale) StartCoroutine(ChangeObjectScale());
    }
    IEnumerator MoveToTarget(Vector3 TargetPosition)
    {
        while (Vector3.Distance(transform.position, TargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Speed * 10 * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
        transform.parent = Parent;
    }
    IEnumerator ChangeObjectScale()
    {
        while (gameObject.activeSelf)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(0, 0, 0), ScaleSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnDisable()
    {
        transform.localScale = StartScale;
    }
    void OnRestart()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
        transform.parent = Parent;
    }
    public Vector3 GetTargetPos => TargetPos;
}
