using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TouchEvent : UnityEvent<Touch>{}

public class TouchController : MonoBehaviour {

	public TouchEvent TouchEvent;
	
	// Update is called once per frame
	private void Update () 
	{
		foreach (var touch in Input.touches)
		{
			TouchEvent.Invoke(touch);
		}	
	}
}

