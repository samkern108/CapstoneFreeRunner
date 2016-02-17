using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsTracker : MonoBehaviour {

	public Text DeliveredText;
	public static StatsTracker self;
	public static int papersDelivered = 0;

	void Start () {
		self = this;
	}

	public void DeliverPaper()
	{
		papersDelivered++;
		DeliveredText.text = papersDelivered + "";
	}

	public void ResetDelivered()
	{
		papersDelivered = 0;
		DeliveredText.text = papersDelivered + "";
	}
}
