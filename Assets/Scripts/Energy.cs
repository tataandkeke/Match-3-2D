using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



public class Energy : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] TextMeshProUGUI timerText;


    private int maxEnergy = 5;
    public int currentEnergy;
    private int restoreDuration = 5;

    private DateTime nextEnergyTime;
    private DateTime lastEnergyTime;
    private bool isRestoring = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("CurrentEnergy"))
        {
            PlayerPrefs.SetInt("CurrentEnergy", 5);
            Load();
            StartCoroutine(RestoreEnergy());
        }
        else
        {
            Load();
            StartCoroutine(RestoreEnergy());
        }
    }

    public void UseEnergy()
    {
        if(currentEnergy >= 1)
        {
            currentEnergy--;
            UpdateEnergy();

            if(isRestoring == false)
            {
                if(currentEnergy + 1 == maxEnergy)
                {
                    nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);
                }

                StartCoroutine(RestoreEnergy());
            }
        }

        else
        {
            Debug.Log("Insufficient Energy");
        }
    }

    private IEnumerator RestoreEnergy()
    {
        UpdateEnergyTimer();
        isRestoring = true;

        while(currentEnergy < maxEnergy)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime nextDateTime = nextEnergyTime;
            bool isEnergyAdding = false;

            while(currentDateTime > nextDateTime)
            {
                if(currentEnergy < maxEnergy)
                {
                    isEnergyAdding = true;
                    currentEnergy++;
                    UpdateEnergy();
                    DateTime timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                    nextDateTime = AddDuration(timeToAdd, restoreDuration);
                }
                else
                {
                    break;
                }
            }

            if(isEnergyAdding == true)
            {
                lastEnergyTime = DateTime.Now;
                nextEnergyTime = nextDateTime;
            }

            UpdateEnergyTimer();
            UpdateEnergy();
            Save();
            yield return null;
        }

        isRestoring = false;
    }

    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        return dateTime.AddMinutes(duration);
    }

    private void UpdateEnergyTimer()
    {
        

        if (currentEnergy >= maxEnergy)
        {
            timerText.text = "Full";
            return;
        }

        TimeSpan time = nextEnergyTime - DateTime.Now;
        string timeValue = String.Format("{0:D2}:{1:D1}", time.Minutes, time.Seconds);
        timerText.text = timeValue;
    }

    private void UpdateEnergy()
    {
        energyText.text = currentEnergy.ToString() + "/" + maxEnergy.ToString();
    }

    private DateTime StringToDate(string datetime)
    {
        if (String.IsNullOrEmpty(datetime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(datetime);
        }
    }

    private void Load()
    {
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy");
        nextEnergyTime = StringToDate(PlayerPrefs.GetString("NextEnergyTime"));
        lastEnergyTime = StringToDate(PlayerPrefs.GetString("LastEnergyTime"));
    }

    private void Save()
    {
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetString("NextEnergyTime", nextEnergyTime.ToString());
        PlayerPrefs.SetString("LastEnergyTime", lastEnergyTime.ToString());
    }
}
