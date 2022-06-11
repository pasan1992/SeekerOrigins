//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<Sprite> _bgList;

    [SerializeField] LevelListObject _levelListObject;

    // Start is called before the first frame update
    void Start()
    {
        SetLevels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateTempLevelListObj()
    {
        var lbl_obj = new LevelListObject();
        lbl_obj.levelList = new List<Level>();
        Level lvl1 = new Level();
        lvl1.LevelDescription = "This is test level";
        lvl1.levelImagePath = "Test Image path";
        lvl1.levelMainText = "sdsd";
        lvl1.levelSubText = "sdsd";
        lbl_obj.levelList.Add(lvl1);
        string json = JsonUtility.ToJson(lbl_obj);
        Debug.Log(json);
    }

    public void SetLevels()
    {





        string jsonRead = File.ReadAllText(Application.dataPath + "/LavelFile.json");
        _levelListObject = JsonUtility.FromJson<LevelListObject>(jsonRead);
        print(_levelListObject);
        print(_levelListObject.levelList[0].LevelDescription);














        //LevelRoot level1LoadedData = (LevelRoot)JsonConvert.DeserializeObject(jsonRead);
        // print(_levelListObject.levelList.Count);
        // Debug.Log(_levelListObject);

        //foreach (Level level in level1LoadedData.levels)
        //{
        //    print(level.levelMainText);
        //}

        //string level1LoadedData = JsonUtility.FromJson<string>(jsonRead);
        //print(level1LoadedData.levelMainText);
        //print(level1LoadedData.levelSubText);
        //print(level1LoadedData.ToString());

        //string jsonRead = File.ReadAllText(Application.dataPath + "/Dev_Iresh/MainMenu/levelData.json");
        //Level level1LoadedData = JsonUtility.FromJson<Level>(jsonRead);
        //print(level1LoadedData.levelMainText);
        //print(level1LoadedData.levelSubText);

        //string jsonRead = File.ReadAllText(Application.dataPath + "/Dev_Iresh/MainMenu/levelData.json");
        ////List<Level> levels = new List<Level>();
        //    var levels = JsonUtility.FromJson<Level>(jsonRead);
        //print(levels);
        ////foreach (var level in levels)
        ////{
        ////    print(level.levelMainText);
        ////    print(level.levelSubText);
        ////}


    }

    public void MainMenu()
    {
        SceneManager.LoadScene(GameEnums.Scence.Main_Menu.ToString());
    }

    public void LevelDone(int val)
    {
        if (val == 1)
        {
            PlayerPrefs.SetInt("level1", val);
        }
        else if (val == 2)
        {
            PlayerPrefs.SetInt("level2", val);
        }
        else if (val == 3)
        {
            PlayerPrefs.SetInt("level3", val);
        }
        else if (val == 0)
        {
            PlayerPrefs.SetInt("level1", val);
            PlayerPrefs.SetInt("level2", val);
            PlayerPrefs.SetInt("level3", val);
        }
    }
}
