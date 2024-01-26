using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
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
        Managers.Game.Player.Cooking.OnCookingShowAndHide();
        //Debug.Log("모닥불에서 요리 판넬 띄우기");
    }
}
