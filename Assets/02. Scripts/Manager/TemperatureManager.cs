// �ۼ� ��¥ : 2024. 01. 10
// �ۼ��� : Park Jun Uk

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureManager
{
    // ���� �µ� { get; set; }
    public float Temperature { get; private set; } = 8.0f;

    // �ð��� ��������


    // �ڽ��� ���� ( ���� �߿�, �߿�, ����, ����, ���� } �ʿ��Ѱ�? 

    // �� ���� ���濡 ���� �̺�Ʈ
    // �߿�����. ����������. ��������. �������������.
    public event Action<float> OnChanged;

    public void Init(GameManager gameManager)
    {
        // �ð��� �ݹ�

        // ���� ���� �ݹ�
    }

    private void OnMonsterSpawned(float ratio)
    {

    }

    private void OnMorningCame()
    {
        // 2��?
    }

    private void OnEveningCame()
    {
        // 
    }

    private void OnNightCame()
    {
        // 
    }
}
