using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    SaveGame _saveGame = new SaveGame();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SaveGame(int val)
    {
        _saveGame.latestCheckPoint = val;
    }
}
