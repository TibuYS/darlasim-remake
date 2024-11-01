using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGlobals : MonoBehaviour
{
    public Yandere Player;
    public PromptManager PromptManager;
    public Camera mainCamera;
    public static GameGlobals instance;
    [Header("Developer Tools")]
    [Range(0, 100)] public float timeSpeed;

    void Start()
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
