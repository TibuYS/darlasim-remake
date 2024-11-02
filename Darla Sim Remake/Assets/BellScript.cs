using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellScript : MonoBehaviour
{
    void Start()
    {
        if(SoundManager.instance.getClip("Schoolbell") == null)
        {
            Debug.LogError("assign bell sound!");
        }
        StartCoroutine(bellRing());
    }

    public IEnumerator bellRing()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.getClip("Ambiences"), 1, true, SoundManager.AudioGroup.SFX);
        SoundManager.instance.PlaySound(SoundManager.instance.getClip("Schoolbell"), 1, false, SoundManager.AudioGroup.SFX);
        yield return new WaitForSeconds(SoundManager.instance.getClip("Schoolbell").length);
        SoundManager.instance.PlaySound(SoundManager.instance.getClip("TemporarySchooldayTrack2"), 1, true, SoundManager.AudioGroup.Music);
        Destroy(this);
    }
}
