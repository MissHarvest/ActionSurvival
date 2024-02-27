using System;
using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private int _cookingLevel = 0;
    private AudioSource _audioSource;
    private Ignition _ignition;
    private UIIgnition _ignitionUI;
    private UICooking _cookingUI;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _ignition = GetComponent<Ignition>();
        _audioSource.loop = true;
        var clip = Managers.Resource.GetCache<AudioClip>("BonFire.wav");
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void Interact(Player player)
    {
        _ignitionUI = Managers.UI.GetPopupUI<UIIgnition>();
        _ignitionUI.ignition = _ignition;

        if (!_ignitionUI.ignitionDic.ContainsKey(gameObject.name))
        {
            _ignitionUI.ignitionDic.Add(gameObject.name, _ignition);
        }
        
        _cookingUI = Managers.UI.GetPopupUI<UICooking>();
        _cookingUI.ignition = _ignition;
        Managers.UI.ShowPopupUI<UIIgnition>();
    }
}
