using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconToggle : MonoBehaviour {

	// the sprite that we use if we are set to true
	public Sprite m_iconTrue;

	// the sprite that we use if we are set to false
	public Sprite m_iconFalse;

	[SerializeField]
	Image m_image;

	public bool m_defaultIconState;

	void Start()
	{
		if (!m_image)
			m_image = GetComponent<Image>();

	}
	public void ToggleIcon(bool state)
	{
		if (!m_image)
			return;
		
		if (m_iconTrue && m_iconFalse)
			m_image.sprite = (state) ? m_iconTrue : m_iconFalse;
		else
			Debug.LogWarning("ICONTOGGLE Undefined iconTrue and/or iconFalse!");
	}

}
