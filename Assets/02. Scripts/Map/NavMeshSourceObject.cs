using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 2024-02-07 WJY
public class NavMeshSourceObject : MonoBehaviour
{
    private NavMeshBuildSource[] _buildSources;

    [System.Serializable]
    private struct SeializableNavMeshBuildSource
    {
        public Object sourceObject;
        public NavMeshBuildSourceShape shape;
        public Transform transform;
        public Component component;
        public Vector3 size;
    }

    [SerializeField] private SeializableNavMeshBuildSource[] _sources;

    public void ConvertData()
    {
        _buildSources = new NavMeshBuildSource[_sources.Length];
        for (int i = 0; i < _sources.Length; i++)
        {
            _buildSources[i] = new()
            {
                shape = _sources[i].shape,
                sourceObject = _sources[i].sourceObject,
                transform = _sources[i].transform.localToWorldMatrix,
                component = _sources[i].component,
                size = _sources[i].size,
            };
        }
    }

    private void Start()
    {
        SendToBuilder();
    }

    private void SendToBuilder()
    {
        if (_buildSources == null)
            ConvertData();

        foreach (var source in _buildSources)
            Managers.Game.WorldNavMeshBuilder.AddSources(source);
    }

    public NavMeshBuildSource[] GetSources()
    {
        if (_buildSources == null)
            ConvertData();

        return _buildSources;
    }
}