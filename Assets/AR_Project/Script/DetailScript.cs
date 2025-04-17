using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DetailScript : MonoBehaviour
{
    [SerializeField] private Slider detailSlider;
    [SerializeField] private TextMeshProUGUI detailName, detailValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetDetails(string name , string value , float sliderValue)
    {
        detailName.text = name;
        detailValue.text = value;
        detailSlider.value = sliderValue;
    }
}
