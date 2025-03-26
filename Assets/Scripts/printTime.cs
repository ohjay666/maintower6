using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class printTime : MonoBehaviour
{
    public GameObject OverlayTextObject;
    private OverlayText text;
    // Start is called before the first frame update
    void Start()
    {
        text = OverlayTextObject.GetComponent<OverlayText>();
    }

    // Update is called once per frame
    void Update()
    {
        float timeElapsed = Time.time;  // Time since game started in seconds
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeElapsed);
        string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");
        text.set(formattedTime);
    }
}
