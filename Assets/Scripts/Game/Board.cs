using UnityEngine;
using UnityEngine.UI;

using System.Collections;

// class responsible for keeping track of a grid of squares and what occupies each
public class Board : MonoBehaviour {

	public Transform m_emptySprite;
	//public Transform fullSprite;
	//public Transform wallSprite;

	// nine-sliced sprite that we will use to surround the border
	public RectTransform m_borderSprite;

	// automatic space added between the board the border 
	public float m_borderPadding = 0.25f;

	// where we will draw our UI elements
	public RectTransform m_UICanvas;

	// board dimensions
	public int m_height = 30;
	public int m_width = 10;

	// we end the game if our shapes hit at this line or above
	public int m_maxLine = 22;

	// particle effects for the row clearing
	public ParticlePlayer[] m_rowFX = new ParticlePlayer[4];

	// grid keeps track of any game pieces
	Transform[,] m_grid;

	// how many wall sprites to draw around
	//public int borderWidth = 20;

	public bool m_visualEmptyCells = false;

	// number of rows completed
	public int m_completedRows = 0;


	void Start()
	{
		m_grid = new Transform[m_width, m_height];

		//testGameObject = new GameObject("blah");
		//newParent = gameObject.transform;
		if (m_visualEmptyCells)
		{
			DrawEmptyCells();
			DrawBorder();
		}
	}


	void DrawEmptyCells() {
		for (int y = 0; y < m_maxLine; y++)
		{
			for (int x = 0; x < m_width; x++) 
			{
				Transform clone;
				clone = Instantiate(m_emptySprite, new Vector3(x + transform.position.x, y + transform.position.y, 0), Quaternion.identity) as Transform;
				clone.name = "Board Space ( x = " + x.ToString() +  " , y =" + y.ToString() + " )"; 
				clone.transform.parent = transform;
			}
		}
	}

	// automatically draws a boundary around the Board based on the corners
	void DrawBorder()
	{
		
		float centerX = ((float)(m_width - 1)/2f );
		float centerY = ((float)(m_maxLine - 1)/2f );

		float left = 0f - 0.5f - m_borderPadding;
		float right = m_width - 0.5f + m_borderPadding;
		float top = m_maxLine - 0.5f + m_borderPadding;
		float bottom = 0f - 0.5f - m_borderPadding;

		//Debug.Log("center of the board is at: x = " + centerX.ToString() + ", y = " + centerY.ToString());

		// viewport position, normalized 
		Vector2 viewportCenter = Camera.main.WorldToViewportPoint(new Vector3(centerX, centerY, 0));
		Vector2 viewportBottomLeft = Camera.main.WorldToViewportPoint(new Vector3(left, bottom, 0));
		Vector2 viewportTopRight = Camera.main.WorldToViewportPoint(new Vector3(right, top, 0));

		// note: that both the viewport and canvas space are normalized

		// in viewportPos, the (0,0) is at the lower left  but for the RectTransform the (0,0) is at the center; in viewportPos, this is (0.5, 0.5),
		// so we need to subtract 0.5 from the x and the y to convert between them


		//Debug.Log("viewport center is at: x = " + viewportCenter.x.ToString() + ", y = " + viewportCenter.y.ToString());
		//Debug.Log("The screen size is: " + UICanvas.sizeDelta.ToString());
		//Debug.Log("The screen width is: " + Screen.width.ToString());
		//Debug.Log("The screen height is: " + Screen.height.ToString());

		// Canvas space
		Vector2 screenCenter =new Vector2(m_UICanvas.sizeDelta.x * (viewportCenter.x -0.5f),m_UICanvas.sizeDelta.y * (viewportCenter.y - 0.5f));
		Vector2 bottomLeftScreen = new Vector2(m_UICanvas.sizeDelta.x * (viewportBottomLeft.x - 0.5f), m_UICanvas.sizeDelta.y * (viewportBottomLeft.y - 0.5f));
		Vector2 topRightScreen = new Vector2(m_UICanvas.sizeDelta.x * (viewportTopRight.x - 0.5f), m_UICanvas.sizeDelta.y * (viewportTopRight.y - 0.5f));

		//Debug.Log("viewport position of " + new Vector3(centerX, centerY, 0).ToString() + " is " + viewportPos.ToString());
		//Debug.Log("viewport position of " + new Vector3(bottomLeft.x, bottomLeft.y, 0).ToString() + " is " + bottomLeftScreen.ToString());
		//Debug.Log("viewport position of " + new Vector3(topRight.x, topRight.y, 0).ToString() + " is " + topRightScreen.ToString());
		//Debug.Log("width by height  = " + (topRightScreen - bottomLeftScreen).ToString());

		//Debug.Log ("Canvas size delta = " + UICanvas.sizeDelta.ToString());

		//Vector2 spriteSize = (topRightScreen - bottomLeftScreen);

		m_borderSprite.anchoredPosition = screenCenter;
		m_borderSprite.sizeDelta = (topRightScreen - bottomLeftScreen);
	}


