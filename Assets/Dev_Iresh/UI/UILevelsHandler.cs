using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILevelsHandler : MonoBehaviour
{
    [SerializeField] List<Sprite> _imageList;

    [SerializeField] GameObject _objectThumb; 
    [SerializeField] Transform _objectParentThumb; 
    [SerializeField] GameObject _imageThumb; 
    [SerializeField] Text _titleThumb;     
    
    [SerializeField] GameObject _imageMain; 
    [SerializeField] Text _LevelNoMain; 
    [SerializeField] Text _titleMain; 
    [SerializeField] Text _descriptionMain;
    [SerializeField] GameObject _nextBtnMain;
    [SerializeField] GameObject _prevBtnMain;


    levelDataList _loadedlevelDataList = new levelDataList();

    public int _currantLevelID = 0;
    int _levelsCount = 0;

    void Start()
    {
        //LevelData levelData = new LevelData();
        //levelData.name = "l1";
        //levelData.description = "aaaaa";

        //string json = JsonUtility.ToJson(levelData);

        //print(json);
        //print(Application.dataPath);

        //File.WriteAllText(Application.dataPath + "/levelData.json", json);

        //LevelData loadedPlayerData = JsonUtility.FromJson<LevelData>(json);


        //string jsonData = File.ReadAllText(Application.dataPath + "/levelData.json");
        //LevelData loadedPlayerData = 
        //JsonUtility.FromJson<LevelData>(jsonData);
        //print("name: " + loadedPlayerData.name);
        //print("description: " + loadedPlayerData.description);
        //print("imagePath: " + loadedPlayerData.imagePath);


        string jsonData = File.ReadAllText(Application.dataPath + "/levelData.json");
        //LevelObject loadedPlayerData = JsonUtility.FromJson<LevelObject>(jsonData);

        
        object obj = _loadedlevelDataList;
        JsonUtility.FromJsonOverwrite(jsonData, obj);
        _loadedlevelDataList = (levelDataList)obj;
        _levelsCount = _loadedlevelDataList.levelList.Count;

        LoadLevelDataToThumb();

        //int i = 0;
        //foreach (var level in _loadedlevelDataList.levelList)
        //{
        //    var newObj = Instantiate(_objectThumb, _objectParentThumb);

        //    newObj.name = i.ToString();
        //    newObj.GetComponent<Button>().onClick.AddListener(delegate { LoadLevelDataToMain(Int32.Parse(newObj.name)); });
        //    newObj.transform.GetChild(0).GetComponent<Image>().sprite = _imageList[i];
        //    newObj.transform.GetChild(1).GetComponent<Text>().text = _loadedlevelDataList.levelList[i].levelMainText;
        //    i++;

        //}
        //print("daa: " + _loadedlevelDataList.levelList.Count);

        //LoadLevelDataToMain(0);
        //print("daa2: " + loadedlevelDataList.levelList[0].levelMainText);
    }

    private void Update()
    {

    }

    void LoadLevelDataToThumb()
    {
        int i = 0;
        foreach (var level in _loadedlevelDataList.levelList)
        {
            var newObj = Instantiate(_objectThumb, _objectParentThumb);

            newObj.name = i.ToString();
            newObj.GetComponent<Button>().onClick.AddListener(delegate { LoadLevelDataToMain(Int32.Parse(newObj.name)); });
            newObj.transform.GetChild(0).GetComponent<Image>().sprite = _imageList[i];
            newObj.transform.GetChild(1).GetComponent<Text>().text = _loadedlevelDataList.levelList[i].levelMainText;
            i++;

        }
        LoadLevelDataToMain(0);
    }


    public void LevelChangeFromBtns(int val)
    {
        var prevLevelID = _currantLevelID;

        _currantLevelID += val;

        if (_currantLevelID < 0)
        {
            _currantLevelID = 0;
        }

        if (_currantLevelID >= _levelsCount)
        {
            _currantLevelID = _levelsCount - 1;
        }

        if (_currantLevelID >= 0 && _currantLevelID < _levelsCount && prevLevelID != _currantLevelID)
        {
            LoadLevelDataToMain(_currantLevelID);
        }
    }

    public void LoadLevelDataToMain(int level)
    {
        _currantLevelID = level;

        if (level <= 0)
        {
            _prevBtnMain.SetActive(false);
        }
        else if (level > 0)
        {
            _prevBtnMain.SetActive(true);
        }
        if (level >= _levelsCount - 1)
        {
            _nextBtnMain.SetActive(false);
        }
        else if (level < _levelsCount)
        {
            _nextBtnMain.SetActive(true);
        }

        _imageMain.GetComponent<Image>().sprite = _imageList[level];
        _LevelNoMain.text = _loadedlevelDataList.levelList[level].levelMainText;
        _titleMain.text = _loadedlevelDataList.levelList[level].levelSubText;
        _descriptionMain.text = _loadedlevelDataList.levelList[level].LevelDescription;
    }



    //class LevelData
    //{
    //    public string name;
    //    public string description;
    //    public string imagePath;
    //}


    //[System.Serializable]
    //public struct LevelObject
    //{
    //    [System.Serializable]
    //    public struct levelList
    //    {
    //        public string levelMainText;
    //        public string levelSubText;
    //        public string LevelDescription;
    //        public string levelImagePath;
    //    }

    //    public levelList[] level;
    //}



    [System.Serializable]
    public struct levelData
    {
        public string levelMainText;
        public string levelSubText;
        public string LevelDescription;
        public string levelImagePath;
        public string levelIsPlayed;
    }

    [System.Serializable]
    public struct levelDataList
    {
        public List <levelData> levelList;
    }

    //    public static List<T> ReadFromJSON<T>(string filename)
    //    {
    //        string content = ReadFile(GetPath(filename));

    //        if (string.IsNullOrEmpty(content) || content == "{}")
    //        {
    //            return new List<T>();
    //        }
    //        List<T> res = JsonHelper.FromJson<T>(content).ToList();

    //        return res;
    //    }

    //    static string GetPath(string filename)
    //    {
    //        return Application.persistentDataPath + "/" + filename;
    //    }

    //    static string ReadFile(string path)
    //    {
    //        if (File.Exists(path))
    //        {
    //            using(StreamReader reader = new StreamReader(path))
    //            {
    //                string content = reader.ReadToEnd();
    //                return content;
    //            }
    //        }
    //        return "";
    //    }
    //}
    //public static class JsonHelper
    //{
    //    public static T[] FromJson<T>(string json)
    //    {
    //        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
    //        return wrapper.Items;
    //    }

    //    public static string ToJson<T>(T[] array)
    //    {
    //        Wrapper<T> wrapper = new Wrapper<T>();
    //        wrapper.Items = array;
    //        return JsonUtility.ToJson(wrapper);
    //    }

    //    public static string ToJson<T>(T[] array, bool prettyPrint)
    //    {
    //        Wrapper<T> wrapper = new Wrapper<T>();
    //        wrapper.Items = array;
    //        return JsonUtility.ToJson(wrapper, prettyPrint);
    //    }

    //    [Serializable]
    //    private class Wrapper<T>
    //    {
    //        public T[] Items;
    //    }
}
