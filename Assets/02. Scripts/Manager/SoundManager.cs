using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SoundManager
{
    private AudioSource _bgmSource;
    private GameObject _root;
    private AudioMixer _audioMixer;
    private Dictionary <string, float> _bgmContanier = new Dictionary<string, float>();
    private IObjectPool<SFXSound> _sfxSounds;
    private GameObject _sfxSoundPrefab;
    private Transform _sfxRoot;    

    public void Init()
    {
        _root = new GameObject("@Sound");
        _audioMixer = Managers.Resource.GetCache<AudioMixer>("AudioMixer.mixer");
        var bgms = Managers.Resource.GetCacheGroup<AudioClip>("BGM");        
        _bgmContanier.TryAdd("CenterIslandBGM", 0.2f);
        _bgmContanier.TryAdd("IceIslandBGM", 0.2f);
        _bgmContanier.TryAdd("FireIslandBGM", 0.2f);

        _sfxSounds = new ObjectPool<SFXSound>(CreateSFX, OnGetSFX, OnReleaseSFX, OnDestroySFX, maxSize: 20);

        CreateBgmPlayer();
        CreateSFXPlayers();
    }

    private void CreateBgmPlayer()
    {
        var bgmPlayer = new GameObject("@BGM");
        bgmPlayer.transform.parent = _root.transform;

        _bgmSource = bgmPlayer.AddComponent<AudioSource>();        
        _bgmSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        _bgmSource.loop = true;
    }

    private void CreateSFXPlayers()
    {
        _sfxRoot = new GameObject("@SFX").transform;
        _sfxRoot.parent = _root.transform;
        _sfxSoundPrefab = Managers.Resource.GetCache<GameObject>("SFXSound.prefab");
    }

    public void PlayBGM(string bgmName, float volume = 1.0f)
    {
        var bgmClip = Managers.Resource.GetCache<AudioClip>($"{bgmName}BGM.wav");
        _bgmSource.clip = bgmClip;
        _bgmSource.playOnAwake = true;
        _bgmSource.volume = volume;
        _bgmSource.Play();
    }

    public void PlayIslandBGM(string bgmName)
    {
        PlayBGM(bgmName, 0.2f);
    }

    public SFXSound PlayEffectSound(string sfxName, float volume = 0.3f, bool loop = false)
    {
        return PlayEffectSound(null, sfxName, volume, loop);
    }

    public SFXSound PlayEffectSound(Vector3? position, string sfxName, float volume, bool loop)
    {
        var sound = _sfxSounds.Get();
        var clip = Managers.Resource.GetCache<AudioClip>($"{sfxName}.wav");
        sound.PlaySound(position, clip, volume, loop);
        return sound;
    }

    public void Set(string group, float volume)
    {
        _audioMixer.SetFloat(group, volume);
        PlayerPrefs.SetFloat($"{group}Volume", volume);
    }

    #region Object Pooling
    private SFXSound CreateSFX()
    {
        SFXSound sfx = Object.Instantiate(_sfxSoundPrefab, _sfxRoot).GetComponent<SFXSound>();
        sfx.SetManagedPool(_sfxSounds);
        sfx.AudioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];
        return sfx;
    }

    private void OnGetSFX(SFXSound sfx)
    {
        sfx.gameObject.SetActive(true);
    }

    private void OnReleaseSFX(SFXSound sfx)
    {
        sfx.gameObject.SetActive(false);
    }

    private void OnDestroySFX(SFXSound sfx)
    {
        Object.Destroy(sfx.gameObject);
    }
    #endregion
}
