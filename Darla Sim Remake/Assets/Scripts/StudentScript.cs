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

[System.Serializable]
public class DestinationSpot
{
    public string destinationName = "Unnamed Destination";
    public Phase destinationPhase;
    public Transform destinationLocation;
    public string destinationAnimation;
}

public class StudentScript : MonoBehaviour
{
    [Header("Edit in inspector")]
    public Color subtitleColor;
    [Range(1.2f, 5)]public float pathfindingSpeed = 1.2f;
    public Transform studentLocker;
    public List<DestinationSpot> destinationSpots = new List<DestinationSpot>();
    [Space]
    [Header("Components")]
    public Animation studentAnimation;
    public NavMeshAgent studentAgent;
    public PromptScript studentPrompt;
    public StudentDataScriptable studentData;
    [Space]
    [Header("Runtime values")]
    public bool changingShoes = false;
    public bool changedShoes = false;
    private Transform shoeChangeSpot;
    public DestinationSpot currentDestination;
    private UnityEvent talkEvent;
    private UnityEvent killEvent;
    private UnityEvent dragEvent;
    private UnityEvent dropEvent;
    
    void Start()
    {
        ResetData(); //until we have more students
        BecomeRagdoll(false); //so she doesnt fall lol
        talkEvent = new UnityEvent();
        talkEvent.AddListener(Talk);
        killEvent = new UnityEvent();
        killEvent.AddListener(Kill);
        dragEvent = new UnityEvent();
        dragEvent.AddListener(Drag);
        dropEvent = new UnityEvent();
        dropEvent.AddListener(Drop);
        GameObject spot = new GameObject();
        studentAgent.speed = pathfindingSpeed;
        shoeChangeSpot = spot.transform;
        shoeChangeSpot.transform.localPosition = new Vector3(studentLocker.transform.localPosition.x + 0.45f, studentLocker.transform.localPosition.y, studentLocker.transform.localPosition.z);
        shoeChangeSpot.transform.localEulerAngles = new Vector3(studentLocker.transform.localEulerAngles.x, -studentLocker.transform.localEulerAngles.y, studentLocker.transform.localPosition.z);
        DestinationSpot newSpot = new DestinationSpot();
        newSpot.destinationLocation = shoeChangeSpot;
        newSpot.destinationName = gameObject.name + " shoe changing spot";
        shoeChangeSpot.gameObject.name = newSpot.destinationName;
        SetDestination(newSpot);
    }

    #region related to pathfinding
    public void SetDestination(DestinationSpot newDestinationSpot)
    {
        currentDestination = newDestinationSpot;
        studentAgent.SetDestination(newDestinationSpot.destinationLocation.localPosition);
    }

    public void AdjustToDayPhase()
    {
        if (!changedShoes) { return; }
        foreach(DestinationSpot newSpot in destinationSpots)
        {
            if(newSpot.destinationPhase == GameGlobals.instance.TimeManager.CurrentPhase)
            {
                SetDestination(newSpot);
            }
        }
    }

    public void Stop(bool shouldStop, bool adjustToCurrentDestination = false)
    {
        studentAgent.isStopped = shouldStop;
        if (adjustToCurrentDestination && currentDestination != null) transform.DOLocalMove(currentDestination.destinationLocation.localPosition, 0.2f);
        if (adjustToCurrentDestination && currentDestination != null) transform.DOLocalRotate(currentDestination.destinationLocation.localEulerAngles, 0.2f);
    }

    public void PlayDestinationAnimation()
    {
        if(currentDestination == null) { return; }
        string animToPlay = currentDestination.destinationAnimation == "" ? studentData.IdleAnimation : currentDestination.destinationAnimation;
        studentAnimation.CrossFade(animToPlay);
    }
    #endregion


    private void Update()
    {

        if (studentData.isDead) return;
        UpdatePathfinding();
        UpdatePrompt();  
    }

    IEnumerator changeShoes()
    {
        Stop(true, true);
        changingShoes = true;
        string shoeChangeAnim = studentData.studentGender == Gender.Female ? "f02_shoeLocker5_00" : "shoeLocker5_00";
        studentAnimation.Play(shoeChangeAnim);
        yield return new WaitForSeconds(studentAnimation[shoeChangeAnim].length);
        changingShoes = false;
        changedShoes = true;
        Stop(false);
        AdjustToDayPhase();
    }

    public void UpdatePathfinding()
    {
        if (!studentAgent.enabled) return;
        if (studentAgent.isStopped) return;

        if (studentAgent.remainingDistance < 0.1)
        {
            if (!studentAgent.isStopped)
            {
                Stop(true, true);
                PlayDestinationAnimation();
                if (!changedShoes && !changedShoes) StartCoroutine(changeShoes());
            }
        }
        else
        {
            if (studentAgent.speed == 1.2f)
            {
                studentAnimation.CrossFade(studentData.WalkAnimation);
            }
            else if (studentAgent.speed >= 3)
            {
                studentAnimation.CrossFade(studentData.SprintAnimation);
            }
        }
    }
    public void UpdatePrompt()
    {
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
        if (changingShoes)
        {
            SubtitleManagerScript.instance.DisplaySubtitle("I'm busy right now.", 3, subtitleColor);
            return;
        }
        SubtitleManagerScript.instance.DisplaySubtitle("I can't talk right now, sorry!", 4, subtitleColor);
        SubtitleManagerScript.instance.DisplaySubtitle("[Conversation system not implemented yet!]", 4, Color.red);
    }
    #endregion

