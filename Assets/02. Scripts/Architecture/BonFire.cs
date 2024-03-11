using System;
using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private int _cookingLevel = 0;
    public string clipName = "BonFire";
    private SFXSound _sound;
    private UIIgnition _ignitionUI;
    private UICooking _cookingUI;
    private Ignition _ignition;

    public BuildableObject BuildableObject { get; private set; }

    private void Awake()
    {
        _ignition = GetComponent<Ignition>();
        var buildableObject = GetComponent<BuildableObject>();
        buildableObject.OnDestroyed += OnDestruct;

        BuildableObject = GetComponent<BuildableObject>();
        BuildableObject.OnRenamed += _ignition.Load;
    }

    private void Start()
    {
        _sound = Managers.Sound.PlayEffectSound(
            transform.position,
            clipName,
            0.7f,
            true);
    }

    public void Interact(Player player)
    {
        _ignitionUI = Managers.UI.GetPopupUI<UIIgnition>();
        _ignitionUI.ignition = _ignition;
        _cookingUI = Managers.UI.GetPopupUI<UICooking>();
        _cookingUI.ignition = _ignition;
        Managers.UI.ShowPopupUI<UIIgnition>();
    }

    public void OnDestruct()
    {
        _sound.StopSound();
    }

    public float GetInteractTime()
    {
        return 0f;
    }
}
