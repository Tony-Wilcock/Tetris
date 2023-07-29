using UnityEngine;
using System.Collections;

public class Shape : MonoBehaviour {

	// whether our Shape can rotate or not
	public bool m_canRotate = true;

	// small offset that can be used to fix UI centered
	public Vector3 UIOffset;


	// move the shape by a vector moveDirection
	void Move(Vector3 moveDirection) 
	{
		transform.position += moveDirection;
	}

	// move the shape downward one space
	public void MoveDown() 
	{
		Move(new Vector3(0, -1, 0));
	}

	// only used to correct an invalid downward move
	public void MoveUp() 
	{
		Move(new Vector3(0, 1, 0));
	}

	// move one space to the right
	public void MoveRight()
	{
		Move(new Vector3(1, 0, 0));
	}

	// move one space to the left
	public void MoveLeft()
	{
		Move(new Vector3(-1, 0, 0));
	}

	// rotate clockwise 90 degrees
	public void RotateRight()
	{
		if (m_canRotate)
			transform.Rotate(0, 0, -90);
	}

	// rotate counter clockwise 90 degrees
	public void RotateLeft()
	{
		if (m_canRotate)
			transform.Rotate(0, 0, 90);

	}

	public void RotateClockwise(bool rotateClockwise)
	{
		if (rotateClockwise)
			RotateRight();
		else
			RotateLeft();


	}
	// for testing only
	/*
	public void ReportCoord()
	{
		Vector2 myPos = new Vector2(transform.position.x,transform.position.y);
		string posMsg = VectorUtility.Round(myPos).ToString();
		Debug.Log(gameObject.name  + " is at coordinate " + posMsg);
	}
	*/

}