    #region everything related to attacking and killing
    public void Kill()
    {
        if (GameGlobals.instance.Player.isKilling) return; //alr in progress or alr in a killing state with another student ??
        StopAllCoroutines();
        GameGlobals.instance.Player.isKilling = true;
        studentAnimation.Stop();
        Stop(true);
        GameGlobals.instance.PromptManager.enabled = false;
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
        GameGlobals.instance.Player.isKilling = false;
        StartCoroutine(GameGlobals.instance.ChangeTimeSpeed(1f, 0.5f));
        GameGlobals.instance.PromptManager.enabled = true;
        // SlowMotionScript.instance.SlowMotionEffect(false);
        GameGlobals.instance.Player.enabled = true;
        GameGlobals.instance.Player.currentItem.enabled = true;
        studentData.isDead = true;
        transform.parent = null;
        BecomeRagdoll(true);
        ShowPrompt(true);
        UpdateStudentPrompt("Drag", KeyCode.Q, dragEvent);
    }

    IEnumerator KillRoutine(AttackType attackType)
    {
        GameGlobals.instance.Player.enabled = false; //stop player
        GameGlobals.instance.Player.currentItem.enabled = false; //prevents dropping mid-attack
        ShowPrompt(false);
        transform.parent = GameGlobals.instance.Player.transform;
        string studentAttackAnim = "";
        Stop(true, false);
        switch (attackType)
        {
            case AttackType.Front:
                transform.DOLocalMove(new Vector3(0, 0, 1.004f), 0.3f);
                transform.DOLookAt(GameGlobals.instance.Player.transform.localPosition, 0.3f);
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

    #region everything related to ragdolls
    public void BecomeRagdoll(bool becomeRagdoll)
    {
        // Toggle animations and agents based on the ragdoll state
        studentAnimation.enabled = !becomeRagdoll;
        studentAgent.enabled = !becomeRagdoll;

        // Retrieve all rigidbodies and colliders in children
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        // If transitioning to ragdoll
        if (becomeRagdoll)
        {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false; // Make all rigidbodies non-kinematic
                rb.useGravity = true; // Enable gravity
            }

            // Disable main collider if necessary
            foreach (var col in colliders)
            {
                if (col.gameObject.name != gameObject.name) // Keep the main collider active
                {
                    col.enabled = true; // Enable all colliders
                }
            }

            // Ensure main Rigidbody is not kinematic
            Rigidbody mainRigidbody = GetComponent<Rigidbody>();
            if (mainRigidbody != null)
            {
                mainRigidbody.isKinematic = false; // Make sure the main Rigidbody is non-kinematic
            }
        }
        else // Transitioning out of ragdoll
        {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true; // Set rigidbodies to kinematic
                rb.useGravity = false; // Disable gravity
            }

            // Enable colliders based on ragdoll state
            foreach (var col in colliders)
            {
                if (col.gameObject.name != gameObject.name) // Keep the main collider active
                {
                    col.enabled = false; // Disable all colliders
                }
            }

            // Reset layer collision settings
            int objectLayer = gameObject.layer;
            for (int i = 0; i < 32; i++)
            {
                if (i != objectLayer)
                {
                    Physics.IgnoreLayerCollision(objectLayer, i, true); // Disable collisions
                }
            }

        }

        // Handle layer collisions
        int layer = gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (i != layer)
            {
                Physics.IgnoreLayerCollision(layer, i, !becomeRagdoll); // Enable or disable layer collisions
            }
        }
    }



    public void Drag()
    {
        if (GameGlobals.instance.Player.currentCorpse != null) return;
        UpdateStudentPrompt("Drop", KeyCode.Q, dropEvent);
        GameGlobals.instance.Player.DragBody(this);
        string limbPath = studentData.studentGender == Gender.Female ? "PelvisRoot/Hips/Spine/Spine1/Spine2/Spine3/RightShoulder/RightArm/RightArmRoll/RightForeArm" : "";
        Rigidbody limbToGrab = transform.Find(limbPath).gameObject.GetComponent<Rigidbody>();
        if(limbToGrab != null) GameGlobals.instance.Player.limbDragger.GrabLimb(limbToGrab);
    }

    public void Drop()
    {
        if (GameGlobals.instance.Player.currentCorpse != this|| GameGlobals.instance.Player.currentCorpse == null) return;
        BecomeRagdoll(true);
        GameGlobals.instance.Player.DropBody();
        GameGlobals.instance.Player.limbDragger.ReleaseLimb();
        UpdateStudentPrompt("Drag", KeyCode.Q, dragEvent);
    }
    #endregion
}