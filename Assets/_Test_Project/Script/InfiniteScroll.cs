using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfiniteScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public HorizontalLayoutGroup HLG;

    public RectTransform[] ItemList;

    private bool isScrolling = false;
    private float scrollSpeed;
    private float scrollDuration;

    private Vector2 OldVelocity;
    private bool isUpdated;
    public RayCastHitScript hitScript;
    void Start()
    {
        isUpdated = false;
        OldVelocity = Vector2.zero;
        int ItemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));

        // Populate content with items
        for (int i = 0; i < ItemsToAdd; i++)
        {
            RectTransform RT = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling();
        }
        for (int i = 0; i < ItemsToAdd; i++)
        {
            int num = ItemList.Length - i - 1;
            while (num < 0)
            {
                num += ItemList.Length;
            }
            RectTransform RT = Instantiate(ItemList[num], contentPanelTransform);
            RT.SetAsFirstSibling();
        }
        contentPanelTransform.localPosition = new Vector3((0 - (ItemList[0].rect.width + HLG.spacing) * ItemsToAdd),
            contentPanelTransform.localPosition.y,
            contentPanelTransform.localPosition.z);
        StartConstantSpeedScroll();
    }

    void Update()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = OldVelocity;
        }

        if (contentPanelTransform.localPosition.x > 0)
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            isUpdated = true;
        }

        if (contentPanelTransform.localPosition.x < 0 - (ItemList.Length * (ItemList[0].rect.width + HLG.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition += new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            isUpdated = true;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartConstantSpeedScroll();
        }
    }

    // Function to start scrolling with constant high speed and stop after a random duration
    public void StartConstantSpeedScroll()
    {
        if (!isScrolling)
        {
            isScrolling = true;
            scrollSpeed = 2500f; // Set high speed
            scrollDuration = Random.Range(2f, 5f); // Random duration between 2 and 5 seconds
            StartCoroutine(ScrollForRandomDuration());
        }
    }

    private IEnumerator ScrollForRandomDuration()
    {
        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            // Apply constant velocity to scrollRect
            scrollRect.velocity = new Vector2(-scrollSpeed, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Smoothly decelerate to stop
        float decelerationTime = 1f;
        while (scrollSpeed > 0)
        {
            scrollSpeed -= Time.deltaTime * (1500f / decelerationTime); // Decelerate over 1 second
            scrollRect.velocity = new Vector2(-scrollSpeed, 0);
            yield return null;
        }

        isScrolling = false;
        scrollRect.velocity = Vector2.zero; // Ensure it's fully stopped

        // Snap to nearest item
        SnapToNearestItem();
    }

    private void SnapToNearestItem()
    {
        float itemWidth = ItemList[0].rect.width + HLG.spacing;
        float currentX = contentPanelTransform.localPosition.x;

        // Calculate the closest item index
        int closestItemIndex = Mathf.RoundToInt(currentX / itemWidth);

        // Snap the content to the closest item's position
        float nearestItemX = closestItemIndex * itemWidth;
        contentPanelTransform.localPosition = new Vector3(nearestItemX, contentPanelTransform.localPosition.y, contentPanelTransform.localPosition.z);

        // Convert the index to a positive value if necessary
        int correctedIndex = (closestItemIndex % ItemList.Length + ItemList.Length) % ItemList.Length;
        hitScript.LoadScene();
    }
}
