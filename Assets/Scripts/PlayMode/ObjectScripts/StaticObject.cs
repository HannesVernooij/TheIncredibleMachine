using UnityEngine;
using System.Collections;

public class StaticObject : ActiveObject
{
    private CircleCollider2D _collider;
    private void Start()
    {
        _collider = gameObject.AddComponent<CircleCollider2D>();
        _collider.sharedMaterial = Resources.Load("Bounce") as PhysicsMaterial2D;
    }

    public override void Play()
    {

    }

    public override void Stop()
    {

    }
}
