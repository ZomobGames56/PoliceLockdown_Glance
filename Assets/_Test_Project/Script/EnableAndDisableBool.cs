using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAndDisableBool : MonoBehaviour
{
    //public SaveStateScript saveStateScript;
    public List<CarObject> carobjects;
    public SaveStateScript saveStateScript;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            carobjects[index].isAvailable = true;
            index++;
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if (index > 1)
            {

                index--;
                carobjects[index].isAvailable = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            saveStateScript.savedData.coin += 10;
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            saveStateScript.SaveState();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            saveStateScript.GetData();
        }
    }
}
