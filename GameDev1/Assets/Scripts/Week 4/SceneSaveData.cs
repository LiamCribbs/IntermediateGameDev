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

public class DialogueEmitterSaveData : SaveData
{
    public int dialogueIndex;
    public bool ended;
    public Vector3 position;
}

public class CollectableSaveData : SaveData
{
    public bool collected;
}

public class ScudsRoomManagerSaveData : SaveData
{
    public bool hasFallGuyFallen;
}