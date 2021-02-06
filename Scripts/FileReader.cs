using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class FileReader : MonoBehaviour
{

    [SerializeField] TextAsset wallText;
    string wallTextRaw;
    string[] wallTextArray;
    // Start is called before the first frame update
    void Start()
    {
        CreateWallText();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void CreateWallText()
    {
        wallTextRaw = wallText.text;
        wallTextArray = wallTextRaw.Split(new[] { "\r\n", "\r", "\n" },
                                                StringSplitOptions.None);

    }

    public string[] GetWallTextArray()
    {
        return wallTextArray;
    }
}

    


/*
        using (StreamReader sr = new StreamReader("filePath"))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] words = line.Split(' ');
            }
            */
