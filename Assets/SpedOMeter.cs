using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SpedOMeter : MonoBehaviour
{
    public Slider slider;
    public Slider MySlider { get { return slider; } set { slider = value; } }
    public TextMeshProUGUI text;

    public void updateText()
    {
        Debug.Log("Should be updating text");
        text.text = slider.value.ToString("#");
    }
}
