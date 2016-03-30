 using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour {

	//WHENEVER YOU WANT TO ADD A NEW DAY, MAKE SURE TO SET UP ITS GAMEOBJECT, THEN GO TO (2)
	public GameObject day1;
	public GameObject day2;
	public GameObject day3;
	public GameObject[] days; 
	public GameObject hero;
	public GameObject mainCamera;
    public GameObject daySwitch;

	public bool paused;

	public static DayManager self;

	//The day is offset by 1 because we need it to be 0-based to index the days array, but 1-based to index the... human English non-bullshit sensors.
	public int currentDay; 
	public int[] mailboxesPerDay;

	void Start () {
		Application.targetFrameRate = 60;
		self = this;

		//(2): ADD NEXT DAY TO THIS LIST
		days = new GameObject[]{ day1, day2, day3 };

		mailboxesPerDay = new int[days.Length];

		for (int i = 0; i < mailboxesPerDay.Length; i++) {
			mailboxesPerDay [i] = days [i].GetComponentsInChildren<Mailbox> ().Length;
		}
		currentDay = 0;

		//TODO we should call LoadDay here, but for testing purposes, we do not.
		Timer.self.LoadChallengeTime (4, 56);
		StatsTracker.self.ResetDelivered ();
	}
		
	public void LoadNextDay() {
        daySwitch.SendMessage("NextDay",currentDay);
		LoadDay (currentDay + 1);
	}

	public void LoadDay(int day) 
	{
		days [day].SetActive(true);
		days [currentDay].SetActive(false);
		currentDay = day;
		StatsTracker.self.ResetDelivered ();
		Timer.self.LoadChallengeTime (4, 56);
	}

	public bool PaperRouteFinished() {
		return StatsTracker.papersDelivered >= mailboxesPerDay[currentDay] - Mathf.Ceil(mailboxesPerDay[currentDay]/2);
	}

	public void ResetLevel() {
		//Sends the "Reset" message to all objects in the scene.
		//Every game object should implement a Reset function to reset its position, health,
		// etc for when the player dies or resets the level.
		//days[currentDay].BroadcastMessage ("Reset");
		BroadcastMessage ("Reset");
		hero.BroadcastMessage ("Reset");
		mainCamera.BroadcastMessage ("Reset");
	}

	public void PauseGame(bool pause) {
		paused = pause;
		PlayerController.PlayerInputEnabled = !pause;
		Timer.PauseTimer (pause);
	}
}
