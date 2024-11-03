using UnityEngine;
using System.Collections.Generic;

public class ScaleBugFixScript : MonoBehaviour
{
    private struct InitialScaleData
    {
        public Transform transform;
        public Vector3 originalScale;

        public InitialScaleData(Transform transform, Vector3 initialScale)
        {
            this.transform = transform;
            this.originalScale = initialScale;
        }
    }

    private List<InitialScaleData> originalScales;

    void Start()
    {
        originalScales = new List<InitialScaleData>();
        SaveInitialScale(transform);
        foreach (Transform child in transform)
        {
            SaveInitialScale(child);
        }
    }

    private void SaveInitialScale(Transform objTransform)
    {
        originalScales.Add(new InitialScaleData(objTransform, objTransform.localScale));
    }

    public void ResetScales()
    {
        foreach (var data in originalScales)
        {
            data.transform.localScale = data.originalScale;
        }
    }
}
