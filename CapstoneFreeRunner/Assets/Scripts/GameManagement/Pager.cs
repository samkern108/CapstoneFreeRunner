using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pager : MonoBehaviour {

	public GameObject pager;
	public Text pagerText;
	public static Pager self;

	void Start()
	{
		self = this;
	}

	public void DisplayPagerWithTextForTime(string text, float time)
	{
		pager.SetActive (true);
		pagerText.text = text;
		Invoke ("DisablePager", time);
	}

	public void DisablePager()
	{
		pager.SetActive (false);
		pagerText.text = "";
	}
}
