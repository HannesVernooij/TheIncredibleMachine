using UnityEngine;

public class Target : ActiveObject
{
    private ParticleSystem _ps;
    private CircleCollider2D _collider;

    private void Awake()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
        _collider = gameObject.AddComponent<CircleCollider2D>();
        _collider.isTrigger = true;
    }

    public override void Play()
    {
    }

    public override void Stop()
    {
    }

    public void OnTriggerEnter2D()
    {
        _ps.Play();
        PlayMode.Instance.LevelComplete();
    }
}
