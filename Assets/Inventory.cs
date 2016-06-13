using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory
{
    private List<int> _itemList = new List<int>();
    private List<int> _itemAmountList = new List<int>();
    private List<Text> _itemAmountTexts = new List<Text>();

    public List<int> ItemList { get { return _itemList; } }
    public List<int> ItemAmountList { get { return _itemAmountList; } }

    public void AddItem(int itemID, int amount)
    {
        ItemList.Add(itemID);
        ItemAmountList.Add(amount);
    }

    public int GetAndDecreaseItemAmount(int itemID)
    {
        for(int i = 0; i < _itemList.Count; i++)
        {
            if(itemID == _itemList[i])
            {
                _itemAmountList[itemID]--;
                return _itemAmountList[itemID];
            }
        }
        return 0;
    }
}
