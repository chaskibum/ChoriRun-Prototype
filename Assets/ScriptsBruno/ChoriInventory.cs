using System.Collections.Generic;
using UnityEngine;

public class ChoriInventory : MonoBehaviour
{
    public List<Ingredient> inventory;
    private const int MaxItemsSize = 5;
    
    public bool AddItem(Ingredient ingredient)
    {
        if (inventory.Count >= MaxItemsSize) return false;
        inventory.Add(ingredient);
        return true;
    }
    
    // Reemplazar una linea de return con flechita apuntando a lo que queremos devolver. 
    public void RemoveItem(Ingredient ingredient) => inventory.Remove(ingredient);

    public bool CheckForItem(Ingredient ingredient) => inventory.Contains(ingredient);
    
    public List<Ingredient> GetIngredients() => inventory;
}
