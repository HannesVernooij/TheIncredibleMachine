using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory
{
    private List<InventoryItem> _items = new List<InventoryItem>();
    public List<InventoryItem> Items { get { return _items; } set { _items = value; } }

    public void AddItem(int itemID, int amount)
    {
        _items.Add(new InventoryItem(itemID,amount));
    }

    public int GetItemAmount(int itemID)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if(_items[i].ID == itemID)
            {
                return _items[i].Amount;
            }
        }
        return 0;
    }

    public void DecreaseItemAmount(int itemID)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].ID == itemID)
            {
                _items[i].Amount--;
            }
        }
    }

    public int GetPosition(int itemID)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].ID == itemID)
            {
                return i;
            }
        }
        return 0;
    }
}
