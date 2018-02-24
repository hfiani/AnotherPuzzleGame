using UnityEngine;
using System.Collections;
using System.IO;
using System;

public enum Orientation 
{
	VERTICAL,
	HORIZONTAL
}

public static class Utilities
{
	public static AndroidJavaClass puzzleFunctionsClass;
	public static AndroidJavaClass unityClass;
	public static AndroidJavaObject currentActivity;
	public static AndroidJavaObject gallery;

	public static ScreenOrientation lastOrientation;
	public static Orientation consideredOrientation;
	
	public static void ChangeOrientation(Orientation orientation)
	{
		consideredOrientation = orientation;
		if (orientation == Orientation.HORIZONTAL)
		{
			Screen.orientation = ScreenOrientation.Landscape;
		}
		else
		{
			Screen.orientation = ScreenOrientation.Portrait;
		}
	}
}
