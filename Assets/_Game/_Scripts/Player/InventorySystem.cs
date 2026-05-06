using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("My Backpack")]
    // We use a List so we can add/remove items easily in the Inspector
    public List<InventorySlot> backpack = new List<InventorySlot>();

    // A helper function to add items (we will use this later when picking things up)
    public void AddItem(ItemData itemToAdd, int amount)
    {
        // 1. Check if we already have a stack of this item
        foreach (InventorySlot slot in backpack)
        {
            if (slot.item == itemToAdd)
            {
                slot.quantity += amount;
                Debug.Log($"Added {amount} to existing stack of {itemToAdd.itemName}");
                return;
            }
        }

        // 2. If not, create a new slot
        backpack.Add(new InventorySlot(itemToAdd, amount));
        Debug.Log($"Added new item: {itemToAdd.itemName}");
    }
}

// This tiny class helps Unity show the list nicely in the Inspector
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot(ItemData _item, int _qty)
    {
        item = _item;
        quantity = _qty;
    }
}
