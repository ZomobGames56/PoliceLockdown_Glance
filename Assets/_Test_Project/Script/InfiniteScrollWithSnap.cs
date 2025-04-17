using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollWithSnap : MonoBehaviour
{
    [Header("Scroll Settings")]
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public HorizontalLayoutGroup HLG;

    [Header("Level Selection Settings")]
    public GameObject scrollBar;
    public RectTransform[] ItemList;

    private Vector2 oldVelocity;
    private bool isUpdated;
    private float[] pos;
    private float scrollPos;
    private float distance;

    void Start()
    {
        isUpdated = false;
        oldVelocity = Vector2.zero;

        // Initialize infinite scroll
        int itemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));
        for (int i = 0; i < itemsToAdd; i++)
        {
            RectTransform rt = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            rt.SetAsLastSibling();
        }
        for (int i = 0; i < itemsToAdd; i++)
        {
            int num = ItemList.Length - i - 1;
            while (num < 0)
            {
                num += ItemList.Length;
            }
            RectTransform rt = Instantiate(ItemList[num], contentPanelTransform);
            rt.SetAsFirstSibling();
        }
        contentPanelTransform.localPosition = new Vector3((0 - (ItemList[0].rect.width + HLG.spacing) * itemsToAdd),
            contentPanelTransform.localPosition.y,
            contentPanelTransform.localPosition.z);

        // Initialize level selection
        pos = new float[contentPanelTransform.childCount];
        distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
    }

    void Update()
    {
        HandleInfiniteScroll();
        HandleLevelSelection();
    }

    private void HandleInfiniteScroll()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = oldVelocity;
        }

        if (contentPanelTransform.localPosition.x > 0)
        {
            Canvas.ForceUpdateCanvases();
            oldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            isUpdated = true;
        }
        if (contentPanelTransform.localPosition.x < 0 - (ItemList.Length * (ItemList[0].rect.width + HLG.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            oldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition += new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            isUpdated = true;
        }
    }

    private void HandleLevelSelection()
    {
        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollBar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
                {
                    scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            {
                contentPanelTransform.GetChild(i).localScale = Vector2.Lerp(contentPanelTransform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        contentPanelTransform.GetChild(a).localScale = Vector2.Lerp(contentPanelTransform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }
    }
}