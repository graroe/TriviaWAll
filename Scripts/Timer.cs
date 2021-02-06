using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{

    [SerializeField] float totalDurationSecs;
    [SerializeField] float timeRemaining;  //serialized for debugging only
    private float maxSliderValue;
    private bool timerIsRunning = true;
    

    WallState wallState;
    SceneLoader sceneLoader;
    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = totalDurationSecs;
        maxSliderValue = GetComponent<Slider>().maxValue;
        wallState = FindObjectOfType<WallState>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            Updatetimer();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
             wallState.ResolveUnfinishedWall();
             StartCoroutine("ActivateAnswers");
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            sceneLoader.ReloadToStart();
        }
    }

    private void Updatetimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            GetComponent<Slider>().value = (totalDurationSecs - timeRemaining) / totalDurationSecs * maxSliderValue;
        }
        else
        {
            FreezeTimer();
            timeRemaining = 0;
            wallState.FreezeWall();
        }
    }

    public void FreezeTimer()
    {
        timerIsRunning = false;
    }

}
