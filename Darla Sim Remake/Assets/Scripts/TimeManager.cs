using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public enum Phase
{
    BeforeClass,
    ClassPreparation,
    Classtime,
    Lunchtime,
    CleaningTime,
    EndOfDay,
    None
}

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI HourText;
    public TextMeshProUGUI DayPhaseText;
    public Phase CurrentPhase;
    public Phase PreviousPhase;
    public static TimeManager Instance;
    [Range(0.1f, 100)] public float timeMultiplier = 1.0f; 

    public float seconds;
    public int hours;
    public int minutes;
    public string AMPM;
    public bool timePaused;



    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        hours = 7;
        AMPM = "AM";
        CurrentPhase = Phase.BeforeClass;
    }
    public void SetTime(int hours_, int minutes_, string ampm_)
    {
        AMPM = ampm_;
        seconds = 0;
        hours = hours_;
        minutes = minutes_;
        UpdateTime();
        UpdateDayPhase();
    }

    private void Update()
    {
        if (!timePaused)
        {
            UpdateTime();
            UpdateDayPhase();
            if (Input.GetKeyDown(KeyCode.P)) minutes = 59;
        }
    }

    void UpdateTime()
    {
        seconds += Time.deltaTime * timeMultiplier;

        if (seconds >= 20f)
        {
            minutes++;
            seconds = 0f;
        }

        if (minutes >= 60)
        {
            hours++;
            minutes = 0;
        }

        if (hours == 12)
        {
            AMPM = "PM";
        }

        if (hours >= 13)
        {
            hours = 1;
            minutes = 0;
        }

        UpdateText();
    }

    void UpdateDayPhase()
    {
        if (hours == 7 && minutes == 0) CurrentPhase = Phase.BeforeClass;
        if (hours == 8 && minutes == 0) CurrentPhase = Phase.ClassPreparation;
        if ((hours == 8 && minutes == 30) || (hours == 1 && minutes == 29)) CurrentPhase = Phase.Classtime;
        if (hours == 1 && minutes == 30 && AMPM == "PM") CurrentPhase = Phase.Lunchtime;
        if (hours == 2 && minutes == 00 && AMPM == "PM") CurrentPhase = Phase.ClassPreparation;
        if (hours == 2 && minutes == 15 && AMPM == "PM") CurrentPhase = Phase.Classtime;
        if (hours == 3 && minutes == 30) CurrentPhase = Phase.CleaningTime;
        if (hours == 4 && minutes == 30) CurrentPhase = Phase.EndOfDay;
        if (PreviousPhase != CurrentPhase && CurrentPhase != Phase.BeforeClass)
        {
            SetPreviousPhase();
        }
        DayPhaseText.text = GetCurrentPhase().ToString();
    }

   

    void SetPreviousPhase()
    {
        PreviousPhase = CurrentPhase;
    }

    public Phase GetCurrentDayPhase()
    {
        return CurrentPhase;
    }

    private string GetCurrentPhase()
    {
        switch (CurrentPhase)
        {
            case Phase.BeforeClass:
                return "Before Class";

            case Phase.ClassPreparation:
                return "Class Preparation";

            case Phase.Classtime:
                return "Class Time";

            case Phase.Lunchtime:
                return "Lunch Time";

            case Phase.CleaningTime:
                return "Cleaning Time";

            case Phase.EndOfDay:
                return "After School";
        }

        return string.Empty;
    }

    private void UpdateText()
    {
        string hourString = hours.ToString("00");
        string minuteString = minutes.ToString("00");
        HourText.text = hourString + ":" + minuteString + " " + AMPM;
    }
}