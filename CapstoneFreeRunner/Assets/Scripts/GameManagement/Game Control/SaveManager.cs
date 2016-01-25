using UnityEngine;
using System.Collections;

public class SaveManger : MonoBehaviour {

	void OnApplicationQuit() {
		PlayerPrefs.Save();
	}
}
