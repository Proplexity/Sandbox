using System;
using UnityEngine;
using TMPro;

public class DayCycle : MonoBehaviour
{
    [SerializeField] float _timeMultiplier;
    [SerializeField] float _startHour;
       
    [SerializeField] Light sun;

    [SerializeField] float _sunRiseHour;
    [SerializeField] float _sunSetHour;

    private TimeSpan sunRiseTime;
    private TimeSpan sunSetTime;


    private DateTime currentTime;


    [SerializeField] TextMeshProUGUI timeText;

    private void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(_startHour);
        sunRiseTime = TimeSpan.FromHours(_sunRiseHour);
        sunSetTime = TimeSpan.FromHours(_sunSetHour);
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * _timeMultiplier);

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }


    private void RotateSun()
    {
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunRiseTime && currentTime.TimeOfDay < sunSetTime)
        {
            TimeSpan sunriseTosunsetDuration = CalculateTimeDifference(sunRiseTime, sunSetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunRiseTime, currentTime.TimeOfDay);


            double percentage = timeSinceSunrise.TotalMinutes / sunriseTosunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetTotSunriseDuration = CalculateTimeDifference(sunSetTime, sunRiseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunSetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetTotSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        sun.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }


    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan differnce = toTime - fromTime;

        if (differnce.TotalSeconds < 0)
        {
            differnce += TimeSpan.FromHours(24);
        }

        return differnce;
    }









}