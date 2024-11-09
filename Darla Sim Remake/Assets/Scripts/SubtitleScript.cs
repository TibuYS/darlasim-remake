using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleScript : MonoBehaviour
{
    public GameObject speakerObject;
    private TMP_Text text; 
    public float maxSize = 52f;
    public float minSize = 20f;
    public float maxDistance = 5f; 

    private void Start()
    {
        text = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (speakerObject == null) return;
        float distance = Vector3.Distance(GameGlobals.instance.Player.transform.position, speakerObject.transform.position);
        if (distance < maxDistance)
        {
            float newSize = Mathf.Lerp(maxSize, minSize, distance / maxDistance);
            text.fontSize = Mathf.Clamp(newSize, minSize, maxSize);
            if (!text.gameObject.activeSelf)
            {
                text.gameObject.SetActive(true);
            }
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
}
