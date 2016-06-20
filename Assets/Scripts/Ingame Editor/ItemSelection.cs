using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour
{
    // load game data
    [SerializeField]
    private Texture2D _cursorTex;
    [SerializeField]
    private GameObject[] _availableItemObects;
    [Space(10)]
    [SerializeField]
    private GameObject _uiPrefab, _textPrefab;
    [SerializeField]
    private Canvas _canvas;
    private int _gridDetail = 100;
    public Inventory _inventory;

    // in game data
    private Item _selectedItem;
    private float _snappedX, _snappedY;
    private int _gridX, _gridY;
    private Item[][] _levelData;
    private List<Button> _currentUIItems;
    private int _levelHeight, _levelWidth;
    private List<Text> _tileAmountTexts;

    // level data
    private GridPosition[] _persistentLevelObjects;

    private void Awake()
    {
        Debug.Log("Awake");
        Cursor.SetCursor(_cursorTex, new Vector2(0, 0), CursorMode.Auto);

        _levelHeight = Mathf.CeilToInt(Camera.main.orthographicSize * 2);
        _levelWidth = Mathf.CeilToInt(_levelHeight * Camera.main.aspect);
        _levelWidth = _levelWidth * _gridDetail;
        _levelHeight = _levelHeight * _gridDetail;

        _levelData = new Item[_levelWidth][];
        for (int x = 0; x < _levelWidth; x++)
        {
            _levelData[x] = new Item[_levelHeight];
        }
    }

    public void FillInventory()
    {
        Inventory inv = new Inventory();
        for (int i = 0; i < _availableItemObects.Length; i++)
        {
            inv.AddItem(i, 99);
        }
        SetInventory(inv);
    }

    private void SetInventory(Inventory inventory)
    {
        _currentUIItems = new List<Button>();
        _tileAmountTexts = new List<Text>();

        ClearOldInventory();
        _inventory = inventory;
        for (int i = 0; i < _inventory.Items.Count; i++)
        {
            int itemID = _inventory.Items[i].ID;
            GameObject UIitem = Instantiate(_uiPrefab) as GameObject;
            Button UIButton = UIitem.GetComponent<Button>();
            UIitem.name = "UI item " + itemID;
            UIitem.GetComponent<Image>().sprite = _availableItemObects[itemID].GetComponent<SpriteRenderer>().sprite;
            UIButton.onClick.AddListener(() => GrabItem(itemID));

            UIitem.transform.SetParent(gameObject.transform);
            transform.localScale = Vector3.one;
            _currentUIItems.Add(UIButton);

            GameObject o = Instantiate(_textPrefab);
            o.transform.SetParent(UIitem.transform);
            Text txt = o.GetComponent<Text>();
            _tileAmountTexts.Add(txt);
            txt.text = _inventory.Items[i].Amount.ToString();
        }
    }

    private void ClearOldInventory()
    {
        for (int i = 0; i < _currentUIItems.Count; i++)
        {
            Destroy(_currentUIItems[i].gameObject);
        }
        _currentUIItems = new List<Button>();
    }

    public void GrabItem(int id)
    {
        if (_inventory.GetItemAmount(id) > 0)
        {
            RemoveSelectedItem();
            int textID = _inventory.GetPosition(id);
            _inventory.DecreaseItemAmount(id);
            _tileAmountTexts[textID].text = _inventory.GetItemAmount(id).ToString();
            GameObject item = Instantiate(_availableItemObects[id]);
            _selectedItem = item.GetComponent<Item>();
            Sprite spr = _selectedItem.GetComponent<Sprite>();
            _selectedItem.ID = id;

            if (_inventory.GetItemAmount(id) == 0)
            {
                _currentUIItems[textID].interactable = false;
            }
        }
    }

    private bool NotStaticOnLevel(int x, int y)
    {
        if (_persistentLevelObjects == null) return true;
        for (int i = 0; i < _persistentLevelObjects.Length; i++)
        {
            GridPosition p = _persistentLevelObjects[i];
            if (p.X == x && p.Y == y)
            {
                return false;
            }
        }
        return true;
    }

    private void Update()
    {
        SetMouseGridPos();
        if (_selectedItem != null)
        {
            _selectedItem.transform.position = new Vector2(_snappedX, _snappedY);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
            {
                GetOrPlaceSelectedItem();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RemoveSelectedItem();
        }

    }

    private void RemoveSelectedItem()
    {
        if (_selectedItem != null)
        {
            int oldItemID = _selectedItem.ID;
            _inventory.Items[oldItemID].Amount++;
            _tileAmountTexts[_selectedItem.ID].text = _inventory.Items[oldItemID].Amount.ToString();
            Destroy(_selectedItem.gameObject);

            if (_inventory.Items[oldItemID].Amount == 1)
            {
                _currentUIItems[oldItemID].interactable = true;
            }
        }

    }

    private void GetOrPlaceSelectedItem()
    {
        PlayMode.Instance.Stop();
        if (_selectedItem == null)
        {
            if (_levelData[_gridX][_gridY] != null)
            {
                if (NotStaticOnLevel(_gridX, _gridY))
                {
                    int id = _levelData[_gridX][_gridY].ID;
                    int itemPos = _inventory.GetPosition(id);
                    _inventory.Items[itemPos].Amount++;
                    Destroy(_levelData[_gridX][_gridY].gameObject);
                    GrabItem(id);
                }
            }
        }
        else
        {
            if (CanPlaceItem(_selectedItem))
            {
                Vector2 center = new Vector2(_snappedX, _snappedY);
                GridPosition gridCenter = new GridPosition(_gridX, _gridY);
                PlaceObjectOnGrid(_selectedItem,center,gridCenter);
                _selectedItem = null;
            }
        }
    }

    private bool CanPlaceItem(Item item)
    {
        // set center of object
        float centerX = _snappedX;
        float centerY = _snappedY;

        // set width and height in grid units
        float gridUnitSize = (1f / _gridDetail);
        int width = Mathf.CeilToInt((item.Width / gridUnitSize));
        int height = Mathf.CeilToInt((item.Height / gridUnitSize));

        int leftX = _gridX - width / 2;
        int bottomY = _gridY - height / 2;

        for (int x = leftX; x < leftX + width; x++)
        {
            for (int y = bottomY; y < bottomY + height; y++)
            {
                if (x > 0 && x < _levelWidth && y > 0 && y < _levelHeight)
                {
                    if (_levelData[x][y] != null)
                    {
                        Debug.Log("CanPlace = false");
                        return false;
                    }
                }
                else return false;
            }
        }
        return true;
    }

    private void PlaceObjectOnGrid(Item item, Vector2 center, GridPosition gridCenter)
    {
        Debug.Log("PLACING OBJECT" + item.gameObject);
        // set width and height in grid units
        float gridUnitSize = (1f / _gridDetail);
        int width = Mathf.CeilToInt((item.Width / gridUnitSize));
        int height = Mathf.CeilToInt((item.Height / gridUnitSize));
        int leftX = gridCenter.X - width / 2;
        int bottomY = gridCenter.Y - height / 2;
        item.transform.position = center;
        item.Pos = center;
        item.PosOnGrid = gridCenter;
        for (int x = leftX; x < leftX + width; x++)
        {
            for (int y = bottomY; y < bottomY + height; y++)
            {
                if (y < _levelData[x].Length)
                {
                    _levelData[x][y] = item;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_levelData != null)
        {
            for (int x = 0; x < _levelData.Length; x++)
            {

                for (int y = 0; y < _levelData[0].Length; y++)
                {
                    if (_levelData[x][y] != null)
                    {
                        Gizmos.color = Color.red;
                        Item i = _levelData[x][y];
                        Gizmos.DrawCube(i.Pos, Vector3.one * i.Width);
                    }
                }
            }
        }
    }

    private void SetMouseGridPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = ((float)System.Math.Round(mousePos.x, 3));
        float y = ((float)System.Math.Round(mousePos.y, 3));
        Vector2 selectedPosition = new Vector2(x, y);

        float maxX = (_levelHeight * _gridDetail / 2);
        float maxY = (_levelHeight * _gridDetail / 2);
        float minX = -maxX;
        float minY = -maxY;

        if (x > minX && x < maxX && y > minY && y < maxY)
        {
            _snappedX = x;
            _snappedY = y;

            _gridX = Mathf.RoundToInt(x * _gridDetail) + _levelWidth / 2;
            _gridY = Mathf.RoundToInt(y * _gridDetail) + _levelHeight / 2;
        }
    }

    public LevelObject[] GetLevelObjects()
    {
        List<Item> addedItems = new List<Item>();
        List<LevelObject> levelObjects = new List<LevelObject>();
        for (int i = 0; i < _levelData.GetLength(0); i++)
        {
            for (int j = 0; j < _levelData[0].GetLength(0); j++)
            {
                Item item = _levelData[i][j];
                if (item != null && !addedItems.Contains(item))
                {
                    Debug.Log(item.ID + " " + i + " " + j);
                    GridPosition pos = new GridPosition(i, j);
                    int id = item.ID;
                    levelObjects.Add(new LevelObject(item.Pos, item.PosOnGrid, id));
                    addedItems.Add(item);
                }
            }
        }
        return levelObjects.ToArray();
    }

    public void PrepareLevel(Level l)
    {
        PlayMode.Instance.Stop();
        ClearGrid();
        _persistentLevelObjects = new GridPosition[l.LevelObjects.Length];
        for (int i = 0; i < l.LevelObjects.Length; i++)
        {
            LevelObject o = l.LevelObjects[i];
            _persistentLevelObjects[i] = new GridPosition(o.GridPos.X, o.GridPos.Y);
            GameObject itemObject = Instantiate(_availableItemObects[o.ID]);
            itemObject.transform.position = new Vector2(0,0);
            Item item = itemObject.GetComponent<Item>();
            PlaceObjectOnGrid(item,o.Pos,o.GridPos);
        }

        SetInventory(l.Inventory);
    }

    private void ClearGrid()
    {
        Debug.Log("Clearing the Grid");
        for (int i = 0; i < _levelWidth; i++)
        {
            for (int j = 0; j < _levelHeight; j++)
            {
                Item g = _levelData[i][j];
                if (g != null)
                {
                    Destroy(g.gameObject);
                }
            }
        }
    }
}
