using System;
using UnityEngine;

public class JigsawGame : Game
{
	private Vector3 screenPositionToAnimate;
	private int toAnimateI, toAnimateJ;
	/*private float vertStep;
	private float horiStep;
	private float extraVertStep;
	private float extraHoriStep;*/
	private Vector3[] snappingPositions;
	private Piece PieceToAnimate;
	private Texture2D tex_resized;

	Texture2D texture_big = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_random_big_0");
	Texture2D texture_small = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_random_small_0");

	Texture2D [] textures;

	Texture2D texture_corner_up = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_corner_up");
	Texture2D texture_corner_down = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_corner_down");
	Texture2D texture_edge = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_edge");

	public JigsawGame (GameObject _piece_template, int _ColumnNumber, int _RowNumber)
		:base (_piece_template, _ColumnNumber, _RowNumber)
	{
		gameMode = GameMode.Jigsaw;

		//Texture2D texture_big = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_random_big_0");
		texture_big.name = "puzzle_random_big_0";
		//Texture2D texture_small = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_random_small_0");
		texture_small.name = "puzzle_random_small_0";

		textures = new Texture2D []{texture_big, texture_small};

		//Texture2D texture_corner_up = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_corner_up");
		//Texture2D texture_corner_down = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_corner_down");
		//Texture2D texture_edge = Resources.Load<Texture2D> ("Textures/Jigsaw/puzzle_edge");
	}

	public override void Prepare(Texture2D tex)
	{
		this.tex = tex;
		snappingPositions = new Vector3[RowNumber * ColumnNumber];
		SliceTexture ();
		OneDToTwoD ();
	}

	public override void Launch() 
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
                
