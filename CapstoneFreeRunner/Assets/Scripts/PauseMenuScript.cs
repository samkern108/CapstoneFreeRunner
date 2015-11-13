using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {
    bool IsPaused = false;
    Rect Menu = new Rect(10, 10, 200, 100); //makes the pop up window
                                            // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // when the player hits 'esc' the game pauses
        {
            IsPaused = !IsPaused;
            if (IsPaused)
            {
                Time.timeScale = 0; //just a precaution for animation
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
    void OnGUI() //this function needs to be called 'OnGUI()' in order for the whole script to work
    {
        if (IsPaused) //when the game is paused, a window will pop up
        {
            Menu = GUILayout.Window(0, Menu, thePauseMenu, "PauseMenu");
        }
    }

    void thePauseMenu(int id) //make sure there is an int in here
    {
        if (GUILayout.Button("Resume")) //GUILayout will have automatic layout
        {
            IsPaused = !IsPaused;
            Time.timeScale = 1;
        }
        if (GUILayout.Button("Main Menu"))
        {
            Application.LoadLevel("TitleScreen");
        }
        if (GUILayout.Button("Quit Game"))
        {
            Application.Quit();
        }
    }
}
