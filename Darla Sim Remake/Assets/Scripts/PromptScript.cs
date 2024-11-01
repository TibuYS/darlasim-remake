using UnityEngine;
using UnityEngine.Events;

public class PromptScript : MonoBehaviour
{
    [Header("Edit in inspector :)")]
    public UnityEvent OnPressed;
    public string Text;
    public Transform Pivot;
    public KeyCode keyCode;
    [Range(0, 2)] public float FillTimer = 0.5f;
    [Range(0.5f, 5)] public float ActivationDistance = 1;
    [Range(0, 1)] public float OffsetX = 0;
    [Range(0, 1)] public float OffsetY = 0;
    public bool IgnoreWalk;

    [Space]
    [Range(0, 2)] public float RemainingTimer;
    public Vector3 AdjustedPosition;
    public bool CanBePressed;
    public bool Nearby;
    public bool Pressed;

    private Yandere player;
    private PromptManager manager;

    private void Start()
    {
        if (!Pivot) Pivot = transform;
        player = GameGlobals.instance.Player;
        manager = GameGlobals.instance.PromptManager;
        UpdateData();
    }

    private void OnDisable()
    {
        ResetPromptState();
    }

    private void ResetPromptState()
    {
        Pressed = false;
        Nearby = false;
        RemainingTimer = FillTimer;

        foreach (var button in manager.buttonConfigs)
        {
            if (button.nearestPrompts.Contains(this))
            {
                button.nearestPrompts.Remove(this);
            }
        }
    }

    public void UpdateData()
    {
        ResetPromptState();

        if (!Pivot) Pivot = transform;
    }

    private void Update()
    {
        AdjustedPosition = Pivot.position + new Vector3(OffsetX, OffsetY, 0);
        float distSquared = (Pivot.position - player.transform.position).sqrMagnitude;

        CanBePressed = player.canMove || IgnoreWalk;
        Nearby = distSquared <= ActivationDistance * ActivationDistance &&
                  !Physics.Linecast(AdjustedPosition, player.Hips.position, manager.wallLayerMask);

        if (Nearby && CanBePressed)
        {
            if (!manager.buttonConfigs[GetButtonIndex()].nearestPrompts.Contains(this))
                manager.buttonConfigs[GetButtonIndex()].nearestPrompts.Add(this);
        }
        else
        {
            Pressed = false;
            if (manager.buttonConfigs[GetButtonIndex()].nearestPrompts.Contains(this))
                manager.buttonConfigs[GetButtonIndex()].nearestPrompts.Remove(this);
        }

        if (Pressed)
        {
            OnPressed.Invoke();
            StopBeingPressed();
        }

        if (Input.GetKey(keyCode)) // Check for the KeyCode directly
        {
            Pressed = RemainingTimer <= 0f;
            RemainingTimer -= Time.deltaTime;
        }
        else
        {
            Pressed = false;
            RemainingTimer = FillTimer;
        }
    }

    private int GetButtonIndex() // Helper function to get the index from the manager
    {
        for (int i = 0; i < manager.buttonConfigs.Length; i++)
        {
            if (manager.buttonConfigs[i].keyCode == keyCode)
            {
                return i;
            }
        }
        return -1; // Not found
    }

    public void StopBeingPressed()
    {
        Pressed = false;
        RemainingTimer = FillTimer;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 position = Pivot ? Pivot.position : transform.position;
        Gizmos.DrawWireSphere(position + new Vector3(OffsetX, OffsetY, 0), ActivationDistance);
    }
#endif
}
