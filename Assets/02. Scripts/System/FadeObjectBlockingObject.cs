using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _fadeAlpha = 0.33f;
    [SerializeField]
    private FadeMode _fadingMode;

    [SerializeField]
    private float _checkPerSecond = 10;
    [SerializeField]
    private int _fadeFPS = 30;

    [Header("Read Only Data")]
    [SerializeField]
    private List<FadingObject> _objectsBlockingView = new List<FadingObject>();
    private List<int> _indexesToClear = new List<int>();
    private Dictionary<FadingObject, Coroutine> _runningCorountines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] _hits = new RaycastHit[10];

    private void Start()
    {
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / _checkPerSecond);

        while (true)
        {
            int hits = Physics.RaycastNonAlloc(_camera.transform.position, (_player.transform.position - _camera.transform.position).normalized, _hits,
                Vector3.Distance(_camera.transform.position, _player.transform.position), _layerMask);
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(_hits[i]);
                    if (fadingObject != null && !_objectsBlockingView.Contains(fadingObject))
                    {
                        if (_runningCorountines.ContainsKey(fadingObject))
                        {
                            if (_runningCorountines[fadingObject] != null)
                            {
                                StopCoroutine(_runningCorountines[fadingObject]);
                            }

                            _runningCorountines.Remove(fadingObject);
                        }

                        _runningCorountines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        _objectsBlockingView.Add(fadingObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return Wait;
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        for (int i = 0; i < _objectsBlockingView.Count; i++)
        {
            bool objectIsBeingHit = false;
            for (int j = 0; j < _hits.Length; j++)
            {
                FadingObject fadingObject = GetFadingObjectFromHit(_hits[j]);
                if (fadingObject != null && fadingObject == _objectsBlockingView[i])
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if (!objectIsBeingHit)
            {
                if (_runningCorountines.ContainsKey(_objectsBlockingView[i]))
                {
                    if (_runningCorountines[_objectsBlockingView[i]] != null)
                    {
                        StopCoroutine(_runningCorountines[_objectsBlockingView[i]]);
                    }
                    _runningCorountines.Remove(_objectsBlockingView[i]);
                }

                _runningCorountines.Add(_objectsBlockingView[i], StartCoroutine(FadeObjectIn(_objectsBlockingView[i])));
                _objectsBlockingView.RemoveAt(i);
            }
        }
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        float waitTime = 1f / _fadeFPS;
        WaitForSeconds Wait = new WaitForSeconds(waitTime);
        int ticks = 1;

        for (int i = 0; i < fadingObject.Materials.Count; i++)
        {
            fadingObject.Materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fadingObject.Materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            fadingObject.Materials[i].SetInt("_ZWrite", 0);

            if (_fadingMode == FadeMode.Fade)
            {
                fadingObject.Materials[i].EnableKeyword("_ALPHABLEND_ON");
            }
            else
            {
                fadingObject.Materials[i].EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }

            fadingObject.Materials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        if (fadingObject.Materials[0].HasProperty("_Color"))
        {
            while (fadingObject.Materials[0].color.a > _fadeAlpha)
            {
                for (int i = 0; i < fadingObject.Materials.Count; i++)
                {
                    if (fadingObject.Materials[i].HasProperty("_Color"))
                    {
                        fadingObject.Materials[i].color = new Color(
                            fadingObject.Materials[i].color.r,
                            fadingObject.Materials[i].color.r,
                            fadingObject.Materials[i].color.r,
                            Mathf.Lerp(fadingObject.InitialAlpha, _fadeAlpha, waitTime * ticks)
                            );
                    }
                }

                ticks++;
                yield return Wait;
            }
        }

        if (_runningCorountines.ContainsKey(fadingObject))
        {
            StopCoroutine(_runningCorountines[fadingObject]);
            _runningCorountines.Remove(fadingObject);
        }
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float waitTime = 1f / _fadeFPS;
        WaitForSeconds Wait = new WaitForSeconds(waitTime);
        int ticks = 1;

        if (fadingObject.Materials[0].HasProperty("_Color"))
        {
            while (fadingObject.Materials[0].color.a < fadingObject.InitialAlpha)
            {
                for (int i = 0; i < fadingObject.Materials.Count; i++)
                {
                    if (fadingObject.Materials[i].HasProperty("_Color"))
                    {
                        fadingObject.Materials[i].color = new Color(
                            fadingObject.Materials[i].color.r,
                            fadingObject.Materials[i].color.r,
                            fadingObject.Materials[i].color.r,
                            Mathf.Lerp(_fadeAlpha, fadingObject.InitialAlpha, waitTime * ticks)
                            );
                    }
                }

                ticks++;
                yield return Wait;
            }
        }

        for (int i = 0; i < fadingObject.Materials.Count; i++ )
        {
            if (_fadingMode == FadeMode.Fade)
            {
                fadingObject.Materials[i].DisableKeyword("_ALPHABLEND_ON");
            }
            else
            {
                fadingObject.Materials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }

            fadingObject.Materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            fadingObject.Materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            fadingObject.Materials[i].SetInt("_ZWrite", 1);
            fadingObject.Materials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }

        if (_runningCorountines.ContainsKey(fadingObject))
        {
            StopCoroutine(_runningCorountines[fadingObject]);
            _runningCorountines.Remove(fadingObject);
        }
    }

    private FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<FadingObject>();
        }
        else
        {
            return null;
        }
    }

    private void ClearHits()
    {
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < _hits.Length; i++)
        {
            _hits[i] = hit;
        }
    }

    public enum FadeMode
    {
        Transparent,
        Fade
    }
}
