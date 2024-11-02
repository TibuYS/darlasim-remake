using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineObjectScript : MonoBehaviour
{
    [Header("UI")]
    public ExamineUIScript examineWindow;
    [Header("Object Properties")]
    public string objectName;
    public string objectDescription;


    public void Examine()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.getClip("Select"), 1, false, SoundManager.AudioGroup.SFX);
        examineWindow.ShowWindow(objectName, objectDescription);
    }
}
