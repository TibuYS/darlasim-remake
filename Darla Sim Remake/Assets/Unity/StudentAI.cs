using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Action
{
    Idle,
    Walk,
    Run,
    Sprint,
    Finished,
    ChangingShoes
}

public class StudentAI : MonoBehaviour
{
    [Header("Settings")]
    public Action CurrentAction;
    [Space]
    public float Distance;
    [Space]
    public bool AtDestination;
    public bool FinishedLocker;
    private bool IsLerping;
    [Header("Misc")]
    public Animation StudentAnims;
    [Space]
    public NavMeshAgent StudentAgent;
    [Header("Animations")]
    public string IdleAnimation = "f02_idleShort_00";
    public string WalkAnimation = "f02_newWalk_00";
    public string RunAnimation = "f02_run_00";
    public string SprintAnimation = "f02_sprint_00";
    [Space]
    public string LockerAnimation = "f02_shoeLocker5_00";
    public string DestinationAnimation = "f02_idleShort_00";
    [Header("Destinations")]
    public Transform LockerDestination;
    public Transform HangoutDestination;
    [Space]
    public Transform CurrentDestination;

    private void Start()
    {
        StudentAgent = GetComponent<NavMeshAgent>();

        CurrentDestination = LockerDestination;
    }

    private void Update()
    {
        if (IsLerping)
        {
            transform.position = Vector3.Lerp(transform.position, CurrentDestination.position, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, CurrentDestination.rotation, Time.deltaTime * 10f);
        }

        if (StudentAgent.enabled)
        {
            Distance = Vector3.Distance(transform.position, CurrentDestination.position);
            StudentAgent.SetDestination(CurrentDestination.position);
        }

        switch (CurrentAction)
        {
            case Action.Idle:
                StudentAnims.CrossFade(IdleAnimation);
                StudentAgent.speed = 0f;
                break;
            case Action.Walk:
                StudentAnims.CrossFade(WalkAnimation);
                StudentAgent.speed = 1f;
                break;
            case Action.Run:
                StudentAnims.CrossFade(RunAnimation);
                StudentAgent.speed = 3f;
                break;
            case Action.Sprint:
                StudentAnims.CrossFade(SprintAnimation);
                StudentAgent.speed = 3f;
                break;
            case Action.Finished:
                StudentAnims.CrossFade(DestinationAnimation);
                StudentAgent.speed = 0f;
                break;
            case Action.ChangingShoes:
                StudentAnims.CrossFade(LockerAnimation);
                StudentAgent.speed = 0f;
                break;
        }

        if (Distance <= 0.2f)
        {
            if (!AtDestination)
            {
                if (!FinishedLocker)
                {
                    AtDestination = true;
                    IsLerping = true;

                    CurrentAction = Action.ChangingShoes;
                    Invoke("ChangeShoes", StudentAnims[LockerAnimation].length);
                }
                else
                {
                    AtDestination = true;
                    IsLerping = true;

                    CurrentAction = Action.Finished;
                }
            }
        }
    }

    private void ChangeShoes()
    {
        AtDestination = false;
        IsLerping = false;
        FinishedLocker = true;
        
        CurrentDestination = HangoutDestination;
        CurrentAction = Action.Walk;
    }
}