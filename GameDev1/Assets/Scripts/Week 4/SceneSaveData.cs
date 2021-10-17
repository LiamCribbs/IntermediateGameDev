using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSaveData
{
    public List<SaveData> saveData;
}

public abstract class SaveData
{

}

public class ElevatorSaveData : SaveData
{
    public Vector2 position;
}