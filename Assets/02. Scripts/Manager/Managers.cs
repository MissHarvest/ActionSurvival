using System;
using UnityEngine;

public class Managers : SingletonBehavior<Managers>
{
    #region Manager Variables

    private readonly ResourceManager _resourceManager = new();
    private readonly UIManager _uiManager = new();
    private readonly SoundManager _soundManager = new();
    private readonly DataManager _dataManager = new();

    #endregion



    #region Manager Properties

    public static ResourceManager Resource => Instance._resourceManager;
    public static UIManager UI => Instance._uiManager;    
    public static SoundManager Sound => Instance._soundManager;
    public static DataManager Data => Instance._dataManager;
    #endregion
}