                float offsetX = Matrix[i, j].GameObject.transform.GetChild(0).localPosition.x * Pieces.transform.localScale.x;
                float offsetY = Matrix[i, j].GameObject.transform.GetChild(0).localPosition.y * Pieces.transform.localScale.y;

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
				Matrix[i,j].GameObject.transform.position = new Vector3(random_x - offsetX, random_y - offsetY, 0);
			}
		}
	}

	public override void CheckPieceInput()
	{
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
				string name = hit.collider.transform.parent.name;
				string[] parts = name.Split ('-');
				int iPart = int.Parse (parts [1]);
				int jPart = int.Parse (parts [2]);

				PieceToAnimate = Matrix [iPart, jPart];
				gameState = GameState.Animating;
			}
		}
	}

	public override void AnimateMovement()
	{
		//animate it
		if (PieceToAnimate != null)
		{
            float offsetX, offsetY;

            offsetX = PieceToAnimate.GameObject.transform.GetChild(0).localPosition.x * Pieces.transform.localScale.x;
            offsetY = PieceToAnimate.GameObject.transform.GetChild(0).localPosition.y * Pieces.transform.localScale.y;

            float distance_to_screen = Camera.main.WorldToScreenPoint (PieceToAnimate.GameObject.transform.position).z;
			Vector3 pos_move;
			if (Input.touchCount > 0)
			{
				pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.touches [0].position.x, Input.touches [0].position.y, distance_to_screen));
			}
			else
			{
				pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
            }
            PieceToAnimate.GameObject.transform.position = new Vector3 (pos_move.x - offsetX, pos_move.y - offsetY, PieceToAnimate.GameObject.transform.position.z);
		}
		if (Input.GetMouseButtonUp (0))
		{
			//make sure they snap
			Snap ();
			//set the required properties
			gameState = GameState.Playing;
			PieceToAnimate = null;
			//check if the use has won
			CheckForVictory ();
		}
	}

	private void SliceTexture()
	{
		if (go != null)
		{
			foreach(GameObject go_o in go)
			{
				GameObject.Destroy (go_o);
			}
		}
		go = new GameObject[RowNumber * ColumnNumber];

        Pieces.transform.localScale = Vector3.one;
        Frame.transform.localScale = Vector3.one;

        for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j] = GameObject.Instantiate(piece_template, Pieces.transform);
                go [i * ColumnNumber + j].name = "piece-" + i + "-" + j;
			}
		}
	}

	private void OneDToTwoD()
	{
		float mask_width = texture_big.width;
		float mask_height = texture_big.height;

        int frame_width = tex.width;
        int frame_height = tex.height;
        
        float vertStepLocal = frame_height * 5 / (4 * RowNumber);
		float horiStepLocal = frame_width * 5 / (4 * ColumnNumber);

        Sprite s = Sprite.Create(tex, new Rect(0, 0, frame_width, frame_height), new Vector2(0.5f, 0.5f));
        
        Frame.GetComponent<SpriteRenderer> ().sprite = s;
		Frame.SetActive (true);

        float ratio_pieces = 0;
        float ratio_frame = 0;

        float tex_width = Frame.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float tex_height = Frame.GetComponent<SpriteRenderer>().sprite.bounds.size.y;

        Vector3 screen_size = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

        /*
        float width_with_margin = Screen.width - 150;
        float height_with_margin = Screen.height - 150;
        */
        float width_with_margin = screen_size.x - 150f / 100;
        float height_with_margin = screen_size.y - 150f / 100;

        if (tex_width > width_with_margin)
        {
            ratio_pieces = (width_with_margin - 50f / 100) / tex_width;
            ratio_frame = width_with_margin / tex_width;
        }
        else if (tex_height > height_with_margin)
        {
            ratio_pieces = (height_with_margin - 50f / 100) / tex_height;
            ratio_frame = height_with_margin / tex_height;
        }

        Pieces.transform.localScale = new Vector3(ratio_pieces, ratio_pieces, 0);
        Frame.transform.localScale = new Vector3(ratio_frame, ratio_frame, 0);

        Matrix = new Piece[RowNumber, ColumnNumber];
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				int sortingOrder = i * ColumnNumber + j + 1;

				Vector3 point = new Vector3(
                    (j - (float)(ColumnNumber - 1) / 2) * frame_width / (ColumnNumber * 100),
                    (RowNumber - 1 - i - (float)(RowNumber - 1) / 2) * frame_height / (RowNumber * 100));
				go [i * ColumnNumber + j].transform.GetChild(0).localPosition = point;
				snappingPositions [i * ColumnNumber + j] = point;

                SpriteRenderer sr = go[i * ColumnNumber + j].GetComponent<SpriteRenderer>();
                sr.sprite = s;
                sr.sortingOrder = i * ColumnNumber + j + 1;
                //sr.sortingLayerID = SortingLayer.NameToID(sr.sortingOrder.ToString());

                Texture2D tex_left;
				Texture2D tex_up;
				Texture2D tex_right;
				Texture2D tex_down;

				Texture2D tex_left_complementary = null;
				Texture2D tex_up_complementary = null;
				//Texture2D tex_right_complementary = null;
				//Texture2D tex_down_complementary = null;

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
				FillSquare(go [(i + 0) * ColumnNumber + (j + 0)], textures_to_fill, sortingOrder);

				// after we fill the piece we fill its other complementary side
				if(j < ColumnNumber - 1)
				{
					tex_left_complementary = tex_right.name.Contains("big")?(textures[Array.IndexOf(textures, tex_right) + 1]):(textures[Array.IndexOf(textures, tex_right) - 1]);

					textures_to_fill = new Texture2D[] {tex_left_complementary, null, null, null};
					FillSquare(go [(i + 0) * ColumnNumber + (j + 1)], textures_to_fill, -1);
				}
				if(i < RowNumber - 1)
				{
					tex_up_complementary = tex_down.name.Contains("big")?(textures[Array.IndexOf(textures, tex_down) + 1]):(textures[Array.IndexOf(textures, tex_down) - 1]);

					textures_to_fill = new Texture2D[] {null, tex_up_complementary, null, null};
					FillSquare(go [(i + 1) * ColumnNumber + (j + 0)], textures_to_fill, -1);
				}

				go [i * ColumnNumber + j].transform.GetChild(0).GetChild (0).localScale = new Vector3 (horiStepLocal / mask_width, vertStepLocal / mask_height);
				go [i * ColumnNumber + j].transform.GetChild(0).GetChild (1).localScale = new Vector3 (vertStepLocal / mask_width, horiStepLocal / mask_height);
				go [i * ColumnNumber + j].transform.GetChild(0).GetChild (2).localScale = new Vector3 (horiStepLocal / mask_width, vertStepLocal / mask_height);
				go [i * ColumnNumber + j].transform.GetChild(0).GetChild (3).localScale = new Vector3 (vertStepLocal / mask_width, horiStepLocal / mask_height);

                //add a box collider the the raycast to work properly
                if (go [i * ColumnNumber + j].transform.GetChild(0).GetComponent<BoxCollider2D>() == null)
                {
					go [i * ColumnNumber + j].transform.GetChild(0).gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
					go [i * ColumnNumber + j].transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().size = new Vector2(horiStepLocal / 100, vertStepLocal / 100);
                }

				//place relevant information
				Matrix[i, j] = new Piece();
				Matrix[i, j].GameObject = go[i * ColumnNumber + j];
				Matrix[i, j].OriginalI = i;
				Matrix[i, j].OriginalJ = j;
			}
		}

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

	// 0 = tex_left, 1 = tex_up, 2 = tex_right, 3 = tex_down
	private void FillSquare(GameObject piece, Texture2D[] textures, int sortingOrder)
	{
		for (int i = 0; i < textures.Length; i++)
		{
			SpriteMask sm = piece.transform.GetChild(0).GetChild (i).GetComponent<SpriteMask> ();
			if (sortingOrder >= 0)
            {
                sm.frontSortingOrder = sortingOrder;
                sm.backSortingOrder = sortingOrder - 1;
                //sm.frontSortingLayerID = SortingLayer.NameToID(sortingOrder.ToString());
            }
			if (textures [i] != null && sm.sprite == null)
			{
				sm.sprite = Sprite.Create (textures [i], new Rect (0, 0, textures [i].width, textures [i].height), new Vector2 (0.5f, 0.5f));
			}
		}
	}

	private void Snap ()
    {
        float offsetX, offsetY;

        offsetX = PieceToAnimate.GameObject.transform.GetChild(0).localPosition.x;
        offsetY = PieceToAnimate.GameObject.transform.GetChild(0).localPosition.y;

        float vertStep = Mathf.Abs(snappingPositions[1 * ColumnNumber + 0].y - snappingPositions[0 * ColumnNumber + 0].y);
		float horiStep = Mathf.Abs(snappingPositions[0 * ColumnNumber + 1].x - snappingPositions[0 * ColumnNumber + 0].x);

		for (int k = 0; k < snappingPositions.Length; k++)
		{
			if (Mathf.Abs(PieceToAnimate.GameObject.transform.localPosition.x + offsetX - snappingPositions[k].x) <= horiStep / 2 &&
				Mathf.Abs(PieceToAnimate.GameObject.transform.localPosition.y + offsetY - snappingPositions[k].y) <= vertStep / 2)
			{
                PieceToAnimate.GameObject.transform.localPosition = snappingPositions[k] - new Vector3(offsetX, offsetY, 0);
			}
		}
	}

	private void CheckForVictory()
	{
		bool victory = true;

		//dual loop to check the object's properties
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				if (
                    Mathf.Abs(Matrix[i, j].GameObject.transform.localPosition.x) > 0.01f ||
                    Mathf.Abs(Matrix[i, j].GameObject.transform.localPosition.y) > 0.01f
                    )
				{
					victory = false;
				}
				else
				{
					Matrix [i, j].GameObject.transform.GetChild(0).GetComponent<BoxCollider2D> ().enabled = false;
					/*Matrix [i, j].GameObject.transform.GetComponent<SpriteRenderer> ().sortingOrder = 1;
					Matrix [i, j].GameObject.GetComponent<SpriteRenderer> ().color  = new UnityEngine.Color(0.9f, 0.09f, 0.09f, 1);
					Matrix [i, j].GameObject.transform.GetChild(0).GetChild (0).GetComponent<SpriteMask> ().frontSortingOrder = 1;
					Matrix [i, j].GameObject.transform.GetChild(0).GetChild (1).GetComponent<SpriteMask> ().frontSortingOrder = 1;
					Matrix [i, j].GameObject.transform.GetChild(0).GetChild (2).GetComponent<SpriteMask> ().frontSortingOrder = 1;
					Matrix [i, j].GameObject.transform.GetChild(0).GetChild (3).GetComponent<SpriteMask> ().frontSortingOrder = 1;*/
				}
			}
		}

		if (victory)
		{
			gameState = GameState.End;
		}
	}
}

