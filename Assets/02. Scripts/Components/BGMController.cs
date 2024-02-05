using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public string targetBGM;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().StandingIslandName = targetBGM;
            Managers.Sound.PlayIslandBGM(targetBGM);
        }        
    }
}
