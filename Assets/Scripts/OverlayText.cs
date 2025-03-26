using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI; // For regular UI Text

public class OverlayText : MonoBehaviour
{
    private Text legacyText;
    private TextMeshProUGUI tmText;
    private string text;
    // private Canvas;

    void Start()
    {
        legacyText = gameObject.GetComponent<Text>();
        tmText = gameObject.GetComponent<TextMeshProUGUI>();

    }

    public void set(object newText)
    {
        text = "" + newText;
        output();
    }
    public void ln(object appendText)
    {
        if(text is null){
            text = "";
        }
        text += "\n" + appendText;
        output();
    }
    public void round(float number,int digits = 0){
        ln(Math.Round(number,digits));
    }
    private void output()
    {
        if (legacyText != null)
        {
            legacyText.text = text;
        }
        if (tmText != null)
        {
            tmText.text = text;
        }
    }

}