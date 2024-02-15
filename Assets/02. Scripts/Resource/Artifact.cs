using System.Collections;
using UnityEngine;

public class Artifact : MonoBehaviour, IInteractable
{
    private Island _island;
    private Vector3 _originPos;
    private Vector3 _activePos;
    private Coroutine _animateCoroutine;
    private bool _isActive;

    public bool IsActive => _isActive;
    public string IslandName => _island.Property.name;
    public Vector3 OriginPos => _originPos;

    public void SetInfo(Island island, bool active, Vector3 pos, Transform root)
    {
        _island = island;
        gameObject.tag = "Gather";
        transform.position = pos;
        _originPos = pos;
        _activePos = _originPos + Vector3.up;
        transform.parent = root;
        _isActive = active;
        SetManagementedObject();
    }

    public void SetActive(bool active)
    {
        if (_isActive == active)
            return;

        _isActive = active;

        SetState();
        SetInfluence();
        SetLayer();
    }

    public void SetLayer()
    {
        if (_isActive)
            gameObject.layer = LayerMask.NameToLayer("Resources");
        else
            gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void SetInfluence()
    {
        if (_isActive)
            _island.Influence += 0.1f;
        else
            _island.Influence -= 0.1f;
    }

    public void SetState()
    {
        if (_animateCoroutine != null)
            StopCoroutine(_animateCoroutine);

        if (_isActive)
            _animateCoroutine = StartCoroutine(Animate(transform.position, _activePos));
        else
            _animateCoroutine = StartCoroutine(Animate(transform.position, _originPos));
    }

    private void SetManagementedObject()
    {
        var manage = gameObject.GetOrAddComponent<ManagementedObject>();
        manage.Add(GetComponent<Collider>(), typeof(Collider));
        manage.Add(GetComponent<Renderer>(), typeof(Renderer));
    }

    public void Interact(Player player)
    {
        SetActive(false);
    }

    private IEnumerator Animate(Vector3 fromPos, Vector3 toPos)
    {
        for (float t = 0f; t < 0.5f; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(fromPos, toPos, t / 0.5f);
            yield return null;
        }
        transform.position = toPos;
        _animateCoroutine = null;
    }
}