using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ButtonScript : MonoBehaviour
{
    private bool recording = false;
    private GameObject recordButton;
    public Sprite start;
    public Sprite stop;
    private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        recordButton = GameObject.Find("RecordButton");
        filePath = "./" + System.DateTime.Now;
    }


    public void RecordClicked()
    {
        Debug.Log("Record clicked");
        recording = !recording;

        if (recording)
            recordButton.GetComponent<Image>().sprite = stop;
        else
            recordButton.GetComponent<Image>().sprite = start;
    }

    public void ExportClicked()
    {
        Debug.Log("Export clicked");
        // Todo: export the recording
    }

    public void FileSelectClicked()
    {
        Debug.Log("File select clicked");
        filePath = EditorUtility.OpenFilePanel("Select Recording Destination", "", ""); // args are title, start directory, file extension
    }
}
