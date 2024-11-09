using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGlobals : MonoBehaviour
{
    public static GameGlobals instance;
    public Yandere Player;
    public PromptManager PromptManager;
    public Camera mainCamera;
    public TimeManager TimeManager;

    void Awake()
    {
        instance = this;
    }

    public void Smooth_ChangeTimeSpeed(float target, float dur)
    {
        StartCoroutine(ChangeTimeSpeed(target, dur));
    }

    public GameObject FindMainParent(GameObject obj)
    {
        Transform currentParent = obj.transform;
        while (currentParent.parent != null)
        {
            currentParent = currentParent.parent;
        }
        return currentParent.gameObject;
    }


    public IEnumerator ChangeTimeSpeed(float targetTimeSpeed, float duration)
    {
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;
        SoundManager.instance.GatherCurrentlyPlaying();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeSpeed, progress);
            SoundManager.instance.ChangeAllPitch(Time.timeScale);
            yield return null;
        }
        Time.timeScale = targetTimeSpeed;
    }

}
