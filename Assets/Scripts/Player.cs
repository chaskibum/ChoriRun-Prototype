using UnityEngine;

public class Player : MonoBehaviour
{
    private int _hp = 2;
    public GameManager manager;
    [SerializeField] Transform TopLane;
    [SerializeField] Transform MiddleLane;
    [SerializeField] Transform BottomLane;
    [SerializeField] int Speed = 1;
    bool isMoving;
    int LaneToBe;
    Transform TargetTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoving = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            if (LaneToBe == 1)
            {
                TargetTransform = TopLane;
            }
            else if (LaneToBe == 0)
            {
                TargetTransform = MiddleLane;
            }
            else if (LaneToBe == -1)
            {
                TargetTransform = BottomLane;
            }

            transform.position = Vector3.Lerp(transform.position, TargetTransform.position, Speed * Time.deltaTime);

            if (transform.position == TargetTransform.position)
            {
                isMoving = false;
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (LaneToBe < 1)
            {
                LaneToBe++;
                isMoving = true;
            }
            
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (LaneToBe > -1)
            {
                LaneToBe--;
                isMoving = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Obstacle"))
        {
            other.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void ChangeHp(int amount = -1)
    {
        _hp += amount;
        Debug.Log(_hp);
        if (_hp <= 0)
        {
            manager.EndGame();
        }
    }
}
