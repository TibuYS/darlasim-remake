using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
Male,
Female
}

[CreateAssetMenu(fileName = "New Student Data File")]
public class StudentDataScriptable : ScriptableObject
{
    public bool isDead = false;
    public Gender studentGender = new Gender();
    public string IdleAnimation = "f02_idleShort_00";
    public string WalkAnimation = "f02_newWalk_00";
    public string RunAnimation = "f02_run_00";
    public string SprintAnimation = "f02_sprint_00";
    public string LockerAnimation = "f02_shoeLocker5_00";
    public string DestinationAnimation = "f02_idleShort_00";
}
