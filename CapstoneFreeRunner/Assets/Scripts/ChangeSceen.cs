using UnityEngine;
using System.Collections;

public class ChangeSceen : MonoBehaviour {

    // Make sure the game is built with all scenes before using this script
    public void OnClick(string aSceneToGo)
    { // type in the scene name to load it
        Application.LoadLevel(aSceneToGo);
    }
}
