﻿using UnityEngine;
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
    private List<Button> _currentUIItems = new List<Button>();
    private int _levelHeight, _levelWidth;
    private List<Text> _tileAmountTexts = new List<Text>();

    // debug data
    [Space(10)]
    [SerializeField]
    private Text _debugTxt;

    private void Start()
    {
        Cursor.SetCursor(_cursorTex, new Vector2(0, 0), CursorMode.Auto);
        Inventory newInv = new Inventory();
        newInv.AddItem(0, 5);
        newInv.AddItem(1, 5);
        newInv.AddItem(2, 5);

        SetInventory(newInv);

        _levelHeight = Mathf.CeilToInt(Camera.main.orthographicSize * 2);
        _levelWidth = Mathf.CeilToInt(_levelHeight * Camera.main.aspect);
        _levelWidth = _levelWidth * _gridDetail;
        _levelHeight = _levelHeight * _gridDetail;

        _levelData = new Item[_levelWidth][];
        for (int x = 0; x < _levelWidth; x++)
        {
            _levelData[x] = new Item[_levelHeight];
        }
        Debug.Log(_levelData.Length);
    }

    private void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
        for (int i = 0; i < _inventory.ItemList.Count; i++)
        {
            int itemID = _inventory.ItemList[i];
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
            txt.text = _inventory.ItemAmountList[i].ToString();
        }
    }

    public void GrabItem(int id)
    {
        if (_inventory.ItemAmountList[id] > 0)
        {
            RemoveSelectedItem();

            _inventory.GetAndDecreaseItemAmount(id);
            _tileAmountTexts[id].text = _inventory.ItemAmountList[id].ToString();
            GameObject item = Instantiate(_availableItemObects[id]);
            _selectedItem = item.GetComponent<Item>();
            Sprite spr = _selectedItem.GetComponent<Sprite>();
            _selectedItem.ID = id;

            if(_inventory.ItemAmountList[id] == 0)
            {
                _currentUIItems[id].interactable = false;
            }
        }
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
            _inventory.ItemAmountList[oldItemID]++;
            _tileAmountTexts[_selectedItem.ID].text = _inventory.ItemAmountList[oldItemID].ToString();
            Destroy(_selectedItem.gameObject);
            
            if(_inventory.ItemAmountList[oldItemID] == 1)
            {
                _currentUIItems[oldItemID].interactable = true;
            }
            //if inv item not exists -> add
            //_inventory.AddItem(_selectedItem.ID, 1);
        }

    }

    private void GetOrPlaceSelectedItem()
    {
        if (_selectedItem == null)
        {
            if (_levelData[_gridX][_gridY] != null)
            {
                _selectedItem = _levelData[_gridX][_gridY];
            }
        }
        else
        {
            if (CanPlaceItem(_selectedItem))
            {
                _levelData[_gridX][_gridY] = _selectedItem;
                _levelData[_gridX][_gridY].Pos = new Vector2(_snappedX, _snappedY);
                PlaceObjectOnGrid(_selectedItem);
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
                if (_levelData[x][y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void PlaceObjectOnGrid(Item item)
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

        item.transform.position = new Vector2(centerX, centerY);
        for (int x = leftX; x < leftX + width; x++)
        {
            for (int y = bottomY; y < bottomY + height; y++)
            {
                _levelData[x][y] = item;
            }
        }
        //_levelData[_gridX][_gridY] = null;
    }

    private void TakeObjectFromGrid(Item item)
    {

    }

    void OnDrawGizmos()
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

    void SetMouseGridPos()
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

            _debugTxt.text = _gridX + " " + _gridY + "      " + _snappedX + " " + _snappedY;
        }
    }
}
