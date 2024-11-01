using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public ButtonConfig[] buttonConfigs;
    public LayerMask wallLayerMask; 
    public Camera mainCam; 

    public Yandere player;

    private void Start()
    {
        player = GameGlobals.instance.Player;
    }

    private void Update()
    {
        foreach (var button in buttonConfigs)
        {
            if (button.nearestPrompts.Count > 0)
            {
                var currentPrompt = GetNearestPrompt(button.nearestPrompts);
                button.currentPrompt = currentPrompt;

                if (mainCam != null)
                {
                    Vector2 screenPos = mainCam.WorldToScreenPoint(currentPrompt.AdjustedPosition);
                    button.promptParent.gameObject.SetActive(true);
                    button.promptParent.position = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 1));
                }

                button.label.text = currentPrompt.Text;

                if (Input.GetKey(button.keyCode)) 
                {
                    currentPrompt.Pressed = currentPrompt.RemainingTimer <= 0f
                        ? true
                        : (currentPrompt.RemainingTimer -= Time.deltaTime) == 0f;
                }
                else
                {
                    currentPrompt.Pressed = false;
                    currentPrompt.RemainingTimer = currentPrompt.FillTimer;
                }

                button.background.fillAmount = currentPrompt.RemainingTimer / currentPrompt.FillTimer;
            }
            else
            {
                button.promptParent.gameObject.SetActive(false);
                button.currentPrompt = null;
            }
        }
    }

    private PromptScript GetNearestPrompt(List<PromptScript> prompts)
    {
        PromptScript nearest = null;
        float minDist = Mathf.Infinity;
        Vector3 playerPos = player.transform.position;

        foreach (var prompt in prompts)
        {
            if (prompt)
            {
                float dist = Vector3.Distance(prompt.transform.position, playerPos);
                if (dist < minDist)
                {
                    nearest = prompt;
                    minDist = dist;
                }
            }
        }
        return nearest;
    }
}

[System.Serializable]
public class ButtonConfig
{
    public KeyCode keyCode;
    public Transform promptParent;
    public Image background;
    public TextMeshProUGUI label;

    [HideInInspector] public PromptScript currentPrompt;
    [HideInInspector] public List<PromptScript> nearestPrompts = new List<PromptScript>();
}