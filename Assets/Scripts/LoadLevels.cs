using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class LoadLevels : MonoBehaviour
{
    private List<XmlDocument> _xmlFiles = new List<XmlDocument>();
    [SerializeField]
    private List<Level> _levels = new List<Level>();

    public delegate void VoidDelegate();
    public event VoidDelegate OnLevelsLoaded;

    private void Awake()
    {
        GetXMLFiles();
        CreateLevels();
        if (OnLevelsLoaded != null)
        {
            OnLevelsLoaded();
        }
    }

    private void GetXMLFiles()
    {
        foreach (string path in Directory.GetFiles(Application.dataPath + "/SavedLevels"))
        {
            if (path.EndsWith(".xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                _xmlFiles.Add(doc);
            }
        }
    }

    private void CreateLevels()
    {
        foreach (XmlDocument xml in _xmlFiles)
        {
            List<LevelObject> levelObjects = new List<LevelObject>();
            Inventory inventory = new Inventory();

            XmlNode startnode = xml.DocumentElement;

            foreach (XmlNode container in startnode.ChildNodes)
            {
                if (container.Name == "LevelObjects")
                {
                    foreach (XmlNode levelObject in container.ChildNodes)
                    {
                        int gridPosX = -1, gridPosY = -1, id = -1;
                        float xPos = -1, yPos = -1;
                        foreach (XmlAttribute property in levelObject.Attributes)
                        {
                            if (property.Name == "xpos") xPos = float.Parse(property.Value);
                            else if (property.Name == "ypos") yPos = float.Parse(property.Value);
                            if (property.Name == "gridxpos") gridPosX = int.Parse(property.Value);
                            else if (property.Name == "gridypos") gridPosY = int.Parse(property.Value);
                            else if (property.Name == "id") id = int.Parse(property.Value);
                        }

                        Vector2 pos = new Vector2(xPos, yPos);
                        GridPosition gridPos = new GridPosition(gridPosX, gridPosY);
                        levelObjects.Add(new LevelObject(pos, gridPos, id));
                    }
                }
                else if (container.Name == "Inventory")
                {
                    foreach (XmlNode inventoryItem in container.ChildNodes)
                    {
                        int id = -1, amount = -1;
                        foreach (XmlAttribute property in inventoryItem.Attributes)
                        {
                            if (property.Name == "id") id = int.Parse(property.Value);
                            else if (property.Name == "amount") amount = int.Parse(property.Value);
                        }
                        inventory.AddItem(id, amount);
                    }
                }
            }

            _levels.Add(new Level(inventory, levelObjects.ToArray()));
        }
    }

    public Level GetLevel(int id)
    {
        return _levels[id];
    }
}
