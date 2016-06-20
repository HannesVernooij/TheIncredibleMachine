using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour
{
    private Inventory _inventory;
    private LevelObject[] _objects;

    public Inventory Inventory { get { return _inventory; } }
    public LevelObject[] LevelObjects { get { return _objects; } }

    public Level(Inventory i, LevelObject[] lo)
    {
        _inventory = i;
        _objects = lo;
    }
}
