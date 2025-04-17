using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class RayCastHitScript : MonoBehaviour
{
    public GameObject previousObject;
    public Canvas canvas;  // Reference to your canvas
    private int currentIndex = 0;
    void Update()
    {
       RaycastAtCenter();
    }

    void RaycastAtCenter()
    {
        // Get the screen center point
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        // Create PointerEventData at the screen center
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenCenter
        };

        // Create a list to store raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // Get the GraphicRaycaster from the canvas
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
        {
            Debug.LogError("No GraphicRaycaster attached to the canvas!");
            return;
        }

        // Perform the raycast
        raycaster.Raycast(pointerData, results);
        if (previousObject != null)
        {
            previousObject.transform.localScale = Vector3.one;
        }
        // Log all UI elements hit
        foreach (RaycastResult result in results)
        {
            if(result.gameObject.TryGetComponent<LevelIndexScript>(out LevelIndexScript indexScript))
            {
                currentIndex = indexScript.GetIndex();
                if(previousObject != result.gameObject && previousObject != null)
                {
                    previousObject.transform.localScale = Vector3.one;
                }
                result.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                previousObject = result.gameObject;
            }

        }

        // If no UI elements were hit
        if (results.Count == 0)
        {

        }
    }
    public void LoadScene()
    {
        StartCoroutine("LoadLevel");

    }
    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(currentIndex);
    }
}
