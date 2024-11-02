using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGlobals : MonoBehaviour
{
    public static GameGlobals instance;
    public Yandere Player;
    public PromptManager PromptManager;
    public Camera mainCamera;
    [Header("Developer Tools")]
    [Range(0, 100)] public float timeSpeed;

    void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(Time.timeScale != timeSpeed)
        {
            Time.timeScale = timeSpeed;
        }
    }

}
