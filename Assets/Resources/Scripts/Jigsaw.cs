using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Jigsaw : MonoBehaviour
{
	#region private variables
	/*private Vector3 screenPositionToAnimate;
	private int toAnimateI, toAnimateJ;
	private GameState gameState;
	private GameMode gameMode;
	private GameObject[] go;
	private GameObject Pieces;
	private GameObject Frame;
	private float vertStep;
	private float horiStep;
	private float extraVertStep;
	private float extraHoriStep;
	//jigsaw only
	private Vector3[] snappingPositions;
	private Piece PieceToAnimateJigsaw;
	// slide only
	private Piece PieceToAnimateSlide;
	private Piece PieceToSwapSlide;
	private bool swapLeft = true;
	private bool swapEnabled = false;
	private Piece[,] MatrixJigsaw;
	private Piece[,] MatrixSlide;*/
	private Game game;
	private bool modeSelected = false;
	#endregion

	#region private serialized variables
	[SerializeField] private int ColumnNumber = 4;
	[SerializeField] private int RowNumber = 3;
	[SerializeField] private GameObject piece_template_jigsaw;
	[SerializeField] private GameObject piece_template_slide;
	#endregion

	#region private static variables
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
			using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (Game.externalFunctionName))
			{
				if (Utilities.puzzleFunctionsClass != null)
				{
					Utilities.puzzleFunctionsClass.CallStatic ("setInfos", Utilities.currentActivity, name, "onPhotoPick");
				}
			}
			Utilities.ChangeOrientation (Orientation.HORIZONTAL);
		}

		Game.objectName = name;

		Game.Init ();
    }

	private void PredefinedPic()
	{
		Texture2D [] textures = Resources.LoadAll<Texture2D> ("Textures/Anime");
		Texture2D tex = textures [UnityEngine.Random.Range (0, textures.Length)];

		/*switch(gameMode)
		{
			case GameMode.Jigsaw:
				PrepareJigsaw (tex);
				SliceTexture (tex);
				OneDToTwoDJigsaw (tex);
				break;
			case GameMode.Slide:
				PrepareSlide (tex);
				SliceTexture (tex);
				OneDToTwoDSlide (tex);
				break;
		}*/
		if (game != null)
		{
			game.Prepare (tex);
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

			if (tex != null && game != null)
			{
				/*switch(gameMode)
				{
					case GameMode.Jigsaw:
						PrepareJigsaw (tex);
						SliceTexture (tex);
						OneDToTwoDJigsaw (tex);
						break;
					case GameMode.Slide:
						PrepareSlide (tex);
						SliceTexture (tex);
						OneDToTwoDSlide (tex);
						break;
				}*/

				game.Prepare (tex);
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

	/*private void PrepareJigsaw(Texture2D tex)
	{
		if (go != null)
		{
			foreach(GameObject go_o in go)
			{
				Destroy (go_o);
			}
		}
		go = new GameObject[RowNumber * ColumnNumber];

		float tex_width = tex.width - 50f;
		float tex_height = tex.height - 50f;
		Pieces.transform.localScale = new Vector3(tex_width / tex.width, tex_height / tex.height, 0);

		vertStep = 4 * tex.height / (1 + 4 * RowNumber);
		horiStep = 4 * tex.width / (1 + 4 * ColumnNumber);

		extraVertStep = tex.height / (1 + 4 * RowNumber);
		extraHoriStep = tex.width / (1 + 4 * ColumnNumber);
	}*/

	/*private void PrepareSlide(Texture2D tex)
	{
		if (go != null)
		{
			foreach(GameObject go_o in go)
			{
				Destroy (go_o);
			}
		}
		go = new GameObject[RowNumber * ColumnNumber];

		float tex_width = tex.width - 50f;
		float tex_height = tex.height - 50f;
		Pieces.transform.localScale = new Vector3(tex_width / tex.width, tex_height / tex.height, 0);

		vertStep = tex.height / RowNumber;
		horiStep = tex.width / ColumnNumber;

		extraVertStep = 0;
		extraHoriStep = 0;
	}*/

	/*private void SliceTexture(Texture2D tex)
	{
		GameObject piece_template = new GameObject ();
		switch (gameMode)
		{
			case GameMode.Jigsaw:
				piece_template = piece_template_jigsaw;
				break;
			case GameMode.Slide:
				piece_template = piece_template_slide;
				break;
		}
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j] = (GameObject)GameObject.Instantiate (piece_template, Pieces.transform);
				Sprite s = Sprite.Create (tex, new Rect (j * horiStep, (RowNumber - 1 - i) * vertStep, horiStep + extraHoriStep, vertStep + extraVertStep), new Vector2 (0.5f, 0.5f));

				SpriteRenderer sr = go [i * ColumnNumber + j].GetComponent<SpriteRenderer> ();
				sr.sprite = s;
				//sr.sortingLayerName = (i * ColumnNumber + j + 1).ToString ();
				//sr.sortingLayerID = SortingLayer.NameToID ((i * ColumnNumber + j + 1).ToString ());
				sr.sortingOrder = i * ColumnNumber + j + 1;

				go [i * ColumnNumber + j].name = "piece-" + i + "-" + j;
			}
		}
	}*/

	/*private void OneDToTwoDJigsaw(Texture2D tex)
	{
		Texture2D texture_big = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_random_big_0");
		texture_big.name = "puzzle_random_big_0";
		Texture2D texture_small = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_random_small_0");
		texture_small.name = "puzzle_random_small_0";

		Texture2D [] textures = {texture_big, texture_small};

		Texture2D texture_corner_up = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_corner_up");
		Texture2D texture_corner_down = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_corner_down");
		Texture2D texture_edge = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_edge");

		float mask_width = texture_big.width / 100;
		float mask_height = texture_big.height / 100;

		float maxVertPoints = 10;
		float maxHorizPoints = 14;

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if ((float)tex.width <= (float)tex.height * 1.2f)
			{
				maxVertPoints = 8;
				maxHorizPoints = 6;
			}
		}
		float vertStepWorld = maxVertPoints / RowNumber;
		float horiStepWorld = maxHorizPoints / ColumnNumber;

		float vertStepLocal = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		float horiStepLocal = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

		float vertRatio = vertStepWorld / vertStepLocal;
		float horiRatio = horiStepWorld / horiStepLocal;
		float ratio  = Mathf.Min (vertRatio, horiRatio);

		int width_frame = (int)(tex.width * ratio * 1.015f);
		int height_frame = (int)(tex.height * ratio * 1.015f);
		Texture2D tex2 = Instantiate(tex) as Texture2D;
		tex2.Resize(width_frame, height_frame);
		Frame.GetComponent<SpriteRenderer> ().sprite = Sprite.Create (tex2, new Rect (0, 0, tex2.width, tex2.height), new Vector2 (0.5f, 0.5f));
		Frame.SetActive (true);

		MatrixJigsaw = new JigsawPiece[RowNumber, ColumnNumber];
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j].transform.localScale = new Vector3 (go [i * ColumnNumber + j].transform.localScale.x * ratio, go [i * ColumnNumber + j].transform.localScale.y * ratio, 1);

				Vector3 point = new Vector3(
					horiStepLocal * (j - (float)(ColumnNumber - 1) / 2) * ratio * 0.8f,
					vertStepLocal * (RowNumber - 1 - i - (float)(RowNumber - 1) / 2) * ratio * 0.8f);
				go [i * ColumnNumber + j].transform.localPosition = point;
				snappingPositions [i * ColumnNumber + j] = point;

				Texture2D tex_left;
				Texture2D tex_up;
				Texture2D tex_right;
				Texture2D tex_down;

				Texture2D tex_left_complementary = null;
				Texture2D tex_up_complementary = null;
				Texture2D tex_right_complementary = null;
				Texture2D tex_down_complementary = null;

				if (i == 0 && j == 0)
				{
					tex_left = texture_corner_up;
					tex_up = texture_corner_down;
					tex_right = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_down = textures [UnityEngine.Random.Range (0, textures.Length)];
				}
				else if (i == 0 && j == ColumnNumber - 1)
				{
					tex_left = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_up = texture_corner_up;
					tex_right = texture_corner_down;
					tex_down = textures [UnityEngine.Random.Range (0, textures.Length)];
				}
				else if (i == RowNumber - 1 && j == 0)
				{
					tex_left = texture_corner_down;
					tex_up = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_right = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_down = texture_corner_up;
				}
				else if (i == RowNumber - 1 && j == ColumnNumber - 1)
				{
					tex_left = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_up = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_right = texture_corner_up;
					tex_down = texture_corner_down;
				}
				else if (i == 0)
				{
					tex_left = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_up = texture_edge;
					tex_right = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_down = textures [UnityEngine.Random.Range (0, textures.Length)];
				}
				else if (j == 0)
				{
					tex_left = texture_edge;
					tex_up = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_right = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_down = textures [UnityEngine.Random.Range (0, textures.Length)];
				}
				else if (i == RowNumber - 1)
				{
					tex_left = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_up = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_right = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_down = texture_edge;
				}
				else if (j == ColumnNumber - 1)
				{
					tex_left = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_up = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_right = texture_edge;
					tex_down = textures [UnityEngine.Random.Range (0, textures.Length)];
				}
				else
				{
					tex_left = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_up = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_right = textures [UnityEngine.Random.Range (0, textures.Length)];
					tex_down = textures [UnityEngine.Random.Range (0, textures.Length)];
				}

				Texture2D[] textures_to_fill;

				textures_to_fill = new Texture2D[] {tex_left, tex_up, tex_right, tex_down};
				FillSquare(go [(i + 0) * ColumnNumber + (j + 0)], textures_to_fill);

				// after we fill the piece we fill its other complementary side
				if(j < ColumnNumber - 1)
				{
					tex_left_complementary = tex_right.name.Contains("big")?(textures[Array.IndexOf(textures, tex_right) + 1]):(textures[Array.IndexOf(textures, tex_right) - 1]);

					textures_to_fill = new Texture2D[] {tex_left_complementary, null, null, null};
					FillSquare(go [(i + 0) * ColumnNumber + (j + 1)], textures_to_fill);
				}
				if(i < RowNumber - 1)
				{
					tex_up_complementary = tex_down.name.Contains("big")?(textures[Array.IndexOf(textures, tex_down) + 1]):(textures[Array.IndexOf(textures, tex_down) - 1]);

					textures_to_fill = new Texture2D[] {null, tex_up_complementary, null, null};
					FillSquare(go [(i + 1) * ColumnNumber + (j + 0)], textures_to_fill);
				}

				go [i * ColumnNumber + j].transform.GetChild (0).localScale = new Vector3 (horiStepLocal / mask_width, vertStepLocal / mask_height);
				go [i * ColumnNumber + j].transform.GetChild (1).localScale = new Vector3 (vertStepLocal / mask_width, horiStepLocal / mask_height);
				go [i * ColumnNumber + j].transform.GetChild (2).localScale = new Vector3 (horiStepLocal / mask_width, vertStepLocal / mask_height);
				go [i * ColumnNumber + j].transform.GetChild (3).localScale = new Vector3 (vertStepLocal / mask_width, horiStepLocal / mask_height);

				int sortingOrder = go [i * ColumnNumber + j].GetComponent<SpriteRenderer> ().sortingOrder;
				go [i * ColumnNumber + j].transform.GetChild (0).GetComponent<SpriteMask> ().frontSortingOrder = sortingOrder;
				go [i * ColumnNumber + j].transform.GetChild (1).GetComponent<SpriteMask> ().frontSortingOrder = sortingOrder;
				go [i * ColumnNumber + j].transform.GetChild (2).GetComponent<SpriteMask> ().frontSortingOrder = sortingOrder;
				go [i * ColumnNumber + j].transform.GetChild (3).GetComponent<SpriteMask> ().frontSortingOrder = sortingOrder;
				//add a box collider the the raycast to work properly
				if (go [i * ColumnNumber + j].GetComponent<BoxCollider2D>() == null)
					go [i * ColumnNumber + j].AddComponent<BoxCollider2D>();
				go [i * ColumnNumber + j].GetComponent<BoxCollider2D>().isTrigger = true;

				//place relevant information
				MatrixJigsaw[i, j] = new JigsawPiece();
				MatrixJigsaw[i, j].GameObject = go[i * ColumnNumber + j];
				MatrixJigsaw[i, j].OriginalI = i;
				MatrixJigsaw[i, j].OriginalJ = j;
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
	}*/

	/*private void OneDToTwoDSlide(Texture2D tex)
	{
		float maxVertPoints = 10;
		float maxHorizPoints = 14;

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if ((float)tex.width <= (float)tex.height * 1.2f)
			{
				maxVertPoints = 8;
				maxHorizPoints = 6;
			}
		}
		float vertStepWorld = maxVertPoints / RowNumber;
		float horiStepWorld = maxHorizPoints / ColumnNumber;

		float vertStepLocal = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		float horiStepLocal = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

		float vertRatio = vertStepWorld / vertStepLocal;
		float horiRatio = horiStepWorld / horiStepLocal;
		float ratio  = Mathf.Min (vertRatio, horiRatio) * 0.8f;

		int width_frame = (int)(tex.width * ratio * 1.015f);
		int height_frame = (int)(tex.height * ratio * 1.015f);
		Texture2D tex2 = Instantiate(tex) as Texture2D;
		tex2.Resize(width_frame, height_frame);
		Frame.GetComponent<SpriteRenderer> ().sprite = Sprite.Create (tex2, new Rect (0, 0, tex2.width, tex2.height), new Vector2 (0.5f, 0.5f));
		Frame.SetActive (true);

		MatrixSlide = new SlidePiece[RowNumber, ColumnNumber];
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j].transform.localScale = new Vector3 (go [i * ColumnNumber + j].transform.localScale.x * ratio, go [i * ColumnNumber + j].transform.localScale.x * ratio, 1);

				Vector3 point = new Vector3(
					horiStepLocal * (j - (float)(ColumnNumber) / 2 + 0.5f) * ratio,
					vertStepLocal * (RowNumber - 0.5f - i - (float)(RowNumber) / 2) * ratio);
				go [i * ColumnNumber + j].transform.localPosition = point;

				//place relevant information
				MatrixSlide[i, j] = new SlidePiece();
				MatrixSlide[i, j].GameObject = go[i * ColumnNumber + j];
				MatrixSlide[i, j].OriginalI = i;
				MatrixSlide[i, j].OriginalJ = j;
				//add a box collider the the raycast to work properly
				if (MatrixSlide[i, j].GameObject.GetComponent<BoxCollider2D>() == null)
					MatrixSlide[i, j].GameObject.AddComponent<BoxCollider2D>();
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
	}*/

    /*private void ShuffleJigsaw()
	{
		float vertStep = Mathf.Abs(snappingPositions[1 * ColumnNumber + 0].y - snappingPositions[0 * ColumnNumber + 0].y);
		float horiStep = Mathf.Abs(snappingPositions[0 * ColumnNumber + 1].x - snappingPositions[0 * ColumnNumber + 0].x);

		float top = snappingPositions [0].y + vertStep / 2 + Pieces.transform.position.y;
		float left = snappingPositions [0].x - horiStep / 2 + Pieces.transform.position.x;
		float bottom = snappingPositions [ColumnNumber * RowNumber - 1].y - vertStep / 2 + Pieces.transform.position.y;
		float right = snappingPositions [ColumnNumber * RowNumber - 1].x - horiStep / 2 + Pieces.transform.position.x;

		float viewportTop = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, 0)).y;
		float viewportLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, 0)).x;
		float viewportBottom = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, 0)).y;
		float viewportRight = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, 0)).x;

        //shuffle
		for (int i = 0; i < RowNumber; i++)
        {
			for (int j = 0; j < ColumnNumber; j++)
			{
				float random_x = 0;
				float random_y = 0;
				if (UnityEngine.Random.Range (0, 2) == 0)
				{
					random_x = UnityEngine.Random.Range (viewportLeft, left - horiStep / 2);
					random_y = UnityEngine.Random.Range (viewportBottom, top);
				}
				else
				{
					random_x = UnityEngine.Random.Range (right + horiStep / 2, viewportRight);
					random_y = UnityEngine.Random.Range (viewportBottom, top);
				}
				//scramble'em
				MatrixJigsaw[i,j].GameObject.transform.position = new Vector3(random_x, random_y, 0);
            }
        }
	}*/

	/*private void Snap(int i, int j)
	{
		float vertStep = Mathf.Abs(snappingPositions[1 * ColumnNumber + 0].y - snappingPositions[0 * ColumnNumber + 0].y);
		float horiStep = Mathf.Abs(snappingPositions[0 * ColumnNumber + 1].x - snappingPositions[0 * ColumnNumber + 0].x);

		for (int k = 0; k < snappingPositions.Length; k++)
		{
			if (Mathf.Abs(MatrixJigsaw [i, j].GameObject.transform.localPosition.x - snappingPositions[k].x) <= horiStep / 2 &&
				Mathf.Abs(MatrixJigsaw [i, j].GameObject.transform.localPosition.y - snappingPositions[k].y) <= vertStep / 2)
			{
				MatrixJigsaw [i, j].GameObject.transform.localPosition = snappingPositions [k];
			}
		}
	}*/

	/*private void ShuffleSlide()
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
	}*/

	/*private void Swap(int i, int j, int random_i, int random_j, bool changeposition)
	{
		//temp piece, necessary for swapping
		SlidePiece temp = MatrixSlide[i, j];
		MatrixSlide[i, j] = MatrixSlide[random_i, random_j];
		MatrixSlide[random_i, random_j] = temp;

		if (changeposition)
		{
			Vector3 temppos = MatrixSlide [i, j].GameObject.transform.localPosition;
			MatrixSlide [i, j].GameObject.transform.localPosition = MatrixSlide [random_i, random_j].GameObject.transform.localPosition;
			MatrixSlide [random_i, random_j].GameObject.transform.localPosition = temppos;
		}

		//set the required properties
		MatrixSlide[i, j].CurrentI = i;
		MatrixSlide[i, j].CurrentJ = j;
		MatrixSlide[random_i, random_j].CurrentI = random_i;
		MatrixSlide[random_i, random_j].CurrentJ = random_j;
	}*/

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

		switch (Game.GetGameState())
        {
            case GameState.Start:
                break;
			case GameState.Playing:
				if (game != null)
				{
					game.CheckPieceInput ();
				}
                break;
			case GameState.Animating:
				if (game != null)
				{
					game.AnimateMovement();
				}
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
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}

		switch (Game.GetGameState())
		{
			case GameState.Menu:
				int order = -1;
				if (order++ >= -1 && Game.GetGameMode() == GameMode.None && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Jigsaw"))
				{
					/*gameState = GameState.Start;
					gameMode = GameMode.Jigsaw;*/
					Game.SetGameState (GameState.Start);
					game = new JigsawGame (piece_template_jigsaw, ColumnNumber, RowNumber);
				}
				if (order++ >= -1 && Game.GetGameMode() == GameMode.None && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Slide"))
				{
					/*gameState = GameState.Start;
					gameMode = GameMode.Slide;*/
					Game.SetGameState (GameState.Start);
					game = new SlideGame (piece_template_slide, ColumnNumber, RowNumber);
				}
				break;
			case GameState.Start:
				order = -1;
				if (order++ >= -1 && modeSelected && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Start"))
				{
					/*switch(gameMode)
					{
						case GameMode.Jigsaw:
							ShuffleJigsaw ();
							break;
						case GameMode.Slide:
							go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = false;
							ShuffleSlide ();
							break;
					}*/
					if (game != null)
					{
						game.Launch ();
					}

					Game.SetGameState(GameState.Playing);
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Predefined"))
				{
					PredefinedPic();
					modeSelected = true;
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Gallery"))
				{
					Utilities.lastOrientation = Screen.orientation;
					if (Application.platform == RuntimePlatform.Android)
					{
						Debug.Log ("OnGUI before call");
						using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (Game.externalFunctionName))
						{
							if (Utilities.puzzleFunctionsClass != null)
							{
								Utilities.puzzleFunctionsClass.CallStatic ("setInfos", Utilities.currentActivity, name, "onPhotoPick");
								Utilities.puzzleFunctionsClass.CallStatic ("openGallery");
							}
						}
					}
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Camera"))
				{
					//cameraStart ();
					using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (Game.externalFunctionName))
					{
						if (Utilities.puzzleFunctionsClass != null)
						{
							Utilities.puzzleFunctionsClass.CallStatic ("setInfos", Utilities.currentActivity, name, "onPhotoPick");
							Utilities.puzzleFunctionsClass.CallStatic ("openCamera");
						}
					}

					Utilities.lastOrientation = Screen.orientation;
					Utilities.ChangeOrientation (Orientation.HORIZONTAL);
				}
				break;
			case GameState.Playing:
			case GameState.Animating:
				if (GUI.Button (new Rect (0, 50, 100, 50), "Reset"))
				{
					modeSelected = false;

					SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				}
				/*switch (gameMode)
				{
					case GameMode.Jigsaw:
						float vertStep = Mathf.Abs (snappingPositions [1 * ColumnNumber + 0].y - snappingPositions [0 * ColumnNumber + 0].y);
						float horiStep = Mathf.Abs (snappingPositions [0 * ColumnNumber + 1].x - snappingPositions [0 * ColumnNumber + 0].x);

						float top = snappingPositions [0].y + vertStep / 2 + Pieces.transform.position.y;
						float left = snappingPositions [0].x - horiStep / 2 + Pieces.transform.position.x;
						break;
					case GameMode.Slide:
						if (swapLeft && GUI.Button (new Rect (offsetX, 50 + offsetY, 100, 50), (swapEnabled?"Swap NOW":"Click to Swap!")))
						{
							swapEnabled = !swapEnabled;
						}
						if (swapLeft && swapEnabled)
						{
							GUI.Label (new Rect (0, 0, 100, 100), "Swap any two of your choice!");
						}
						else
						{
							PieceToSwapSlide = null;
						}
						break;
				}*/
				if (game != null)
				{
					game.UpdateGUIAnimating (offsetX, offsetY);
				}
				break;
            case GameState.End:
                GUI.Label(new Rect(0, 0, 100, 100), "Congrats, tap to start over!");
                break;
            default:
                break;
        }
	}

    /*void CheckPieceInput()
	{
		switch (gameMode)
		{
			case GameMode.Jigsaw:
				if (Input.GetMouseButtonDown (0) || Input.touchCount > 0)
				{
					Ray ray = new Ray ();
					if (Input.touchCount > 0)
					{
						ray = Camera.main.ScreenPointToRay (Input.touches [0].position);
					}
					else
					{
						ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					}
					RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction);

					//check if a piece was hit
					if (hit.collider != null)
					{
						string name = hit.collider.gameObject.name;
						string[] parts = name.Split ('-');
						int iPart = int.Parse (parts [1]);
						int jPart = int.Parse (parts [2]);

						PieceToAnimateJigsaw = MatrixJigsaw [iPart, jPart];
						gameState = GameState.Animating;
					}
				}
				break;
			case GameMode.Slide:
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
								if (MatrixSlide [i, j].OriginalI == iPart
									&& MatrixSlide [i, j].OriginalJ == jPart)
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
								if (PieceToSwapSlide == null)
								{
									PieceToSwapSlide = MatrixSlide [iFound, jFound];
								}
								else
								{
									Swap (PieceToSwapSlide.CurrentI, PieceToSwapSlide.CurrentJ, iFound, jFound, true);
									CheckForVictory ();
									PieceToSwapSlide = null;
									swapLeft = false;
									swapEnabled = false;
								}
							}
						}
						else
						{
							//check for the null piece, taking into account the game bounds
							bool pieceFound = false;
							if (iFound > 0 && !MatrixSlide [iFound - 1, jFound].GameObject.GetComponent<SpriteRenderer> ().enabled)
							{
								pieceFound = true;
								toAnimateI = iFound - 1;
								toAnimateJ = jFound;
							}
							else if (jFound > 0 && !MatrixSlide [iFound, jFound - 1].GameObject.GetComponent<SpriteRenderer> ().enabled)
							{
								pieceFound = true;
								toAnimateI = iFound;
								toAnimateJ = jFound - 1;
							}
							else if (iFound < RowNumber - 1 && !MatrixSlide [iFound + 1, jFound].GameObject.GetComponent<SpriteRenderer> ().enabled)
							{
								pieceFound = true;
								toAnimateI = iFound + 1;
								toAnimateJ = jFound;
							}
							else if (jFound < ColumnNumber - 1 && !MatrixSlide [iFound, jFound + 1].GameObject.GetComponent<SpriteRenderer> ().enabled)
							{
								pieceFound = true;
								toAnimateI = iFound;
								toAnimateJ = jFound + 1;
							}

							if (pieceFound)
							{
								//get the coordinates of the empty object
								screenPositionToAnimate = MatrixSlide [toAnimateI, toAnimateJ].GameObject.transform.localPosition;
								MatrixSlide [toAnimateI, toAnimateJ].GameObject.transform.localPosition = MatrixSlide [iFound, jFound].GameObject.transform.localPosition;
								PieceToAnimateSlide = MatrixSlide [iFound, jFound];
								gameState = GameState.Animating;
							}
						}
					}
				}
				break;
		}
    }

    private void AnimateMovement()
	{
		switch (gameMode)
		{
			case GameMode.Jigsaw:
        		//animate it
				if (Input.GetMouseButton (0) || Input.touchCount > 0)
				{
					float distance_to_screen = Camera.main.WorldToScreenPoint (PieceToAnimateJigsaw.GameObject.transform.position).z;
					Vector3 pos_move;
					if (Input.touchCount > 0)
					{
						pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.touches [0].position.x, Input.touches [0].position.y, distance_to_screen));
					}
					else
					{
						pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
					}
					PieceToAnimateJigsaw.GameObject.transform.position = new Vector3 (pos_move.x, pos_move.y, PieceToAnimateJigsaw.GameObject.transform.position.z);
				}
				break;
			case GameMode.Slide:
				//animate it
				//Lerp could also be used, but I prefer the MoveTowards approach :)
				PieceToAnimateSlide.GameObject.transform.localPosition = Vector2.MoveTowards 
					(
						PieceToAnimateSlide.GameObject.transform.localPosition, 
						screenPositionToAnimate, Time.deltaTime * AnimSpeed
					);
				break;
		}
	}

    /// <summary>
    /// A simple check to see if the animation has finished
    /// </summary>
    private void CheckIfAnimationEnded()
    {
		switch (gameMode)
		{
			case GameMode.Jigsaw:
				if (Input.GetMouseButtonUp (0))
				{
					//make sure they swap, exchange positions and stuff
					Snap (PieceToAnimateJigsaw.OriginalI, PieceToAnimateJigsaw.OriginalJ);
					//set the required properties
					gameState = GameState.Playing;
					PieceToAnimateJigsaw = null;
					//check if the use has won
					CheckForVictory ();
				}
				break;
			case GameMode.Slide:
				if(Vector2.Distance(PieceToAnimateSlide.GameObject.transform.localPosition, screenPositionToAnimate) < 0.1f)
				{
					PieceToAnimateSlide.GameObject.transform.localPosition = screenPositionToAnimate;
					//make sure they swap, exchange positions and stuff
					Swap(PieceToAnimateSlide.CurrentI, PieceToAnimateSlide.CurrentJ, toAnimateI, toAnimateJ, false);
					//set the required properties
					MatrixSlide[PieceToAnimateSlide.CurrentI, PieceToAnimateSlide.CurrentI].CurrentJ = PieceToAnimateSlide.CurrentI;
					MatrixSlide[PieceToAnimateSlide.CurrentI, PieceToAnimateSlide.CurrentJ].CurrentJ = PieceToAnimateSlide.CurrentJ;
					MatrixSlide[toAnimateI, toAnimateJ].CurrentI = toAnimateI;
					MatrixSlide[toAnimateI, toAnimateJ].CurrentJ = toAnimateJ;
					gameState = GameState.Playing;
					//check if the use has won
					CheckForVictory();
				}
				break;
		}
    }

    private void CheckForVictory()
    {
		bool victory = true;
		switch (gameMode)
		{
			case GameMode.Jigsaw:
        		//dual loop to check the object's properties
				for (int i = 0; i < RowNumber; i++)
				{
					for (int j = 0; j < ColumnNumber; j++)
					{
						if (MatrixJigsaw [i, j].GameObject.transform.localPosition != snappingPositions [i * ColumnNumber + j])
						{
							victory = false;
						}
						else
						{
							//Matrix [i, j].GameObject.GetComponent<SpriteRenderer> ().color  = new UnityEngine.Color(0.9f, 0.9f, 0.9f, 1);
							MatrixJigsaw [i, j].GameObject.GetComponent<BoxCollider2D> ().enabled = false;
							MatrixJigsaw [i, j].GameObject.GetComponent<SpriteRenderer> ().sortingOrder = 1;
							MatrixJigsaw [i, j].GameObject.transform.GetChild (0).GetComponent<SpriteMask> ().frontSortingOrder = 1;
							MatrixJigsaw [i, j].GameObject.transform.GetChild (1).GetComponent<SpriteMask> ().frontSortingOrder = 1;
							MatrixJigsaw [i, j].GameObject.transform.GetChild (2).GetComponent<SpriteMask> ().frontSortingOrder = 1;
							MatrixJigsaw [i, j].GameObject.transform.GetChild (3).GetComponent<SpriteMask> ().frontSortingOrder = 1;
						}
					}
				}
				break;
			case GameMode.Slide:
				//dual loop to check the object's properties
				for (int i = 0; i < RowNumber; i++)
				{
					for (int j = 0; j < ColumnNumber; j++)
					{
						if (MatrixSlide [i, j].CurrentI != MatrixSlide [i, j].OriginalI ||
							MatrixSlide [i, j].CurrentJ != MatrixSlide [i, j].OriginalJ)
						{
							return;
						}
					}
				}

				//if we did not return, then we've won!
				gameState = GameState.End;

				go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = true;
				break;
		}

		if (victory)
		{
			gameState = GameState.End;
		}
	}*/
}
