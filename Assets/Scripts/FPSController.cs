using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    public Text fpsText;
    public float updateInterval;
    private float fps;

    private int frames;

    private float timeLeft;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        frames++;
        if (timeLeft <= 0)
        {
            fps = frames / updateInterval;
            fpsText.text = "FPS: "+fps.ToString("00");
            Reset();
        }
        // var t = Time.deltaTime;
        // print(t);
        // fps = 1 / t;
        // fpsText.text = "FPS: "+fps.ToString("00");
    }

    private void Reset()
    {
        timeLeft = updateInterval;
        frames = 0;
    }
}
