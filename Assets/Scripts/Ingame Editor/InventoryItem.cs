using UnityEngine.UI;

public class InventoryItem
{
    private int _id;
    private int _amount;
    private Text _text;

    public int ID { get { return _id; } }
    public int Amount { get { return _amount; } set { _amount = value; } }

    public InventoryItem(int id, int amount)
    {
        _id = id;
        _amount = amount;
    }
}
