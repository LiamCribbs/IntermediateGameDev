using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTimer : MonoBehaviour, ISaveable
{
    float time;
    public float startTime;

    public TMPro.TextMeshPro text0;
    public TMPro.TextMeshPro text1;

    public UnityEngine.Events.UnityEvent onTimerComplete;

    void Awake()
    {
        time = startTime;
    }

    void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                time = 0f;
                onTimerComplete.Invoke();
            }

            text0.text = Mathf.Round(time) + "s";
            text1.text = text0.text;
        }
    }

    public SaveData Save()
    {
        return new TrainLeaveStationSaveData()
        {
            timer = time
        };
    }

    public void Load(SaveData baseSaveData)
    {
        var saveData = (TrainLeaveStationSaveData)baseSaveData;
        time = saveData.timer;
        if (time <= 0f)
        {
            onTimerComplete.Invoke();
            text0.text = Mathf.Round(time) + "s";
            text1.text = text0.text;
        }
    }
}