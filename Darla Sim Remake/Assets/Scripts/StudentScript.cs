using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;

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
        killEvent.AddListener(Kill);
    }

    private void Update()
    {
        if (studentData.isDead) return;
        if (GameGlobals.instance.Player.holdingWeapon)
        {
            UpdateStudentPrompt("Attack", KeyCode.F, killEvent);
        }
        else
        {
            UpdateStudentPrompt("Talk", KeyCode.E, talkEvent);
        }
    }

    #region function to reset student's scriptable data
    public void ResetData()
    {
        studentData.isDead = false;
    }
    #endregion

    #region prompt related changes
    public void UpdateStudentPrompt(string newPromptText, KeyCode newPromptKey, UnityEvent newPromptAction)
    {
        if(studentPrompt.Text != newPromptText)
        {
            studentPrompt.Text = newPromptText;
        }
        if(studentPrompt.keyCode != newPromptKey)
        {
            studentPrompt.keyCode = newPromptKey;
        }
        if(studentPrompt.OnPressed != newPromptAction)
        {
            studentPrompt.OnPressed = newPromptAction;
        }
    }

    public void ShowPrompt(bool showOrNot)
    {
        studentPrompt.enabled = showOrNot;
    }
    #endregion

    #region  begin conversation function
    public void Talk()
    {
        Debug.Log("Talk");
    }
    #endregion

    #region everything related to attacking and killing
    public void Kill()
    {
        if (GameGlobals.instance.Player.isKilling) return; //alr in progress or alr in a killing state with another student ??
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
                    animation = studentData.studentGender == Gender.Female ? "f02_knifeStealthB_00" : "knifeStealthB_00";
                    return animation;

                case AttackType.Front:
                    animation = studentData.studentGender == Gender.Female ? "f02_knifeHighSanityB_00" : "knifeHighSanityB_00";
                    return animation;
            }
        }


        return animation;
    }

    public void FinishKill()
    {
        Debug.Log("kill finish");
        StartCoroutine(GameGlobals.instance.ChangeTimeSpeed(1f, 0.5f));
        // SlowMotionScript.instance.SlowMotionEffect(false);
        GameGlobals.instance.Player.enabled = true;
        GameGlobals.instance.Player.currentItem.enabled = true;
        studentData.isDead = true;
        transform.parent = null;
        BecomeRagdoll(true);
        ShowPrompt(true);
        UpdateStudentPrompt("Drag", KeyCode.Q, null);
    }

    IEnumerator KillRoutine(AttackType attackType)
    {
        GameGlobals.instance.Player.enabled = false; //stop player
        GameGlobals.instance.Player.currentItem.enabled = false; //prevents dropping mid-attack
        ShowPrompt(false);
        transform.parent = GameGlobals.instance.Player.transform;
        string studentAttackAnim = "";
        switch (attackType)
        {
            case AttackType.Front:
                transform.localPosition = new Vector3(0, 0, 1.004f);
                transform.LookAt(GameGlobals.instance.Player.transform);
                studentAttackAnim = attackAnimation(attackType, GameGlobals.instance.Player.currentItem.weaponType);
                studentAnimation.Play(studentAttackAnim);
                GameGlobals.instance.Player.playerAnimationComponent.Play("f02_knifeHighSanityA_00");
                SoundManager.instance.PlaySound(SoundManager.instance.getClip("KnifeHighSanity"), 1, false, SoundManager.AudioGroup.SFX);
                SoundManager.instance.PlaySound(studentData.studentGender == Gender.Female ? SoundManager.instance.getClip("FemaleScream") : SoundManager.instance.getClip("MaleScream"), 1, false, SoundManager.AudioGroup.SFX);
                if (studentData.slowMotionKill)
                {
                    StartCoroutine(GameGlobals.instance.ChangeTimeSpeed(0.3f, 1f));
                }
                yield return new WaitForSeconds(GameGlobals.instance.Player.playerAnimationComponent["f02_knifeHighSanityA_00"].length);
                FinishKill();
                break;

            case AttackType.Back:
                transform.DOLocalMove(new Vector3(0.026f, GameGlobals.instance.Player.transform.localPosition.y, 0.683f), 0.3f);
                transform.DOLocalRotate(Vector3.zero, 0.3f);

                studentAttackAnim = attackAnimation(attackType, GameGlobals.instance.Player.currentItem.weaponType);
                studentAnimation.Play(studentAttackAnim);
                GameGlobals.instance.Player.playerAnimationComponent.Play("f02_knifeStealthA_00");
                SoundManager.instance.PlaySound(SoundManager.instance.getClip("KnifeStealth"), 1, false, SoundManager.AudioGroup.SFX);
                if (studentData.slowMotionKill)
                {
                    StartCoroutine(GameGlobals.instance.ChangeTimeSpeed(0.5f, 1f));
                }
                yield return new WaitForSeconds(GameGlobals.instance.Player.playerAnimationComponent["f02_knifeStealthA_00"].length);
                FinishKill();
                break;
        }

        yield return null;
    }
    #endregion
}
