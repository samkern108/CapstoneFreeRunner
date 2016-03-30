using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class DaySwitch : MonoBehaviour {

    public GameObject daySwitchUI;
    public GameObject text;
    private int dayNormalized;

	// Use this for initialization
	void Start () {
        daySwitchUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextDay(int day) {
        dayNormalized = day += 1;
        daySwitchUI.SetActive(true);
        DayCompleted();
    }

    public void DayCompleted() {
        text.GetComponent<Text>().text = "Day " + dayNormalized +  "\n Completed";
        Invoke("DayBegin", 2f);
    }

    public void DayBegin() {
        Debug.Log("Called");
        dayNormalized += 1;
        text.GetComponent<Text>().text = "Day " + dayNormalized + "\n Begin";
        Invoke("HideUI", 2f);
    }

    public void HideUI() {
        daySwitchUI.SetActive(false);
    }
}
