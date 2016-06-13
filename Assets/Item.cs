using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    private float _width;
    private float _height;
    private int _id;
    //DEBUG
    private Vector2 _pos;

    public float Width { get { return _width; } }
    public float Height { get { return _height; } }
    public int ID { get { return _id; } set { _id = value; } }

    public Vector2 Pos { get { return _pos; } set { _pos = value; } }

    private void Start()
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
