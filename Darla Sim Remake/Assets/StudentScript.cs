using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

enum AttackType
{
    Front,
    Back
}

public enum Action
{
    Idle,
    Walk,
    Run,
    Sprint,
    Finished,
    ChangingShoes
}

public class StudentScript : MonoBehaviour
{
    [Header("Components")]
    public Animation studentAnimation;
    public NavMeshAgent studentAgent;
    public PromptScript studentPrompt;
    public StudentDataScriptable studentData;

    private UnityEvent talkEvent;
    private UnityEvent killEvent;
    
    void Start()
    {
        ResetData(); //until we have more students
        BecomeRagdoll(false); //so she doesnt fall lol
        talkEvent = new UnityEvent();
        talkEvent.AddListener(Talk);
        killEvent = new UnityEvent();
        talkEvent.AddListener(Kill);
        UpdateStudentPrompt("Talk", KeyCode.E, talkEvent);
    }

    public void ResetData()
    {
        studentData.isDead = false;
    }

   public void UpdateStudentPrompt(string newPromptText, KeyCode newPromptKey, UnityEvent newPromptAction)
    {
        studentPrompt.Text = newPromptText;
        studentPrompt.keyCode = newPromptKey;
        studentPrompt.OnPressed = newPromptAction;
    }

    public void Talk()
    {
        Debug.Log("Talk");
    }

    public void Kill()
    {
        Vector3 playerDirection = (GameGlobals.instance.Player.transform.position - transform.position).normalized;
        Vector3 characterForward = transform.forward;
        float dotProduct = Vector3.Dot(playerDirection, characterForward);
        float threshold = 0.0f;
        if (dotProduct > threshold) //direct attack
        {
            StartCoroutine(KillRoutine(AttackType.Front));
        }
        else //stealth
        {
            StartCoroutine(KillRoutine(AttackType.Back));
        }
    }

    public void BecomeRagdoll(bool becomeRagdoll)
    {
        studentAnimation.enabled = !becomeRagdoll;
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = !becomeRagdoll;
        }
        foreach (var col in colliders)
        {
            if (col.gameObject.name != gameObject.name)
            {
                col.enabled = becomeRagdoll;
            }
        }
        int objectLayer = gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (i != objectLayer)
            {
                Physics.IgnoreLayerCollision(objectLayer, i, !becomeRagdoll);
            }
        }
    }

    string attackAnimation(AttackType attackType, Weapon weaponType)
    {
        string animation = "";
        if (weaponType == Weapon.Knife)
        {
            switch (attackType)
            {
                case AttackType.Back:
                    animation = studentData.studentGender == Gender.Female ? "f02_knifeHighSanityB_00" : "knifeHighSanityB_00";
                    return animation;

                case AttackType.Front:
                    animation = studentData.studentGender == Gender.Female ? "f02_knifeStealthB_00" : "knifeStealthB_00";
                    return animation;
            }
        }


        return animation;
    }

    IEnumerator KillRoutine(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Front:

                break;

            case AttackType.Back:

                break;
        }

        yield return null;
    }
}
