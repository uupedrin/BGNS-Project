using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightManager : MonoSingleton<DayNightManager>
{
    public enum DayPhase { Day, Night};
    [SerializeField] private float dayDuration = 90f;
    [SerializeField] private float nightDuration = 60f;

    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = Color.white;
    [SerializeField] private float lightTransitionDuration = 3f;

    public Action<int> OnDayStart;
    public Action<int> OnNightStart;

    public DayPhase CurrentPhase { get; private set; } = DayPhase.Day;
    public int CurrentDay { get; private set; } = 1;
    public float PhaseTimeRemaining { get; private set; }

    protected override void AwakeBehaviour()
    {
        if (SaveManager.pendingLoad) CurrentDay = SaveData.current.dayData.currentDay;
    }

    protected override void StartBehaviour()
    {
        PhaseTimeRemaining = dayDuration;
        OnDayStart?.Invoke(CurrentDay);
    }

    private void Update()
    {
        PhaseTimeRemaining -= Time.deltaTime;
        if (PhaseTimeRemaining > 0f) return;

        if (CurrentPhase == DayPhase.Day) StartNight();
        else StartDay();
    }

    private void StartDay()
    {
        CurrentPhase = DayPhase.Day;
        CurrentDay++;
        PhaseTimeRemaining = dayDuration;

        if(globalLight != null)
        {
            DOTween.To(() => globalLight.color, c => globalLight.color = c, dayColor, lightTransitionDuration);
        }
        OnDayStart?.Invoke(CurrentDay);
        SaveManager.SaveGame();
    }

    private void StartNight()
    {
        CurrentPhase = DayPhase.Night;
        PhaseTimeRemaining = nightDuration;

        if (globalLight != null)
        {
            DOTween.To(() => globalLight.color, c => globalLight.color = c, nightColor, lightTransitionDuration);
        }

        OnNightStart?.Invoke(CurrentDay);
        SaveManager.SaveGame();
    }
}
