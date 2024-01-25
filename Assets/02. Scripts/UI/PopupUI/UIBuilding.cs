using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuilding : UIPopup
{
    enum GameObjects
    {
        CounterClockWise,
        ClockWise,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }
}
