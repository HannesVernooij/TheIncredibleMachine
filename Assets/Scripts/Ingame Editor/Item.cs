using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    private float _width;
    private float _height;
    private int _id;
    private GridPosition _posOnGrid;
    //DEBUG
    private Vector2 _pos;

    public float Width { get { return _width; } }
    public float Height { get { return _height; } }
    public int ID { get { return _id; } set { _id = value; } }

    public Vector2 Pos { get { return _pos; } set { _pos = value; } }
    public GridPosition PosOnGrid { get { return _posOnGrid; } set { _posOnGrid = value; } }

    private void Awake()
    {
        Vector2 size = GetComponent<SpriteRenderer>().bounds.size;
        _width = size.x;
        _height = size.y;
    }

    public void Execute()
    {
        // actions, physics etc.
    }
}
