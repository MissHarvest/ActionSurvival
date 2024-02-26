using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 25 Park Jun Uk
public class PlayerSoundController : MonoBehaviour
{
    public void PlayMineSFX()
    {
        Managers.Sound.PlayEffectSound("Mine", 1.0f, false);
    }

    public void PlaySFX(string name)
    {
        Managers.Sound.PlayEffectSound(name, 1.0f, false);
    }
}
