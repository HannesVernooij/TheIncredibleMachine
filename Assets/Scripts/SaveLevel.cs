using UnityEngine;
using System.IO;

public class SaveLevel : MonoBehaviour
{
    [SerializeField]
    private int _levelID;
    [SerializeField]
    private ItemSelection _itemSelectionScript;
    [SerializeField]
    private int[] _itemAmounts;
    [SerializeField]
    private bool _save = false;
    private void Update()
    {
        if(_save)
        {
            Save();
        }
    }

    public void Save()
    {
        _save = false;
        StreamWriter writer = new StreamWriter(Application.dataPath + "/SavedLevels/Level" + _levelID + ".xml");

        writer.WriteLine("<Level>");
        writer.WriteLine("  <LevelObjects>");

        LevelObject[] levelObjects = _itemSelectionScript.GetLevelObjects();
        foreach (LevelObject o in levelObjects)
        {
           writer.WriteLine("      <LevelObject xpos=\"" + o.Pos.x + "\" ypos=\"" + o.Pos.y + " \" gridxpos=\"" + o.GridPos.X + " \" gridypos=\"" +  o.GridPos.Y + "\" id=\"" + o.ID + "\"></LevelObject>");
        }
        writer.WriteLine("  </LevelObjects>");

        Inventory inventory = new Inventory();

        for (int i = 0; i < _itemAmounts.Length; i++)
        {
            int amount = _itemAmounts[i];
            if (amount != 0)
            {
                inventory.AddItem(i, amount);
            }
        }

        writer.WriteLine("  <Inventory>");

        for (int i = 0; i < inventory.Items.Count; i++)
        {
            string id = inventory.Items[i].ID.ToString();
            string amount = inventory.Items[i].Amount.ToString();
            writer.WriteLine("      <InventoryItem id=\"" +id + "\" amount=\"" + amount + "\"></InventoryItem>");
        }

        writer.WriteLine("  </Inventory>");
        writer.WriteLine("</Level>");
        writer.Close();
    }
}
