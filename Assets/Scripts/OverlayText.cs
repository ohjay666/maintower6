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
        tmText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void set(object newText)
    {
        text = "" + newText;
        output();
    }
    public string get()
    {
        return tmText.text;
    }

    public void ln(object appendText)
    {
        if (text is null)
        {
            text = "";
        }
        text += "\n" + appendText;
        output();
    }
    public void round(float number, int digits = 0)
    {
        ln(Math.Round(number, digits));
    }
    private void output()
    {

        tmText.text = text;

    }

}