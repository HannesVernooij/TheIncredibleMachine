using UnityEngine;
using System.Collections;
[System.Serializable]
public class FallingObject : ActiveObject
{
    private Vector2 _pos;

    private Rigidbody2D _rigidbody;
    private CircleCollider2D _collider;

    private void Awake()
    {
        PlayMode.Instance.SetFallingObject(this);
        _rigidbody = gameObject.AddComponent<Rigidbody2D>();
        _collider = gameObject.AddComponent<CircleCollider2D>();
        _rigidbody.isKinematic = true;
    }

    public override void Play()
    {
        _pos = transform.position;
        _rigidbody.isKinematic = false;
    }

    public override void Stop()
    {
        _rigidbody.isKinematic = true;
        transform.position = _pos;
    }
}