	// Update the grid array 
	public void StoreShapeInGrid(Shape shape) {

		// clear out any grid squares occupied by the current moving piece
		ClearShapeFromGrid(shape);

		// store each child square into the grid
		foreach (Transform child in shape.transform) 
		{
			Vector2 vectorRounded = VectorUtility.Round(child.position);
			//Vector2 vectorRounded = child.position;

			//Debug.Log (shape.name + " is at coordinate: " + vectorRounded.ToString());
			m_grid[(int)vectorRounded.x, (int)vectorRounded.y] = child;
		}        
	}

	public void ClearShapeFromGrid(Shape shape)
	{
		// clear out any grid squares occupied by the current moving piece
		for (int y = 0; y < m_height; ++y)
			for (int x = 0; x < m_width; ++x)
				if (m_grid[x, y] != null)
				if (m_grid[x, y].parent == shape.transform)
					m_grid[x, y] = null;
	}


	// given a Vector 2 returns a rounded Vector2
	//Vector2 Round(Vector2 vector) {
	//	return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
	//}

	// checks if we are inside the board
	bool IsWithinBoard(int x, int y) 
	{
		return (x >= 0 && x < m_width && y >= 0);
	}

	//
	bool IsOccupied(int x, int y, Shape shape)
	{
		// check if the block occupies a grid square that is not empty and belongs to a different shape
		return  (m_grid[x,y] != null && m_grid[x,y].parent != shape.transform);
	}


	//  given a shape, checks two things:
	// 	a) returns false if the shape is outside the board
	//  b) returns false if any child block of the shape occupies a tile that is not empty
	//  returns true only if neither condition is true
	public bool IsValidPosition(Shape shape) 
	{   
		return IsValidPosition(shape, shape);
	}


	public bool IsValidPosition(Shape shapeClone, Shape shapeOriginal)
	{
		// check each child block of the given shape
		foreach (Transform child in shapeClone.transform) 
		{
			// always round the position to integers
			Vector2 pos = VectorUtility.Round(child.position);

			// check if we are inside the board's grid
			if (!IsWithinBoard((int)pos.x, (int)pos.y))
			{
				return false;
			}

			// check if the block occupies a grid square that is not empty and belongs to a different shape
			//if (grid[(int)pos.x, (int)pos.y] != null &&	grid[(int)pos.x, (int)pos.y].parent != shape.transform)
			if (IsOccupied((int)pos.x, (int)pos.y, shapeOriginal))
			{
				return false;
			}

		}
		// only return true if we are within the Board and do not collide with occupied squares
		return true;

	}


	// methods for deleting and shifting rows

	// clears all of the square objects that occupy row y
	void RemoveRowAt(int y) 
	{
		//Debug.Log("REMOVE ROW AT: " + y.ToString());
		for (int x = 0; x < m_width; ++x) 
		{
			if (m_grid[x, y].gameObject != null)
			{
				// delete the game objects associated with this row
				Destroy(m_grid[x, y].gameObject);
			}

			// make sure each square is set to null
			m_grid[x, y] = null;
		}
	}



