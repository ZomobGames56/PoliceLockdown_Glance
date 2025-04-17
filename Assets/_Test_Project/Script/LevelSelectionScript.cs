using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelSelectionScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int middleChild = (int)((transform.childCount - 1) / 2);
        this.transform.GetChild(middleChild).localScale = Vector3.one * 1.2f;
        StartCoroutine(LevelSelection(Random.Range(10,25)));
    }
    IEnumerator LevelSelection(int rounds)
    {
        while(rounds>0)
        {
            rounds--;
            Transform go = this.transform.GetChild(0);
            go.SetSiblingIndex(transform.childCount - 1);
            int middleChild = (int)((transform.childCount - 1) / 2);
            this.transform.GetChild(middleChild-1).localScale = Vector3.one;
            this.transform.GetChild(middleChild).localScale = Vector3.one * 1.2f;
            yield return new WaitForSeconds(0.15f);

        }
    }
}
