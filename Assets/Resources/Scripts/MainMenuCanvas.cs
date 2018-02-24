using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		Utilities.ChangeOrientation (Orientation.HORIZONTAL);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void LoadJigsaw()
	{
		SceneManager.LoadScene ("jigsaw");
	}

	public void LoadSlide()
	{
		SceneManager.LoadScene ("slide");
	}
}
