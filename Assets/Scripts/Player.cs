using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager manager;
    public AudioSource crashSound;
    public AudioSource motorbikeSound;
    [SerializeField] Transform topLane;
    [SerializeField] Transform middleLane;
    [SerializeField] Transform bottomLane;
    [SerializeField] int speed = 1;
    
    private int _hp = 2;
    private bool _isMoving;
    private int _laneToBe;
    private Transform _targetTransform;

    void Start()
    {
        _isMoving = true;
    }


    void FixedUpdate()
    {
        if (_isMoving)
        {
            if (_laneToBe == 1)
            {
                _targetTransform = topLane;
            }
            else if (_laneToBe == 0)
            {
                _targetTransform = middleLane;
            }
            else if (_laneToBe == -1)
            {
                _targetTransform = bottomLane;
            }

            transform.position = Vector3.Lerp(transform.position, _targetTransform.position, speed * Time.deltaTime);

            if (transform.position == _targetTransform.position)
            {
                _isMoving = false;
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (_laneToBe < 1)
            {
                _laneToBe++;
                _isMoving = true;
            }
            
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_laneToBe > -1)
            {
                _laneToBe--;
                _isMoving = true;
            }
        }
    }
    
    /*private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Obstacle"))
        {
            other.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }*/

    public void ChangeHp(int amount = -1)
    {
        _hp += amount;
        manager.HpFeedback(_hp);
        if (_hp <= 0)
        {
            motorbikeSound.Stop();
            crashSound.Play();
            manager.EndGame();
        }
    }
}
