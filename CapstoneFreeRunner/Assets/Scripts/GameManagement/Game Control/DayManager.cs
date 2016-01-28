using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour {

	//WHENEVER YOU WANT TO ADD A NEW DAY, MAKE SURE TO SET UP ITS GAMEOBJECT, THEN GO TO (2)
	public GameObject day1;
	public GameObject day2;
	public GameObject[] days; 

	public static DayManager self;

	//The day is offset by 1 because we need it to be 0-based to index the days array, but 1-based to index the... human English non-bullshit sensors.
	public int currentDay; 
	public int[] mailboxesPerDay;

	void Start () {
		self = this;

		//(2): ADD NEXT DAY TO THIS LIST
		days = new GameObject[]{ day1, day2 };

		mailboxesPerDay = new int[days.Length];

		for (int i = 0; i < mailboxesPerDay.Length; i++) {
			mailboxesPerDay [i] = days [i].GetComponentsInChildren<Mailbox> ().Length;
			Debug.Log (mailboxesPerDay[i]);
		}
		currentDay = 1;
	}

	public void LoadNextDay() {
		LoadDay (currentDay + 1);
	}

	public void LoadDay(int day) {
		days [day - 1].SetActive(true);
		days [currentDay - 1].SetActive(false);
		currentDay = day;
	}

	public bool PaperRouteFinished() {
		return false;
	}

	public void ResetLevel() {
		//Sends the "Reset" message to all objects in the scene.
		//Every game object should implement a Reset function to reset its position, health,
		// etc for when the player dies or resets the level.
		days[currentDay].BroadcastMessage ("Reset");
	}
}
