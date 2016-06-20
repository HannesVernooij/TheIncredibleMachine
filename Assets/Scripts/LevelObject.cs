using UnityEngine;
using System.Collections;

public class LevelObject
{
    private Vector2 _pos;
    private GridPosition _gridPos;
    private int _id;

    public Vector2 Pos { get { return _pos; } }
    public GridPosition GridPos { get { return _gridPos; } }
    public int ID { get { return _id; } }

    public LevelObject(Vector2 pos, GridPosition gridPos, int id)
    {
        _pos = pos;
        _gridPos = gridPos;
        _id = id;
    }
}
