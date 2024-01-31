using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public string targetBGM;

    private void OnTriggerEnter(Collider other)
    {
        Managers.Sound.ChangeIslandBGM(targetBGM);        
    }
}
