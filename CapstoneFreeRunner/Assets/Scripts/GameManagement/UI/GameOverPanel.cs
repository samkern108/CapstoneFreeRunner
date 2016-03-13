using UnityEngine;
using System.Collections;

public class GameOverPanel : MonoBehaviour {

	void Update () {
		if (InputWrapper.Restart ()) {
			DayManager.self.ResetLevel ();
		}
	}
}
