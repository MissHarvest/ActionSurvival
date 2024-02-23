using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private int _cookingLevel = 0;
    private AudioSource _audioSource;
    private Ignition _ignition;

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
        //var ui = Managers.UI.ShowPopupUI<UICooking>();
        //ui.SetAdvancedRecipeUIActive(_cookingLevel);
        GameManager.Instance.Player.Ignition = _ignition;
        Managers.UI.ShowPopupUI<UIIgnition>();
    }
}
