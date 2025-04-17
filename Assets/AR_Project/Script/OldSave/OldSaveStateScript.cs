using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class OldSaveData
{
    public int coin;
    public int carValue;
    public int highScore;
    public List<CarObject> carBool = new List<CarObject>();
    public OldSaveData(int _coin, int _carValue , int _highScore)
    {
        coin = _coin;
        carValue = _carValue;
        highScore = _highScore;
    }
}
public class OldSaveStateScript : MonoBehaviour
{
    public List<CarObject> carObjects = new List<CarObject>();
    public OldSaveData savedData = new OldSaveData(0, 0 , 0);
    private void Awake()
    {
        GetData();
    }
    // Start is called before the first frame update
    public void GetData()
    {
        Application.ExternalCall("getUserData");
    }
    public void CreateUserState()
    {
        print("User Data is Created");
        savedData = new OldSaveData(0, 0 , 0);
        savedData.carBool.Clear();
        foreach (CarObject carObject in carObjects)
        {
            savedData.carBool.Add(carObject);
        }

    }
    public void GetState(string jsonString)
    {
        // Deserialize JSON array into a list of objects
        if (CheckForCompatibility(jsonString))
        {
            print(jsonString + "######### Data found In UNity #####");
            List<OldSaveData> OldSaveDataList = JsonConvert.DeserializeObject<List<OldSaveData>>(jsonString);
            // Access the first object
            savedData.coin = OldSaveDataList[0].coin;
            savedData.carValue = OldSaveDataList[0].carValue;
            foreach (CarObject carobject in carObjects)
            {
                foreach (CarObject savedObject in OldSaveDataList[0].carBool)
                {
                    if (savedObject.name == carobject.name)
                    {
                        carobject.isAvailable = savedObject.isAvailable;
                    }
                }
            }
            SaveState();
            print("Data recieved in unity succesfully");
        }
        else
        {
            CreateUserState();
        }
    }
    private bool CheckForCompatibility(string jsonString)
    {
        try
        {
            List<OldSaveData> dataList = JsonConvert.DeserializeObject<List<OldSaveData>>(jsonString);
            return dataList != null && dataList.Count > 0; // Check if list is not null or empty
        }
        catch
        {
            Debug.LogError("JSON Error User Data is Not Valid");
            return false; // JSON is invalid
        }
    }
    public void SaveState()
    {
        print(savedData.coin + "save data coin" + savedData.carValue + "save data CarValue");
        List<OldSaveData> OldSaveDataArray = new List<OldSaveData>
        {
            savedData
        };

        string jsonData = JsonConvert.SerializeObject(OldSaveDataArray);
        string jsFunctionCall = $"saveUserData({jsonData});";
        Application.ExternalEval(jsFunctionCall);
    }
}
