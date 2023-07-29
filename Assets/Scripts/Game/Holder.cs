using UnityEngine;
using System.Collections;

public class Holder : MonoBehaviour {

	// store our "held" game piece here until we need it - this is a Transform though we only want the position property
	public Transform m_holdPosition;

	// current shape being held
	public Shape m_heldShape = null;

	// whether the hold Button is ready 
	private bool m_canRelease = false;

	// what to scale the Shape when it is held
	public float m_scale = 0.5f;

	// gentle beep when we click our button
	public AudioClip buttonClip;

	// error beep if we try to use button before it is ready
	public AudioClip errorClip;

	// particles to play when we catch a shape
	public ParticlePlayer m_particleEffect;


	// places a shape in our holder - we will call this from the gameController
	public void Catch(Shape shape)
	{

		if (m_heldShape != null)
		{
			Debug.LogWarning("HOLDER WARNING!  Release a shape before trying to hold.");
			return;
		}

		//
		if (shape == null)
		{
			Debug.LogWarning("HOLDER WARNING! " + shape.name + " is invalid!");

			// tell the game controller that an error occurred
			return;
		}

		// move the shape to the hold position
		if (m_holdPosition)
		{
			shape.transform.position = m_holdPosition.position + shape.UIOffset;
			shape.transform.localScale = new Vector3(m_scale,m_scale,m_scale);
			m_heldShape = shape;

			if (m_particleEffect)
			{
				m_particleEffect.Play();
			}

		}
		else
		{
			Debug.LogWarning("HOLDER WARNING!  Holder has no hold position  assigned!");

		}

	}

	// releases the Shape from this holder and returns it - again we only call this from the gameController
	public Shape Release()
	{
		if (!m_canRelease)
		{
			Debug.LogWarning("HOLDER WARNING!  Wait for cool down!");
			return null;
		}

		if (m_heldShape == null)
		{
			Debug.LogWarning("HOLDER WARNING!  Holder contains no shape!");
			return null;
		}

		// cache our Shape temporarily
		Shape tempShape = m_heldShape;

		// unscale the held scale
		tempShape.transform.localScale = Vector3.one;

		// release the shape
		m_heldShape = null;

		// wait for Reset before we can release again
		m_canRelease = false;

		// return our shape
		return tempShape;
	}

	// allows the button to be activated - when the current piece lands at the bottom we can reset; called by the GameController
	public void Reset()
	{
		m_canRelease = true;

	}

	public bool CanRelease()
	{
		return m_canRelease;

	}
}

