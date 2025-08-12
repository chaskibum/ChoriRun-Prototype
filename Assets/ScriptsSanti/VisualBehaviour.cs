using System.Collections;
using UnityEngine;

public class VisualBehaviour : MonoBehaviour
{
    GameManagerBeta gameManager;
    BackgroundGroup backgroundGroup;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        backgroundGroup = transform.parent.GetComponent<BackgroundGroup>();
    }
    void Update()
    {
        if (gameManager.GetGameStarted)
        {
            if (backgroundGroup.GetUseSameSpeedAsItems)
            {
                transform.position += Vector3.left * gameManager.GetItemsGroupSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.left * backgroundGroup.GetSpeed * Time.deltaTime;
            }
        }
    }
    public void DecreseAlpha()
    {

    }

    #region Colission
    float GetPreviousRightEdge(int currentIndex)
    {
        Transform ChildTransform = transform.parent.GetChild(currentIndex - 1);
        var ChildBoxCollider = ChildTransform.GetComponent<BoxCollider2D>();

        return ChildTransform.localPosition.x + ChildBoxCollider.offset.x + (ChildBoxCollider.size.x / 2f);
    }
    Vector2 CalculateNewPosition(int index, float PreviousRightEdge)
    {
        var ChildBoxCollider = transform.parent.GetChild(index).GetComponent<BoxCollider2D>();
        float LeftEdge = (ChildBoxCollider.size.x / 2f) - ChildBoxCollider.offset.x;
        float newXPos = PreviousRightEdge + backgroundGroup.GetSpaceBtwVisual +LeftEdge;
        return new Vector2(newXPos, 0);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Limit"))
        {
            int index = transform.GetSiblingIndex();
            if (index - 1 >= 0)
            {
                transform.localPosition = CalculateNewPosition(index, GetPreviousRightEdge(index));
            }
            else
            {
                transform.localPosition = CalculateNewPosition(index, GetPreviousRightEdge(transform.parent.childCount));
            }
        }
    }
    #endregion
}
