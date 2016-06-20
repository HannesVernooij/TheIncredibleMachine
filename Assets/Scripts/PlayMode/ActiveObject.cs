using UnityEngine;

public abstract class ActiveObject : MonoBehaviour
{
    private void Start()
    {
        PlayMode.Instance.PlayEvent += Play;
        PlayMode.Instance.StopEvent += Stop;
    }
    public abstract void Play();
    public abstract void Stop();
    private void OnDestroy()
    {
        PlayMode.Instance.PlayEvent -= Play;
        PlayMode.Instance.PlayEvent -= Stop;
    }
}
