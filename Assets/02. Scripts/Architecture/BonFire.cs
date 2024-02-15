using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private int _cookingLevel = 0;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        var clip = Managers.Resource.GetCache<AudioClip>("BonFire.wav");
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void Interact(Player player)
    {
        var ui = Managers.UI.ShowPopupUI<UICooking>();
        ui.SetAdvancedRecipeUIActive(_cookingLevel);
    }
}
