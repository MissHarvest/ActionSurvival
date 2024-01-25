using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 25 Park Jun Uk
public class SFXSound : MonoBehaviour
{
    public AudioSource AudioSource { get; private set; }
    public event Action<SFXSound> OnPlayEnd;
    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        gameObject.SetActive(true);
        AudioSource.PlayOneShot(clip);
        float length = clip.length;
        StartCoroutine(ReturnToManager(length));
    }

    IEnumerator ReturnToManager(float time)
    {
        yield return new WaitForSeconds(time);
        OnPlayEnd?.Invoke(this);
    }
}
