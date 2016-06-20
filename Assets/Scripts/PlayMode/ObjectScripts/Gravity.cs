using UnityEngine;
using System.Collections;
using System;

public class Gravity : ActiveObject
{
    [SerializeField]
    private FallingObject _fallingObject;
    private Rigidbody2D _targetRigidbody;

    public override void Play()
    {
        _fallingObject = PlayMode.Instance.FallingObject;
        _targetRigidbody = _fallingObject.GetComponent<Rigidbody2D>();
        _targetRigidbody.AddForce((transform.position - _fallingObject.transform.position) * 50);
    }

    public override void Stop()
    {

    }
}
