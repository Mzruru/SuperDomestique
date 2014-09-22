using UnityEngine;
using System.Collections;

public abstract class SkyColoursController : MonoBehaviour {

	[Tooltip("A GameObject which has the SkyColours behaviour")]
	public SkyColours
		skyColours;
	[Tooltip("Where in the sky to get the colour from (0 = lower colour, 1 = upper colour)"), Range(0,1)]
	public float
		skyColourPosition = 0.5f;
	
	// Use this for initialization
	void Start ()
	{
		if (skyColours == null) {
			Debug.Log ("SkyColoursLighting: You must attach a GameObject which has the skyColours behaviour");
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		DualColour colours = skyColours.GetColoursForCurrentTime ();
		UpdateValues(colours);
	}
	
	protected abstract void UpdateValues(DualColour colours);
}
