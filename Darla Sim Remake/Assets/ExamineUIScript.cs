using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ExamineUIScript : MonoBehaviour
{
    public TMP_Text examinedObjectName;
    public TMP_Text examinedObjectDescription;
    public GameObject examineWindowPrefab;

    public void ShowWindow(string objectName, string objectDescription)
    {
        StopAllCoroutines();
        DOTween.Kill(examineWindowPrefab);
        examineWindowPrefab.transform.localScale = Vector3.zero;
        examinedObjectName.text = objectName;
        examinedObjectDescription.text = objectDescription;
        examineWindowPrefab.transform.DOScale(Vector3.one, 0.3f).OnComplete(() => {StartCoroutine(Hide());}); ;
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(5);
        examineWindowPrefab.transform.DOScale(Vector3.zero, 0.3f);
    }
}
