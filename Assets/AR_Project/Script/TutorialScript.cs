using UnityEngine;
using System.Collections.Generic;
public class TutorialScript : MonoBehaviour
{
    private string firstOpen = "GameStarted";
    private int screenIndex;
    [SerializeField] private List<GameObject> screens = new List<GameObject>();
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();
    private void Awake()
    {
        if (!PlayerPrefs.HasKey(firstOpen))
        {
            PlayerPrefs.SetInt(firstOpen, 1);
            GameObjectAction(false);
            SetAllFalse();
            SetActive(screenIndex = 0);
        }else
        {
            GameObjectAction(true);
            this.gameObject.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Next()
    {
        if (screenIndex < screens.Count - 1)
        {
            screenIndex++;
            SetActive(screenIndex);
        }
        else
        {
            GameObjectAction(true);
            this.gameObject.SetActive(false);
        }
    }
    private void GameObjectAction(bool _bool)
    {
        foreach(GameObject go in gameObjects)
        {
            go.SetActive(_bool);
        }
    }
    private void SetAllFalse()

    {
        foreach (GameObject go in screens)
        {
            go.SetActive(false);
        }
    }
    private void SetActive(int index)
    {
        if(index > 0)
            screens[index - 1].SetActive(false);
        screens[index].SetActive(true);
    }
}
