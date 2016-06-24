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
        Debug.Log("SET: " + Time.time);
        PlayMode.Instance.SetFallingObject(this);
        //Debug.Log(_rigidbody != null);
        Debug.Log(gameObject.GetInstanceID());
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody2D>();
        }
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
        if (this == null)
        {
            return;
        }
        Debug.Log(gameObject.GetInstanceID());
        Debug.Log("KINEMATIC: " + Time.time);
        _rigidbody.isKinematic = true;
        transform.position = _pos;
    }
}
