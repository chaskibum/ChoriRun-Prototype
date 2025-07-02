using UnityEngine;

public class PlayerSanti : MonoBehaviour
{
    [SerializeField] Transform TopLane;
    [SerializeField] Transform MiddleLane;
    [SerializeField] Transform BottomLane;
    [SerializeField] int Speed = 1;
    bool isMoving;
    int LaneToBe;
    Transform TargetTransform;
    public Transform ingredientContainer;
    
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
        if (other.CompareTag("Oil"))
        {
            Debug.Log("Collided with oil"  + other.tag);
        }
        else if (other.CompareTag("Pothole") || other.CompareTag("Cone"))
        {
            Debug.Log("Collided with Pothole/Cone"  + other.tag);
        }
        /*else if (other.CompareTag("Chorizo") || other.CompareTag("Lettuce") || other.CompareTag("Tomato") || other.CompareTag("Bread"))
        {
            Debug.Log("Collided with ingredient" + other.tag);
        }*/
        else if (other.CompareTag("Bread"))
        {
            if (ingredientContainer.GetChild(0).gameObject.activeInHierarchy)
            {
                ingredientContainer.GetChild(4).gameObject.SetActive(true);
            }
            else
            {
                ingredientContainer.GetChild(0).gameObject.SetActive(true);
            }
        }
        else if (other.CompareTag("Chorizo"))
        {
            ingredientContainer.GetChild(1).gameObject.SetActive(true);
        }
        else if (other.CompareTag("Lettuce"))
        {
            ingredientContainer.GetChild(2).gameObject.SetActive(true);
        }
        else if (other.CompareTag("Tomato"))
        {
            ingredientContainer.GetChild(3).gameObject.SetActive(true);
        }
    }
}
