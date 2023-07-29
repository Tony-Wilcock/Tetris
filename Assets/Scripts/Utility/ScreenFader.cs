using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// ScreenFader can only be attached to an Image
[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour {

	// whether we turn on at Start automatically
	public bool autoRun = false;

	// the image we are fading
	private MaskableGraphic graphic;

	// the base color of the image
	private Color screenColor;

	private  float alpha;

	public float startAlpha;
	public float targetAlpha;

	// wait n seconds before we start
	public float delay;

	// time to complete the fade transition
	public float timeToFade = 1f;

	// our increment
	[SerializeField]
	private float inc;

	// cache our original start and target alpha values
	private float origStartAlpha;
	private float origTargetAlpha;


	void Start () {
		// cache our original start and target alpha values
		origStartAlpha = startAlpha;
		origTargetAlpha = targetAlpha;

		// cache the color from the image
		graphic = GetComponent<MaskableGraphic>();

		screenColor = graphic.color;

		// set our starting alpha
		alpha = startAlpha;
		Color tempColor = new Color (screenColor.r,screenColor.g,screenColor.b,startAlpha);

		// set the image or material to our temp color
		graphic.color = tempColor;

		// calculate the increment per frame - technically this is slightly wrong since the Time.deltaTime varies slightly per frame
		inc = (targetAlpha - startAlpha)/timeToFade*Time.deltaTime;

		// start the fade if using autoRun
		if (autoRun)
			Fade ();

	}

	// trigger this externally or from Start method to begin the Fade
	public void Fade()
	{
		if (graphic == null)
		{
			Debug.Log ("SCREENFADER Error!  Attach an MaskableGraphic component to object " + this.gameObject.name);
			return;
		}

		StartCoroutine("FadeRoutine");
	}

	IEnumerator FadeRoutine()
	{
		yield return new WaitForSeconds(delay);
		while (Mathf.Abs(targetAlpha - alpha) > .01)
		{
			yield return new WaitForEndOfFrame();
			alpha = alpha + inc;
			Color tempColor = new Color (screenColor.r,screenColor.g,screenColor.b,alpha);

			graphic.color = tempColor;
		}
	}

	public void FadeOn()
	{
		startAlpha = 0f;
		targetAlpha = (origStartAlpha > origTargetAlpha) ? origStartAlpha: origTargetAlpha;
		alpha = startAlpha;

		// calculate the increment per frame - technically this is slightly wrong since the Time.deltaTime varies slightly per frame
		inc = (targetAlpha - startAlpha)/timeToFade*Time.deltaTime;

		Fade();
	}

	public void FadeOff()
	{

		startAlpha =  (origStartAlpha > origTargetAlpha) ? origStartAlpha: origTargetAlpha;
		targetAlpha = 0f;
		alpha = startAlpha;

		// calculate the increment per frame - technically this is slightly wrong since the Time.deltaTime varies slightly per frame
		inc = (targetAlpha - startAlpha)/timeToFade*Time.deltaTime;

		Fade();
	}

}
