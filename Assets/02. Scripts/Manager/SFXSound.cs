using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

// 2024. 01. 25 Park Jun Uk
public class SFXSound : MonoBehaviour
{
    public AudioSource AudioSource { get; private set; }
    private IObjectPool<SFXSound> _managedPool;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void SetManagedPool(IObjectPool<SFXSound> managedPool)
    {
        _managedPool = managedPool;
    }

    public void PlaySound(Vector3? position, AudioClip clip, float volume, bool loop)
    {
        AudioSource.spatialBlend = position == null ? 0.0f : 1.0f;
        if (position != null) transform.position = position.Value;        
        AudioSource.volume = volume;
        AudioSource.loop = loop;
        if(loop)
        {
            AudioSource.clip = clip;
            AudioSource.Play();           
        }
        else
        {
            AudioSource.PlayOneShot(clip);
            var length = clip.length;
            StartCoroutine(ReturnToManager(length));
        }
    }

    IEnumerator ReturnToManager(float time)
    {
        yield return new WaitForSeconds(time);
        _managedPool.Release(this);
    }
}
