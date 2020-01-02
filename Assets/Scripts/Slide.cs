using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Slide : MonoBehaviour
{
	/*#region private variables
	private Vector3 screenPositionToAnimate;
	private SlidePiece PieceToAnimate;
	private SlidePiece PieceToSwap;
	private int toAnimateI, toAnimateJ;
	private bool swapLeft = true;
	private bool swapEnabled = false;
	private bool modeSelected = false;
	private GameState gameState;
	private GameObject[] go;
	private GameObject Pieces;
	private GameObject Frame;
	#endregion

	#region private serialized variables
	[SerializeField] private AspectRatioFitter fit;
	[SerializeField] private RawImage background;
	[SerializeField] private int ColumnNumber = 4;
	[SerializeField] private int RowNumber = 3;
	[SerializeField] private SlidePiece[,] Matrix;
	[SerializeField] private float AnimSpeed = 10f;
	[SerializeField] private GameObject piece_template;
	#endregion

	#region private static variables
	private static string externalFunctionName = "com.wolfattack.gallery.PuzzleFunctions";
	#endregion

    // Use this for initialization
    void Start()
	{	
		if (Application.platform == RuntimePlatform.Android)
		{
			using (Utilities.unityClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"))
			{
				Utilities.currentActivity = Utilities.unityClass.GetStatic<AndroidJavaObject> ("currentActivity");
			}
			using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (externalFunctionName))
			{
				if (Utilities.puzzleFunctionsClass != null)
				{
					Utilities.puzzleFunctionsClass.CallStatic ("setContext", Utilities.currentActivity);
				}
			}
			Utilities.ChangeOrientation (Orientation.HORIZONTAL);
		}

		Pieces = GameObject.FindGameObjectWithTag ("Pieces");
		Frame = GameObject.Find ("Frame");
		Frame.SetActive (false);

		swapLeft = true;
		Matrix = new SlidePiece[RowNumber, ColumnNumber];
		gameState = GameState.Start;
    }

	private void PredefinedPic()
	{
		Texture2D [] textures = Resources.LoadAll<Texture2D> ("Textures/Anime");
		Texture2D tex = textures [UnityEngine.Random.Range (0, textures.Length)];

		SliceTexture(tex);
		OneDToTwoD (tex);
	}

	private void SliceTexture(Texture2D tex)
	{
		if (go != null)
		{
			foreach(GameObject go_o in go)
			{
				Destroy (go_o);
			}
			go = null;
		}

		go = new GameObject[RowNumber * ColumnNumber];

		float vertStep = tex.height / RowNumber;
		float horiStep = tex.width / ColumnNumber;
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j] = (GameObject)GameObject.Instantiate (piece_template, Pieces.transform);
				Sprite s = Sprite.Create (tex, new Rect (j * horiStep, (RowNumber - 1 - i) * vertStep, horiStep, vertStep), new Vector2 (0.5f, 0.5f));
				go [i * ColumnNumber + j].GetComponent<SpriteRenderer> ().sprite = s;
				go [i * ColumnNumber + j].name = "piece-" + i + "-" + j + "";
			}
		}
	}

	private void OneDToTwoD(Texture2D tex)
	{
		float maxVertPoints = 10;
		float maxHorizPoints = 14;

		if (Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Utilities.consideredOrientation == Orientation.VERTICAL)
			{
				maxVertPoints = 8;
				maxHorizPoints = 6;
			}
		}
		float vertStepWorld = maxVertPoints / RowNumber;
		float horiStepWorld = maxHorizPoints / ColumnNumber;

		float vertStep = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		float horiStep = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

		float vertRatio = vertStepWorld / vertStep;
		float horiRatio = horiStepWorld / horiStep;
		float ratio  = Mathf.Min (vertRatio, horiRatio) * 0.8f;

		int width_frame = (int)(tex.width * ratio * 1.015f);
		int height_frame = (int)(tex.height * ratio * 1.015f);
		Texture2D tex2 = Instantiate(tex) as Texture2D;
		tex2.Resize(width_frame, height_frame);
		Frame.GetComponent<SpriteRenderer> ().sprite = Sprite.Create (tex2, new Rect (0, 0, tex2.width, tex2.height), new Vector2 (0.5f, 0.5f));
		Frame.SetActive (true);

		Matrix = new SlidePiece[RowNumber, ColumnNumber];
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j].transform.localScale = new Vector3 (go [i * ColumnNumber + j].transform.localScale.x * ratio, go [i * ColumnNumber + j].transform.localScale.x * ratio, 1);

				Vector3 point = new Vector3(
					horiStep * (j - (float)(ColumnNumber) / 2) * ratio,
					vertStep * (RowNumber - 1 - i - (float)(RowNumber) / 2) * ratio);
				go [i * ColumnNumber + j].transform.localPosition = point;

				//place relevant information
				Matrix[i, j] = new SlidePiece();
				Matrix[i, j].GameObject = go[i * ColumnNumber + j];
				Matrix[i, j].OriginalI = i; Matrix[i, j].OriginalJ = j;
				//add a box collider the the raycast to work properly
				if (Matrix[i, j].GameObject.GetComponent<BoxCollider2D>() == null)
					Matrix[i, j].GameObject.AddComponent<BoxCollider2D>();
			}
		}
		modeSelected = true;
		GC.Collect ();

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if ((float)tex.width <= (float)tex.height * 1.2f)
			{
				Utilities.ChangeOrientation (Orientation.VERTICAL);
			}
			else
			{
				Utilities.ChangeOrientation (Orientation.HORIZONTAL);
			}
		}
	}

    private void Shuffle()
    {
        //shuffle
		for (int i = 0; i < RowNumber; i++)
        {
			for (int j = 0; j < ColumnNumber; j++)
            {
				int random_i = UnityEngine.Random.Range(0, RowNumber);
				int random_j = UnityEngine.Random.Range(0, ColumnNumber);
                //swap'em
                Swap(i, j, random_i, random_j, true);
            }
        }
    }

	private void Swap(int i, int j, int random_i, int random_j, bool changeposition)
    {
        //temp piece, necessary for swapping
        SlidePiece temp = Matrix[i, j];
        Matrix[i, j] = Matrix[random_i, random_j];
        Matrix[random_i, random_j] = temp;

		if (changeposition)
		{
			Vector3 temppos = Matrix [i, j].GameObject.transform.localPosition;
			Matrix [i, j].GameObject.transform.localPosition = Matrix [random_i, random_j].GameObject.transform.localPosition;
			Matrix [random_i, random_j].GameObject.transform.localPosition = temppos;
		}

        //set the required properties
		Matrix[i, j].CurrentI = i;
		Matrix[i, j].CurrentJ = j;
        Matrix[random_i, random_j].CurrentI = random_i;
        Matrix[random_i, random_j].CurrentJ = random_j;
    }


    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Application.Quit ();
			}
		}

        switch (gameState)
        {
            case GameState.Start:
                break;
            case GameState.Playing:
                CheckPieceInput();
                break;
            case GameState.Animating:
                AnimateMovement(PieceToAnimate, Time.deltaTime);
                CheckIfAnimationEnded();
                break;
            case GameState.End:
                if (Input.GetMouseButtonUp(0))
                {
					//reload
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                break;
            default:
                break;
        }
    }
   
    /// <summary>
    /// boring UI, waiting for uGUI framework :)
    /// </summary>
    void OnGUI()
	{
		int offsetX = 0;
		int offsetY = 0;
		if (Utilities.consideredOrientation == Orientation.HORIZONTAL)
		{
			offsetX = 0;
			offsetY = 50;
		}
		else if (Utilities.consideredOrientation == Orientation.VERTICAL)
		{
			offsetX = 100;
			offsetY = 0;
		}

		if (GUI.Button (new Rect (Screen.width - 100, Screen.height - 50, 100, 50), "Back"))
		{
			SceneManager.LoadScene ("mainmenu");
		}
        
		switch (gameState)
		{
			case GameState.Start:
				int order = -1;
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Start") && modeSelected)
				{
					go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = false;
					Shuffle ();
					gameState = GameState.Playing;
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Predefined"))
				{
					PredefinedPic();
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Gallery"))
				{
					Utilities.lastOrientation = Screen.orientation;
					if (Application.platform == RuntimePlatform.Android)
					{
						Debug.Log ("OnGUI before call");
						using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (externalFunctionName))
						{
							if (Utilities.puzzleFunctionsClass != null)
							{
								Utilities.puzzleFunctionsClass.CallStatic ("setContext", Utilities.currentActivity);
								Utilities.puzzleFunctionsClass.CallStatic ("openGallery");
							}
						}
					}
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Camera"))
				{
					using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (externalFunctionName))
					{
						if (Utilities.puzzleFunctionsClass != null)
						{
							Utilities.puzzleFunctionsClass.CallStatic ("setContext", Utilities.currentActivity);
							Utilities.puzzleFunctionsClass.CallStatic ("openCamera");
						}
					}
				}
				break;
			case GameState.Playing:
			case GameState.Animating:
				if (swapLeft && GUI.Button (new Rect (offsetX, 50 + offsetY, 100, 50), (swapEnabled?"Swap NOW":"Click to Swap!")))
				{
					swapEnabled = !swapEnabled;
				}
				if (GUI.Button (new Rect (0, 50, 100, 50), "Reset"))
				{
					modeSelected = false;

					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
				if (swapLeft && swapEnabled)
				{
					GUI.Label (new Rect (0, 0, 100, 100), "Swap any two of your choice!");
				}
				else
				{
					PieceToSwap = null;
				}
                break;
            case GameState.End:
                GUI.Label(new Rect(0, 0, 100, 100), "Congrats, tap to start over!");
                break;
            default:
                break;
        }
	}

	public void onPhotoPick(string photoPath)
	{
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists (photoPath))
		{
			fileData = File.ReadAllBytes (photoPath);
			tex = new Texture2D (2, 2);
			tex.LoadImage (fileData);

			if (tex != null)
			{
				SliceTexture (tex);
				OneDToTwoD (tex);
			}
			else
			{
				Screen.orientation = Utilities.lastOrientation;
			}
		}
		else
		{
			Screen.orientation = Utilities.lastOrientation;
		}
	}

    void CheckPieceInput()
    {
		if (Input.GetMouseButtonUp (0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction);

			//check if a piece was hit
			if (hit.collider != null)
			{
				string name = hit.collider.gameObject.name;
				string [] parts = name.Split ('-');
				int iPart = int.Parse (parts [1]);
				int jPart = int.Parse (parts [2]);

				int iFound = -1, jFound = -1;
				//find which one was hit, in our 2D array
				//there must be a better way than this one
				for (int i = 0; i < RowNumber; i++)
				{
					if (iFound != -1)
						break;
					for (int j = 0; j < ColumnNumber; j++)
					{
						if (iFound != -1)
							break;
						if (Matrix [i, j].OriginalI == iPart
						     && Matrix [i, j].OriginalJ == jPart)
						{
							iFound = i;
							jFound = j;
						}
					}
				}

				if (swapEnabled)
				{
					if (swapLeft)
					{
						if (PieceToSwap == null)
						{
							PieceToSwap = Matrix [iFound, jFound];
						}
						else
						{
							Swap (PieceToSwap.CurrentI, PieceToSwap.CurrentJ, iFound, jFound, true);
							CheckForVictory ();
							PieceToSwap = null;
							swapLeft = false;
							swapEnabled = false;
						}
					}
				}
				else
				{
					//check for the null piece, taking into account the game bounds
					bool pieceFound = false;
					if (iFound > 0 && !Matrix [iFound - 1, jFound].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound - 1;
						toAnimateJ = jFound;
					}
					else if (jFound > 0 && !Matrix [iFound, jFound - 1].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound;
						toAnimateJ = jFound - 1;
					}
					else if (iFound < RowNumber - 1 && !Matrix [iFound + 1, jFound].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound + 1;
						toAnimateJ = jFound;
					}
					else if (jFound < ColumnNumber - 1 && !Matrix [iFound, jFound + 1].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound;
						toAnimateJ = jFound + 1;
					}

					if (pieceFound)
					{
						//get the coordinates of the empty object
						screenPositionToAnimate = Matrix [toAnimateI, toAnimateJ].GameObject.transform.localPosition;
						Matrix [toAnimateI, toAnimateJ].GameObject.transform.localPosition = Matrix [iFound, jFound].GameObject.transform.localPosition;
						PieceToAnimate = Matrix [iFound, jFound];
						gameState = GameState.Animating;
					}
				}
			}
		}
    }

    private void AnimateMovement(SlidePiece toMove,  float time)
    {
        //animate it
        //Lerp could also be used, but I prefer the MoveTowards approach :)
		toMove.GameObject.transform.localPosition = Vector2.MoveTowards(toMove.GameObject.transform.localPosition, 
        screenPositionToAnimate , time * AnimSpeed);
    }

    /// <summary>
    /// A simple check to see if the animation has finished
    /// </summary>
    private void CheckIfAnimationEnded()
    {
		if(Vector2.Distance(PieceToAnimate.GameObject.transform.localPosition, screenPositionToAnimate) < 0.1f)
        {
			PieceToAnimate.GameObject.transform.localPosition = screenPositionToAnimate;
            //make sure they swap, exchange positions and stuff
			Swap(PieceToAnimate.CurrentI, PieceToAnimate.CurrentJ, toAnimateI, toAnimateJ, false);
			//set the required properties
			Matrix[PieceToAnimate.CurrentI, PieceToAnimate.CurrentI].CurrentJ = PieceToAnimate.CurrentI;
			Matrix[PieceToAnimate.CurrentI, PieceToAnimate.CurrentJ].CurrentJ = PieceToAnimate.CurrentJ;
			Matrix[toAnimateI, toAnimateJ].CurrentI = toAnimateI;
			Matrix[toAnimateI, toAnimateJ].CurrentJ = toAnimateJ;
            gameState = GameState.Playing;
            //check if the use has won
            CheckForVictory();
        }
    }

    private void CheckForVictory()
    {
        //dual loop to check the object's properties
        for (int i = 0; i < RowNumber; i++)
        {
            for (int j = 0; j < ColumnNumber; j++)
            {
				if (Matrix [i, j].CurrentI != Matrix [i, j].OriginalI ||
					Matrix [i, j].CurrentJ != Matrix [i, j].OriginalJ)
				{
					return;
				}
            }
        }

		//if we did not return, then we've won!
		gameState = GameState.End;

		go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = true;
	}*/
}
