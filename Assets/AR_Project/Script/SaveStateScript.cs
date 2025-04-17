using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
[System.Serializable]
public class SaveData
{
    public int coin;
    public int carValue;
    public int highScore;
    public List<CarData> carData = new List<CarData>();
    public SaveData(int _coin, int _carValue, int _highScore , List<CarData> _carDatas)
    {
        coin = _coin;
        carValue = _carValue;
        highScore = _highScore;
        carData = _carDatas;
    }
}
[System.Serializable]
public class CarData
{
    public bool purchased;
    public string name;
    public CarData(bool _purchased , string _name)
    {
        purchased = _purchased;
        name = _name;
    }
}
public class SaveStateScript : MonoBehaviour
{
    public static bool openedFirstTime = false;
    public SaveData savedData;
    public List<CarObject> carObjects = new List<CarObject>();
    public GameData gameData;
    private void Awake()
    {
        if(!openedFirstTime && SceneManager.GetActiveScene().buildIndex == 0)
        {
            GetData();
            print("GET STATE IS CALLED");
            openedFirstTime = true;
        }
        else
        {

            List<CarData> carDatas = new List<CarData>();
            foreach (CarObject carObject in carObjects)
            {
                CarData newData = new CarData(carObject.isAvailable, carObject.name);
                carDatas.Add(newData);
            }
            //Create New Data
            savedData = new SaveData(0, 0, 0, carDatas);
            savedData.coin = gameData.coin;
            savedData.highScore = gameData.highScore;
            savedData.carValue = gameData.carValue;
            print(JsonUtility.ToJson(savedData, true));
            print(JsonUtility.ToJson(gameData, true));

        }
    }
    // Start is called before the first frame update
    public void GetData()
    {
        Application.ExternalCall("getUserData");
    }
    public void CreateUserState()
    {
        print("New User Data Created");
        List<CarData> carDatas = new List<CarData>();
        foreach (CarObject carObject in carObjects)
        {
            CarData newData = new CarData(carObject.isAvailable, carObject.name);
            carDatas.Add(newData);
        }
        //Create New Data
        savedData = new SaveData(0, 0, 0 , carDatas);

    }
    public void GetState(string jsonString)
    {
        if(CheckString(jsonString))
        {
            //Get State
            List<SaveData> SaveDataList = JsonConvert.DeserializeObject<List<SaveData>>(jsonString);
            savedData.coin = SaveDataList[0].coin;
            savedData.highScore = SaveDataList[0].highScore;
            savedData.carValue = SaveDataList[0].carValue;
            savedData.carData.Clear();
            foreach (CarData r_Data in SaveDataList[0].carData)
            {
                CarData _carData = new CarData(r_Data.purchased, r_Data.name);
                savedData.carData.Add(_carData);
            }
            foreach (CarObject carObject in carObjects)
            {
                foreach(CarData carData in savedData.carData)
                {
                    if(carData.name == carObject.name)
                    {
                        carObject.isAvailable = carData.purchased;
                    }
                }
            }
            print("Data retreived from the backend to Unity" + jsonString);
            string jsonData = JsonUtility.ToJson(savedData, true);
            print("Data Converted form backend to unity" + jsonData);

        }
        else
        {
            CreateUserState();
        }
    }
    private bool CheckString(string data)
    {
        try
        {
            List<SaveData> dataList = JsonConvert.DeserializeObject<List<SaveData>>(data);
            return dataList != null && dataList.Count > 0 && dataList[0].carData.Count>0; // Check if list is not null or empty
        }
        catch
        {
            Debug.LogError("JSON Error User Data is Not Valid");
            return false; // JSON is invalid
        }
    }
    public void SaveState()
    {
        gameData.coin = savedData.coin;
        gameData.highScore = savedData.highScore;
        gameData.carValue = savedData.carValue;

        savedData.carData.Clear();
        foreach (CarObject carObject in carObjects)
        {
            CarData _carData = new CarData(carObject.isAvailable, carObject.name);
            savedData.carData.Add(_carData);
        }
        //Set State
        List<SaveData> SaveDataArray = new List<SaveData>
        {
            savedData
        };
        string jsonData = JsonConvert.SerializeObject(SaveDataArray);
        string jsFunctionCall = $"saveUserData({jsonData});";
        Application.ExternalEval(jsFunctionCall);
    }
    public int GetCoin()
    {
        return savedData.coin;
    }
    public void SetCoin(int _coin)
    {
        print("Set Coin" + _coin);
        savedData.coin = _coin;
    }
    public int GetHighScore()
    {
        return savedData.highScore;
    }
    public void SetHighScore(int _highScore)
    {
        print("Set Car Score" + _highScore);
        savedData.highScore = _highScore;
    }
    public int GetCarValue()
    {
        return savedData.carValue;
    }
    public void SetCarValue(int _carValue)
    {
        print("Set Car Value" + _carValue);
        savedData.carValue = _carValue;
    }
}