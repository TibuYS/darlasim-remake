using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class Subtitle
{
    public GameObject subtitleObject;
    public TMP_Text subtitleTextComponent;
    public string subtitleText;
    public float subtitleDuration;
    public Color subtitleColor;
    public GameObject subtitleSpeaker;
    public AudioClip subtitleAudio;
    public CanvasGroup subtitleFade;
    public int placementInList;
}
public class SubtitleManagerScript : MonoBehaviour
{
    [Header("Assign in inspector")]
    public GameObject subtitlePrefab;
    public Transform subtitleCanvas;
    [Space]
    [Header("Subtitle Global Settings")]
    [Tooltip("Gap between subtitles!")]public float yOffset = 55;
    [Tooltip("Lowest subtitle position")]public float lowestY = -500;
    [Tooltip("Subtitle fade speed")]public float fadeSpeed = 0.2f;
    [Tooltip("Subtitle lerp speed")]public float lerpSpeed = 0.5f;
    [Space]
    [Header("Runtime values")]
    public List<Subtitle> subtitles;
    public static SubtitleManagerScript instance;

    public void Start()
    {
        instance = this;
    }

    public void DisplaySubtitle(string text, float dur, Color color, GameObject speaker = null, AudioClip voice = null)
    {
        GameObject newSubtitle = Instantiate(subtitlePrefab, subtitleCanvas);
        Subtitle subtitleData = new Subtitle();
        subtitleData.subtitleText = text;
        subtitleData.subtitleDuration = dur;
        subtitleData.subtitleColor = color;
        subtitleData.subtitleSpeaker = speaker;
        subtitleData.subtitleAudio = voice;
        subtitleData.subtitleTextComponent = newSubtitle.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        subtitleData.subtitleFade = newSubtitle.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        subtitleData.subtitleObject = newSubtitle;
        StartCoroutine(subtitleDisplay(subtitleData));
    }

    IEnumerator subtitleDisplay(Subtitle subtitleData)
    {
        subtitleData.subtitleTextComponent.text = subtitleData.subtitleText;
        subtitleData.subtitleTextComponent.color = subtitleData.subtitleColor;
        subtitles.Add(subtitleData);
        subtitleData.placementInList = subtitles.IndexOf(subtitleData);
        if (subtitles.Count == 1)
        {
            //first subtitle, simple fade in
            subtitleData.subtitleObject.transform.localPosition = new Vector3(subtitleData.subtitleObject.transform.localPosition.x, lowestY, subtitleData.subtitleObject.transform.localPosition.z);
            subtitleData.subtitleFade.DOFade(1, fadeSpeed);
        }
        else
        {
            //raise above first subtitle
            subtitleData.subtitleFade.DOFade(1, fadeSpeed);
            subtitleData.subtitleObject.transform.DOLocalMove(new Vector3(subtitleData.subtitleObject.transform.localPosition.x, lowestY + (subtitles.Count - 1) * yOffset, subtitleData.subtitleObject.transform.localPosition.z), lerpSpeed);
        }
        yield return new WaitForSeconds(subtitleData.subtitleDuration);
        subtitleData.subtitleFade.DOFade(0, fadeSpeed);
        subtitleData.subtitleObject.transform.DOLocalMoveY(-1000, lerpSpeed).OnComplete(() => { AdjustSubtitles(subtitles.IndexOf(subtitleData), subtitleData); });

    }

    public void AdjustSubtitles(int removedIndex, Subtitle subtData)
    {
        // Destroy the removed subtitle's object and remove it from the list
        Destroy(subtData.subtitleObject);
        subtitles.RemoveAt(removedIndex);

        // Iterate through the updated list to reposition each subtitle object
        for (int i = 0; i < subtitles.Count; i++)
        {
            Subtitle subtitle = subtitles[i];

            // Calculate the target Y position based on the subtitle's index
            float targetY = lowestY + i * yOffset;

            // Move the subtitle to its new Y position
            subtitle.subtitleObject.transform.DOLocalMoveY(targetY, lerpSpeed);
        }
    }

}
