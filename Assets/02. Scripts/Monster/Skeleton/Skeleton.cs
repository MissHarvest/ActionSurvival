// �ۼ� ��¥ : 2024. 01. 12
// �ۼ��� : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Monster
{
    public Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent.SetDestination(destination.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
