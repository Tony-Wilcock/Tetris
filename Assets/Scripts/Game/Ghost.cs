using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
	// this is the Shape that we will create 
	public Shape m_ghostShape = null;

	//whether we reached the bottom of the Board
	bool m_hitBottom = false;

	// set the Ghost Shape's color to a faint white
	Color m_color = new Color(1f,1f,1f,0.1f);

	// method that we will call from the Controller to draw the ghost Shape
	public void DrawGhost(Shape originalShape, Board gameBoard)
	{
		// Instantiating the ghost shape for the first time
		if (m_ghostShape == null)
		{
			// needed when we use start the game, press Hold, or land the current Shape
			m_ghostShape = Instantiate(originalShape,originalShape.transform.position,originalShape.transform.rotation) as Shape;
			m_ghostShape.gameObject.name = "GhostShape";
			m_ghostShape.transform.localScale = Vector3.one;


			// change the colors of the Ghost shape to be faint white
			SpriteRenderer[] allRenderers = m_ghostShape.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer r in allRenderers)
			{
				r.color = m_color;
			}
		} 

		// update the Shape's side-to-side position before we move it to the bottom
		else
		{
			m_ghostShape.transform.position = originalShape.transform.position;
			m_ghostShape.transform.rotation = originalShape.transform.rotation;

		}

		// move the Shape to the bottom of the Board
		m_hitBottom = false;

		// keep moving the Ghost shape downward until we hit the bottom of the Board; note that we needed to modify
		// Board.IsValidPosition to accept two Shapes in order for this to work 
		while (!m_hitBottom)
		{
			m_ghostShape.MoveDown();
			if (!gameBoard.IsValidPosition(m_ghostShape,originalShape))
			{
				m_ghostShape.MoveUp();
				m_hitBottom = true;
			}
		}
	}

	// call this from the Controller to switch to a new Shape
	public void Reset()
	{
		// probably not the most efficient way of doing this but it works...
		if (m_ghostShape !=null)
			Destroy(m_ghostShape.gameObject);
	}
}
