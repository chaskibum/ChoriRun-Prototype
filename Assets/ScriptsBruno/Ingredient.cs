using UnityEngine;
using UnityEngine.Events;

public class Ingredient : MonoBehaviour
{
    public IngredientType type;

    public Transform ingredientVisuals;

    public UnityEvent onIngredientCollected;
    
    public enum IngredientType
    {
        Bread,
        Chori,
        Lettuce,
        Tomato,
        Bread2,
    }

    private void Start()
    {
        foreach (Transform child in ingredientVisuals)
        {
            child.gameObject.SetActive(false);
        }
        
        // ingredientVisuals.GetChild((int)type).gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (other.gameObject.TryGetComponent(out ChoriInventory inventory))
        {
            if (!inventory.AddItem(this)) return;
            onIngredientCollected?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public IngredientType GetIngredientType() => type;
}
