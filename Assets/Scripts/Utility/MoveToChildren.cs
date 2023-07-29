using UnityEngine;
using System.Collections;

// use this to attach an array of objects to the child transform of the current object
// this is useful for moving our Glowing Square FX objects to the squares in our current Shape

public class MoveToChildren : MonoBehaviour {

	// a list of GameObjects that we will move
	GameObject[] m_targets;

	// we will find our targets with this Tag name
	public string m_targetTag;

	// send this message to the m_targets when we attach
	public string m_message;


	// Use this for initialization
	void Start () {
		if (m_targetTag != null || m_targetTag != "")
			m_targets = GameObject.FindGameObjectsWithTag(m_targetTag);
	}

	public void AttachTargets()
	{
		// check in case we don't have any objects with the proper tag
		if (m_targets.Length ==0)
			return;

		// counter
		int i = 0;

		// loop through all of the child transforms
		foreach(Transform child in gameObject.transform)
		{
			// if a target exists at our current counter ...
			if (m_targets[i]!=null )
			{
				// ...position it over the child transform
				m_targets[i].transform.position = child.position;

				//ParticlePlayer pp = m_targets[i].GetComponent<ParticlePlayer>();
				//pp.Play();

				// send a message to each target object if specified
				if (m_message !=null || m_message != "")
				{
					m_targets[i].SendMessage(m_message);
				}

				// increment the counter
				i++;
			}
		}
	}

	public GameObject[] GetTargets()
	{
		return m_targets;
	}

}
