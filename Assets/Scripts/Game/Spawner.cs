using UnityEngine;
using System.Collections;

public class Spawner: MonoBehaviour {

	// full collection of our Shapes/game pieces
	public Shape[] m_allShapes;

	// represents the positions on the user interface to show the queued shapes
	public Transform[] m_queuedPositions = new Transform[3];

	// an array of the next Shapes
	private Shape[] m_queuedShapes = new Shape[3] ;

	// scale our queued pieces to this value
	public float m_queueScale = 0.3f;

	public ParticlePlayer m_particleEffect;

	public bool m_useQueue = true;

	// 
	void Start()
	{
		if (m_useQueue)
		{
			InitQueue();
		}
		else
		{

			foreach(Transform qp in m_queuedPositions)
			{
				qp.parent.gameObject.SetActive(false);
				//qp.gameObject.SetActive(false);
			}
		}
	}

	// 
	public Shape SpawnShape() 
	{

		Shape shape = null;
		if (m_useQueue)
		{
			shape = GetQueuedShape();
		}
		else
		{
			shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
		}



		
		shape.transform.position = transform.position;
		shape.transform.localScale = Vector3.one;

		//make shape pop to size instead of just appearing there
		StartCoroutine(GrowShape(shape,transform.position));

		// add some particles when the shape spawns
		if (m_particleEffect)
		{
			m_particleEffect.Play();
		}

		if (shape)
		{
			return shape;
		}
		else
		{
			Debug.LogWarning("WARNING!  Check allShapes array in Spawner");
			return null;
		}
	}

	// grow the shape from zero to 1 in one second
	public IEnumerator GrowShape(Shape shape, Vector3 position)
	{
		float size = 0f;

		while (size < 1f)
		{
			shape.transform.localScale = new Vector3(size, size, size);
			size += 0.1f;
			shape.transform.position = position;
			yield return null;
		}

		// force us back to (1,1,1)
		shape.transform.localScale = Vector3.one;
	}

	// fills each empty slot of the queue with a random Shape
	void FillQueue()
	{
		for (int i=0; i < m_queuedShapes.Length; i++)
		{
			if (m_queuedShapes[i] == null)
			{
				m_queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape; 
				m_queuedShapes[i].transform.position = m_queuedPositions[i].position + m_queuedShapes[i].UIOffset;
				m_queuedShapes[i].transform.localScale = new Vector3(m_queueScale,m_queueScale,m_queueScale);
			}
		}
	}

	// inits the queue to null
	void InitQueue()
	{
		for (int i=0; i < m_queuedShapes.Length; i++)
		{
			m_queuedShapes[i] = null;
		}
		FillQueue();
	}

	// returns the first element of the shapeQueue and fills up any empty slots with Random shapes
	Shape GetQueuedShape()
	{
		Shape firstShape = null;

		// designate the 0 index Shape in the queue 
		if (m_queuedShapes[0]!=null)
		{
			firstShape = m_queuedShapes[0];
		}

		// set Shapes1,2,3... to 0,1,2... and move their positions forward in the queue
		for (int i=1; i < m_queuedShapes.Length; i++)
		{
			m_queuedShapes[i-1] = m_queuedShapes[i];
			m_queuedShapes[i-1].transform.position = m_queuedPositions[i-1].position + m_queuedShapes[i-1].UIOffset;
		}

		m_queuedShapes[m_queuedShapes.Length - 1] = null;

		// create a random Shape in the empty position
		FillQueue();

		// returns either the first Shape (or null if the queue is empty)
		return firstShape;

	}

	// returns a random Shape from allShapes
	Shape GetRandomShape()
	{
		int i = Random.Range(0,m_allShapes.Length);

		// check in the event this gets broken in the Inspector
		if (m_allShapes[i] == null)
		{
			Debug.LogWarning("WARNING!  Invalid shape!  Check allShapes in Spawner");
			return null;
		}
		else 
		{
			return m_allShapes[i];
		}
	}

}