	// checks if a row y is full
	bool IsComplete(int y) {
		for (int x = 0; x < m_width; ++x)
		{
			if (m_grid[x, y] == null)
			{
				return false;
			}
		}
		// only runs if the loop finishes is completely full
		return true;
	}


	// shifts row y down one line on the grid to y-1  
	void ShiftOneRowDown(int y) 
	{
		for (int x = 0; x < m_width; ++x) 
		{
			if (m_grid[x, y] != null) 
			{
				// set the row below it to hold the contents of row y
				m_grid[x, y-1] = m_grid[x, y];

				// set row y to hold nothing
				m_grid[x, y] = null;

				// move the transform associated with row y-1 physically down in the world
				m_grid[x, y-1].position += new Vector3(0, -1, 0);
			}
		}
	}

	// shifts rows from y to top of grid down one space
	void ShiftRowsDown(int y) 
	{
		for (int i = y; i < m_height; ++i)
		{
			ShiftOneRowDown(i);
		}
	}

	/*  this is the original version before we add the FX and coroutine
	 * 
	// search the whole grid and clear any rows that are full
	public void RemoveCompletedRows() 
	{
		int startComplete = null;
		int numCompleted = 0;

		for (int y = 0; y < height; ++y) 
		{
			// if the row is full...
			if (IsComplete(y)) 
			{
				
				// remove this one row
				RemoveRowAt(y);

				// play FX for row removal
				PlayRowFX(y);

				// shift the above row down to this row
				ShiftRowsDown(y+1);

				// after we have shifted rows we need to check this row again, so count down before we increment
				y--;
			}
		}

	}
	*/

	public IEnumerator RemoveCompletedRows() 
	{
		m_completedRows = 0;
		int counter = 0;

		// play FX 
		for (int y = 0; y < m_height; ++y) 
		{
			if (IsComplete(y)) 
			{
				if (m_rowFX.Length > 0)
				{
					PlayRowFX(counter,y);
				}
				counter ++;
				m_completedRows ++;
			}
		}

		// if we did find a complete row, our counter will be 1 or greater
		//if (counter > 0)
		//	yield return StartCoroutine("WaitForFX",rowFX[counter-1]);

		yield return new WaitForSeconds(0.2f);

		// remove and shift down rows
		for (int y = 0; y < m_height; ++y) 
		{
			if (IsComplete(y)) 
			{
				
				RemoveRowAt(y);
				ShiftRowsDown(y+1);
				yield return new WaitForSeconds(0.2f);

				y--;
			}
		}
		yield return null;

	}

	IEnumerator WaitForFX(ParticlePlayer rowFX)
	{
		ParticleSystem[] allParticles = rowFX.GetComponentsInChildren<ParticleSystem>();
		bool isActive = true;

		while (isActive)
		{
			isActive = false;

			foreach (ParticleSystem p in allParticles)
			{
				if (p.isPlaying)
				{
					isActive = true;
				}
			}

			yield return null;
		}
	}

	/* before switching to more than one Row Glow
	void PlayRowFX(int y)
	{
		if (rowFX)
		{
			rowFX.transform.position = new Vector3(0, y, 0);
			rowFX.Play();
		}
	}
	*/
	// using multiple RowGlow
	void PlayRowFX(int idx, int yPos)
	{
		if (m_rowFX[idx] !=null)
		{
			m_rowFX[idx].transform.position = new Vector3(0, yPos, -1f);
			m_rowFX[idx].Play();
		}

	}

	// check if a shape has hit the max line
	public bool IsMaxedOut(Shape shape)
	{
		foreach (Transform child in shape.transform) 
		{
			if (child.transform.position.y >= m_maxLine - 2)
			{
				return true;
			}
		}
		return false;
	}
}


