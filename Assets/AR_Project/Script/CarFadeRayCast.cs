using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CarFadeRayCast : MonoBehaviour
{
    private List<FadeScript> previousObject = new List<FadeScript>();
    private GameObject player;
    public void SetObject(GameObject _player)
    {
        player = _player;
    }
    // Update is called once per frame
    void Update()
    {
        foreach(FadeScript scripts in previousObject)
        {
            scripts.BackToNormal();
        }
        // Get the direction from the current object to the target
        Vector3 directionToTarget = player.transform.position - this.transform.position;

        RaycastHit[] hits;
        // Cast a ray from the object's position towards the target and store all hits
        hits = Physics.RaycastAll(transform.position, directionToTarget.normalized, directionToTarget.magnitude);

        if (hits.Length > 0) // Check if any object was hit
        {
            foreach (RaycastHit hit in hits)
            {
                // Check if the hit object is not the target (meaning something is blocking the ray)
                if (hit.transform.tag != "Player")
                {
                    if (hit.collider.gameObject.TryGetComponent<FadeScript>(out FadeScript script))
                    {
                        previousObject.Add(script);
                        script.FadeMaterial();
                    }
                }
            }
        }

    }
}
