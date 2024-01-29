using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    private AudioSource _bgmSource;
    private GameObject _root;
    private AudioMixer _audioMixer;
    private Stack<SFXSound> _inactivated = new Stack<SFXSound>();

    public void Init()
    {
        _root = new GameObject("@Sound");
        _audioMixer = Managers.Resource.GetCache<AudioMixer>("AudioMixer.mixer");
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
        _bgmSource.volume = 1.0f;
    }

    private void CreateSFXPlayers()
    {
        var soundBox = new GameObject("@SFX");
        soundBox.transform.parent = _root.transform;

        for(int i = 0; i < 30; ++i)
        {
            var soundPrefab = Managers.Resource.GetCache<GameObject>("SFXSound.prefab");
            var go = Object.Instantiate(soundPrefab, soundBox.transform); // [ true / false ]
            go.SetActive(false);
            var sfxsound = go.GetComponent<SFXSound>();
            sfxsound.AudioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];
            sfxsound.OnPlayEnd += PushInActivatedObject;
            _inactivated.Push(sfxsound);
        }
    }

    public void PlayBGM(string bgmName)
    {
        var bgmClip = Managers.Resource.GetCache<AudioClip>($"{bgmName}.wav");
        _bgmSource.clip = bgmClip;
        _bgmSource.playOnAwake = true;
        _bgmSource.Play();
    }

    public void PlayEffectSound(Vector3 position, string effectName, float volume = 0.3f)
    {
        if (_inactivated.Count <= 0) return;
        var sound = _inactivated.Pop();
        sound.AudioSource.volume = volume;
        sound.transform.position = position;
        var clip = Managers.Resource.GetCache<AudioClip>($"{effectName}.wav");
        sound.Play(clip);
    }

    private void PushInActivatedObject(SFXSound sfx)
    {
        sfx.gameObject.SetActive(false);
        _inactivated.Push(sfx);
    }

    public void Set(string group, float volume)
    {
        _audioMixer.SetFloat(group, volume);
        PlayerPrefs.SetFloat($"{group}Volume", volume);
    }
}
