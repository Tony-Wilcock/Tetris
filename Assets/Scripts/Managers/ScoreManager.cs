using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	// current score
	int m_score;

	// number of lines to clear before next level
	int m_lines;

	// current level
	int m_level = 0;


	// property that returns our current m_level
	public int M_level
	{
		get
		{
			return m_level;
		}
		set
		{

		}
	}

	// text component that shows the number of lines left to clear
	public Text m_linesText;

	// text component that shows our current level
	public Text m_levelText;

	// text component that shows our current score
	public Text m_scoreText;

	// fx to play when we level up
	public ParticlePlayer m_levelUpEffect;

	// number of digits to pad the score
	public int m_scorePadding = 5;

	// number of lines to clear to reach the next level
	public int m_linesPerLevel = 5;

	//
	public bool didLevelUp = false; 

	// lower limit for number of lines to clear
	const int m_minLines = 1;

	// upper limit for number of lines to clear
	const int m_maxLines = 4;

	void Start()
	{
		ResetLevel();
		ResetLines();
		UpdateUIText();
	}

	// clears n number of lines 
	public void ScoreLines(int n)
	{
		didLevelUp = false;

		// we give bonus lines for anything bigger than 1
		n = Mathf.Clamp(n,m_minLines,m_maxLines);

		switch (n)
		{
			case 1:	
				m_score += 40 * m_level;
				break;
			case 2:
				m_score += 100 * m_level;
				break;
			case 3:
				m_score += 300 * m_level;
				break;
			case 4:
				m_score += 1200 * m_level;
				break;
		}
			
		// decrease number of lines until next level
		m_lines -= n;

		// check if we leveled up
		if (m_lines <= 0)
		{
			LevelUp();
		}

		// update the score and UI
		UpdateUIText();

	}

	// increments our level
	public void LevelUp()
	{
		m_level++;

		ResetLines();
		UpdateUIText();

		if (m_levelUpEffect)
		{
			m_levelUpEffect.Play();
		}

		// let gameController know that we leveled up
		didLevelUp = true;

	}

	// increments m_score by a certain value
	public void AddScore(int scoreVal)
	{
		m_score += scoreVal;
		UpdateUIText();
	}

	// sets m_score to an explicit value
	public void SetScore(int value)
	{
		m_score = value;
		UpdateUIText();
	}

	// sets our lines explicitly
	public void SetLines(int lines)
	{
		m_lines = lines;
		UpdateUIText();
	}

	// sets our level explicitly
	public void SetLevel(int level)
	{
		m_level = level;
		UpdateUIText();
	}

	// start at level 1
	public void ResetLevel()
	{
		SetLevel(1);
	}

	// calculate how many lines when we level up
	void ResetLines()
	{
		m_lines = m_linesPerLevel* m_level;

	}

	void UpdateUIText()
	{
		if (m_linesText)
			m_linesText.text = PadZero(m_lines,1);

		if (m_levelText)
			m_levelText.text = PadZero(m_level,1);

		if (m_scoreText)
			m_scoreText.text = PadZero(m_score,m_scorePadding);
	}

	// returns a formatted string from an integer n that pads zeroes out to a certain number of digits
	string PadZero(int n,int padDigits)
	{
		string nStr = n.ToString();

		while (nStr.Length < padDigits)
		{
			nStr = "0" + nStr;
		}
		return nStr;
	}
}
