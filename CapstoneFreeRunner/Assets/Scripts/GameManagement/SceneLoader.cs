using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

    // Make sure the game is built with all scenes before using this script
    public void OnClick(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}
