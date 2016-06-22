using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private Text _txt;
    private Color _color;
    private float _fadeValue;
    private bool _targetFade;

    private void Start()
    {
        _color = _txt.color;
        _txt.gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        PrepareFade(true);
    }

    public void FadeOut()
    {
        PrepareFade(false);
    }

    public void FadeInAndOut()
    {
        PrepareFade(true, true);
    }

    private void PrepareFade(bool value, bool fadeInAndOut = false)
    {
        _fadeValue = 0;
        _targetFade = value;
        Fading(fadeInAndOut);
        _txt.gameObject.SetActive(true);
    }

    private void Fading(bool fadeTwice = false)
    {
        if (_fadeValue < 1) _fadeValue += Time.deltaTime;
        else _fadeValue = 1;

        _txt.color = new Color(_color.r, _color.g, _color.b, _targetFade?_fadeValue:1-_fadeValue);

        if (_fadeValue < 1) StartCoroutine(DelayFade(fadeTwice));
        else if (fadeTwice == true)
        {
            _fadeValue = 0;
            _targetFade = !_targetFade;
            Fading(false);
        }
        else _txt.gameObject.SetActive(false);
    }

    private IEnumerator DelayFade(bool fadeTwice)
    {
        yield return new WaitForEndOfFrame();
        Fading(fadeTwice);
    }
}
