using System.Collections.Generic;
using UnityEngine;

// 2024-03-07 WJY
public class OutlineManager
{
    private Dictionary<UnityEngine.Object, OutlineDrawer> _outlineDict;
    private OutlineDrawer _currentHighlight;

    public void Init()
    {
        _outlineDict = new();
    }

    public void RegistOutlineDrawer(UnityEngine.Object key, OutlineDrawer drawer)
    {
        _outlineDict.TryAdd(key, drawer);
    }

    public void ReleaseOutlineDrawer(UnityEngine.Object key)
    {
        _outlineDict.Remove(key);
    }

    public bool TryHighlightObject(UnityEngine.Object key)
    {
        var drawer = FindOutlineDrawer(key);
        bool res = false;

        if (drawer != _currentHighlight)
        {
            if (_currentHighlight != null)
                _currentHighlight.SetActive(false);

            if (drawer != null)
            {
                drawer.SetActive(true);
                res = true;
            }
        }

        _currentHighlight = drawer;
        return res;
    }

    public OutlineDrawer FindOutlineDrawer(UnityEngine.Object key)
    {
        if (key == null)
            return null;

        _outlineDict.TryGetValue(key, out var drawer);
        return drawer;
    }
}