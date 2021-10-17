using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    SaveData Save();

    void Load(SaveData saveData);
}