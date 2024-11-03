using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance
{
    //* Scripts inherit from this to save and load.
    
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}
