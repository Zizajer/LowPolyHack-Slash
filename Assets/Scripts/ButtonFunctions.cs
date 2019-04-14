using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour {

    public void ExitGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
