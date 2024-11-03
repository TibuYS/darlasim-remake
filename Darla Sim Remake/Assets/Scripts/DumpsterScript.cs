using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class DumpsterScript : MonoBehaviour
{
    [Header("Set in inspector")]
    public Transform dumpsterLid;
    public Vector3 openRotation;
    public Vector3 openPosition;
    public Vector3 closedRotation;
    public Vector3 closedPosition;
    [Header("Components")]
    public PromptScript dumpsterPrompt;
    [Space]
    [Header("Runtime values")]
    public List<StudentScript> bodies = new List<StudentScript>();
    public bool isOpen = false;
    private UnityEvent interactEvent;
    private UnityEvent dumpEvent;

    private void Start()
    {
        interactEvent = new UnityEvent();
        interactEvent.AddListener(Interact);
        dumpEvent = new UnityEvent();
        dumpEvent.AddListener(DumpBody);
    }
    private void Update()
    {
        if(dumpsterPrompt.Nearby)
        {
            if(GameGlobals.instance.Player.currentCorpse != null)
            {
                if (isOpen && dumpsterPrompt.Text != "Dump")
                {
                    dumpsterPrompt.Text = "Dump";
                    dumpsterPrompt.OnPressed = dumpEvent;
                }
            }
            else
            {
                if(dumpsterPrompt.Text == "Dump")
                {
                    dumpsterPrompt.Text = isOpen ? "Close" : "Open";
                    dumpsterPrompt.OnPressed = interactEvent;
                }
            }
           


        }
    }

    public void DumpBody()
    {

    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            dumpsterLid.DOLocalRotate(openRotation, 0.3f);
            dumpsterLid.DOLocalMove(openPosition, 0.3f);
            dumpsterPrompt.Text = "Close";
        }
        else
        {
            dumpsterLid.DOLocalRotate(closedRotation, 0.3f);
            dumpsterLid.DOLocalMove(closedPosition, 0.3f);
            dumpsterPrompt.Text = "Open";
        }
    }

  
}
