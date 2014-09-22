using UnityEngine;
using System.Collections;

public class SkyColoursLightingController : SkyColoursController
{	
		// Update is called once per frame
		override protected void UpdateValues (DualColour colours)
		{
				gameObject.light.color = Color.Lerp (colours.lower, colours.upper, skyColourPosition);
		}
}
