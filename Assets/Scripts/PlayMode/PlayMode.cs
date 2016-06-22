using UnityEngine;
using System.Collections;

public class PlayMode : Singleton<PlayMode>
{
    [SerializeField]
    private ItemSelection _itemSelection;
    [SerializeField]
    private LoadLevels _levelLoader;
    private int _currentLevel = -1;
    [SerializeField]
    private bool _playMode = false;
    [SerializeField]
    private Fade _fadeScript;

    //play & stop for objects
    private bool _active = false;
    [SerializeField]
    private FallingObject _fallingObject;
    public FallingObject FallingObject { get { return _fallingObject; } }
    public delegate void VoidDelegate();
    public event VoidDelegate PlayEvent;
    public event VoidDelegate StopEvent;

    private void Start()
    {
        if (_playMode)
        {
            NextLevel();
        }
        else _itemSelection.FillInventory();
    }

    private void NextLevel()
    {
        _currentLevel++;
        _fallingObject = null;
        _itemSelection.PrepareLevel(_levelLoader.GetLevel(_currentLevel));
    }

    public void Play()
    {
        if (_active == false)
        {
            _active = true;
            if (PlayEvent != null)
            {
                PlayEvent();
            }
        }
    }

    public void Stop()
    {
        if (_active == true)
        {
            _active = false;
            if (StopEvent != null)
            {
                StopEvent();
            }
        }
    }

    public void LevelComplete()
    {
        _fadeScript.FadeInAndOut();
        StartCoroutine(DelayNextLevel(3));
    }

    public void SetFallingObject(FallingObject o)
    {
        if (_fallingObject == null) _fallingObject = o;
        else Destroy(o.gameObject);
    }

    private IEnumerator DelayNextLevel(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        NextLevel();
    }
}
