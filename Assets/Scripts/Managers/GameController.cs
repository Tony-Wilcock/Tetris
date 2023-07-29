using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    // our game board
    Board m_gameBoard = null;

    // key repeat rate, i.e. time between events when we are holding down a key
    float m_timeToNextKey;

    // the current shape we are controlling
    Shape m_activeShape;

    // controller for spawning new shapes at the top

    Spawner m_spawner;

    Holder m_holder;

    Ghost m_ghost;


    // time it takes our shape to drop one row
    float m_dropInterval = 0.6f;

    // our initial dropInterval speed
    float m_startDropSpeed = 0.6f;

    // the next target time our active shape falls one row;
    float m_timeToDrop = Mathf.Infinity;

    // small delay at beginning of the game
    public float m_startDelay = 1f;

    // turn this on and stop the game 
    public GameObject m_gameOverPanel;

    public ScreenFader m_screenFader;

    // instantiate these when game is over
    public GameObject m_gameOverEffects;

    public bool isPaused = false;

    // toggle this panel when paused
    public GameObject m_pausePanel;

    // set this to true if our game comes to an end
    bool m_gameOver = false;

    // setting for rotation direction
    bool m_rotateClockwise = true;

    // icon that shows the rotation direction
    public Image m_rotateIcon;

    // reference to the scoreManager
    ScoreManager m_scoreManager;

    // reference to the SoundManager object
    SoundManager m_soundManager;



    // runs when we start our application
    void Start()
    {
        m_dropInterval = m_startDropSpeed;

        UpdateRotateIcon();

        m_timeToNextKey = Time.time;

        // references to other objects we need to play the game

        // we need a Board class to play the game
        m_gameBoard = Object.FindObjectOfType<Board>();

        // we need a spawner set up at the top of the Board
        m_spawner = Object.FindObjectOfType<Spawner>();

        // the holder object allows you to temporary hold a Shape you don't want to use immediately
        m_holder = Object.FindObjectOfType<Holder>();

        m_ghost = Object.FindObjectOfType<Ghost>();

        m_soundManager = Object.FindObjectOfType<SoundManager>();

        m_scoreManager = Object.FindObjectOfType<ScoreManager>();

        if (m_gameOverPanel)
            m_gameOverPanel.SetActive(false);

        if (m_pausePanel)
            m_pausePanel.SetActive(false);

        if (m_gameBoard == null)
        {
            Debug.LogWarning("WARNING!  No Board class detected!");
        }

        if (m_spawner == null)
        {
            Debug.LogWarning("WARNING!  Please create a Spawner at the top of the board");
        }

        if (m_holder == null)
        {
            Debug.LogWarning("WARNING!  Please assign the Holder");
        }

        if (m_soundManager == null)
        {
            Debug.LogWarning("WARNING!  No SoundManager found!");
        }

        if (m_scoreManager == null)
        {
            Debug.LogWarning("WARNING!  No ScoreManager found!");
        }

        if (m_screenFader == null)
        {
            Debug.LogWarning("WARNING!  No ScreenFader found!");
        }

        // set our activeShape to start the game
        if (m_spawner != null && m_activeShape == null)
        {
            m_activeShape = m_spawner.SpawnShape();
        }

        // start drop after short delay
        m_timeToDrop = Time.time + m_startDelay;

        if (m_ghost)
            m_ghost.Reset();

        //InvokeRepeating("ReportGhost",1f,1f);
    }

    void ReportGhost()
    {
        if (m_ghost)
        {
            Debug.Log("Ghost is at: " + m_ghost.m_ghostShape.transform.position.ToString());
            Debug.Log("Active shape is at: " + m_activeShape.transform.position.ToString());
        }
    }

    void Update()
    {

        if (m_gameOver)
            return;

        // if some reason there is no active shape let's return a warning since we always should have an active shape
        if (m_activeShape == null)
        {
            Debug.LogWarning("WARNING!  No active shape!");
            return;
        }

        // update the rotation direction icon
        UpdateRotateIcon();

        // handle all of the user input
        HandleInput();
    }

    void HandleInput()
    {
        // if we press right key, shift the shape one space to the right,
        // check if this is a valid position; if it is, update the grid, otherwise, shift it back

        if (Input.GetButtonDown("Pause"))
        {
            Pause();
        }

        if ((Input.GetButton("MoveRight") && (Time.time > m_timeToNextKey)) || Input.GetButtonDown("MoveRight"))
        {
            m_timeToNextKey = Time.time + 0.7f;
            m_activeShape.MoveRight();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
            }
            else
            {
                if (m_soundManager)
                    m_soundManager.PlayClipAtPoint(m_soundManager.m_moveSound, Camera.main.transform.position, m_soundManager.m_fxVolume);
            }
        }
        // same thing with the left arrow key
        //else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        if ((Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKey)) || Input.GetButtonDown("MoveLeft"))
        {
            m_timeToNextKey = Time.time + 0.7f;
            m_activeShape.MoveLeft();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
            }
            else
            {
                if (m_soundManager)
                    m_soundManager.PlayClipAtPoint(m_soundManager.m_moveSound, Camera.main.transform.position, m_soundManager.m_fxVolume);
            }
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }
        // if we press the up arrow key, rotate the shape;
        // check if this is a valid position; if it is, update the grid, otherwise, shift it back
        // if you want to look for the key directly instead of using the InputManager, you would do this: 
        //else if (Input.GetKeyDown(KeyCode.UpArrow)) {
        if (Input.GetButtonDown("Rotate"))
        {
            m_timeToNextKey = Time.time + 0.05f;
            if (m_soundManager)
                m_soundManager.PlayClipAtPoint(m_soundManager.m_moveSound, Camera.main.transform.position, m_soundManager.m_fxVolume);

            if (m_rotateClockwise)
            {
                m_activeShape.RotateRight();
            }
            else
            {
                m_activeShape.RotateLeft();
            }
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_rotateClockwise)
                {
                    m_activeShape.RotateLeft();
                }
                else
                {
                    m_activeShape.RotateRight();
                }
            }
        }
        // if we press the down arrow or if 1 second lapses, shift the shape down
        // if this is a valid position, update the grid,
        // otherwise, shift it back, delete any rows that are full, and Spawn a new box; disable the Boxes script
        // allows the user to press down 
        //else if ((Input.GetKey(KeyCode.DownArrow) && (Time.time > m_timeToNextKey)) || Time.time >= timeToDrop) {
        if (Input.GetButtonDown("MoveDown"))
        {
            m_timeToNextKey = Time.time;
        }
        if ((Input.GetButton("MoveDown") && (Time.time > m_timeToNextKey)) || Time.time >= m_timeToDrop)
        {
            m_activeShape.MoveDown();
            m_timeToNextKey = Time.time + 0.015f;
            // if we moved into empty space, just update the grid
            if (m_gameBoard.IsValidPosition(m_activeShape))
            {
                //m_gameBoard.StoreShapeInGrid(m_activeShape);
                // if we hit the bottom or another piece
            }
            else
            {
                //Debug.Log ("Invalid position for shape" + m_activeShape.name);
                // check if our end game condition is true and end the game if it is
                if (m_gameBoard.IsMaxedOut(m_activeShape))
                {
                    GameOver();
                }
                // if we didn't hit the top, then shape has come to rest
                // check the board for complete rows and respawn a new shape
                else
                {
                    LandShape();
                }
            }
            m_timeToDrop = Time.time + m_dropInterval;
        }
    }

    void LateUpdate()
    {
        if (m_ghost)
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
    }

    // handles code when a Shape hits the bottom of the board
    void LandShape()
    {
        // if we land no delay for next key press
        m_timeToNextKey = Time.time;

        // shift the piece back to the last empty space
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);


        MoveToChildren gameObjs = m_activeShape.GetComponent<MoveToChildren>();
        if (gameObjs)
            gameObjs.AttachTargets();


        // delete any rows that are full
        //m_gameBoard.RemoveCompletedRows();

        // after we revise our game to use a Coroutine
        StartCoroutine(m_gameBoard.RemoveCompletedRows());

        // play sound effects and handle scoring if we cleared any lines
        if (m_gameBoard.m_completedRows > 0)
        {

            // play completed row sound
            if (m_soundManager)
                m_soundManager.PlayClipAtPoint(m_soundManager.m_clearRowSound, Camera.main.transform.position, m_soundManager.m_fxVolume * 0.5f);

            // handle scoring and scoring effects here
            if (m_scoreManager)
            {
                m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

                // check with ScoreManager if we leveled up - we could also have used an event and event listener here
                if (m_scoreManager.didLevelUp)
                {

                    // speedUp dropInterval but make sure it never goes smaller than ...
                    m_dropInterval = Mathf.Clamp(m_startDropSpeed - ((float)m_scoreManager.M_level * 0.05f), 0.05f, 1f);

                    //Debug.Log("LEVEL UP  Drop speed = " + m_dropInterval.ToString());

                    if (m_soundManager)
                    {
                        m_soundManager.PlayClipAtPoint(m_soundManager.m_levelUpClip, Camera.main.transform.position, 1f);
                    }
                }

                // if we scored but did not level up
                else
                {
                    // play vocal effects
                    if (m_gameBoard.m_completedRows > 1)
                    {
                        if (m_soundManager)
                        {
                            m_soundManager.PlayVocals();
                        }
                        //Debug.Log("add vocal effects.  Cleared " + m_gameBoard.completedRows.ToString() + " lines");
                    }
                }
            }
        }

        // reset the Ghost shape
        if (m_ghost)
        {
            m_ghost.Reset();
        }

        // spawn a new shape and set this as our active shape
        m_activeShape = m_spawner.SpawnShape();

        // finish the cool down period for the Holder button
        if (m_holder)
        {
            m_holder.Reset();
        }

        // plays the sound of the shape dropping
        if (m_soundManager)
            m_soundManager.PlayClipAtPoint(m_soundManager.m_dropSound, Camera.main.transform.position, m_soundManager.m_fxVolume * 0.5f);
        //AudioSource.PlayClipAtPoint(m_soundManager.m_dropSound,Camera.main.transform.position,m_soundManager.m_fxVolume);
    }

    // we have maxed out... show our Game Over screen
    public void GameOver()
    {
        // shift the piece back to the last empty space
        m_activeShape.MoveUp();

        // activate the gameOver panel
        m_gameOverPanel.SetActive(true);

        m_gameOver = true;

        // play particle effects
        if (m_gameOverEffects)
        {
            Instantiate(m_gameOverEffects, m_activeShape.transform.position, m_activeShape.transform.rotation);
        }

        // play game over sounds
        if (m_soundManager)
        {
            m_soundManager.PlayGameOver();
        }

        if (m_screenFader)
        {
            m_screenFader.FadeOn();
        }
    }

    public void ToggleRotationDirection()
    {
        m_rotateClockwise = !m_rotateClockwise;

        UpdateRotateIcon();
    }


    void UpdateRotateIcon()
    {
        if (m_rotateIcon)
        {
            m_rotateIcon.transform.localScale = (m_rotateClockwise) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
    }

    // used by button to trigger 
    public void Hold()
    {

        // case1: if the holder is empty...
        if (m_holder.m_heldShape == null)
        {
            // put the active shape into the holding area
            m_holder.Catch(m_activeShape);

            // clear the active shape from the board 
            m_gameBoard.ClearShapeFromGrid(m_activeShape);

            // spawn a new shape
            m_activeShape = m_spawner.SpawnShape();

            // play the button sound here
            if (m_soundManager)
            {
                m_soundManager.PlayClipAtPoint(m_holder.buttonClip, transform.position, m_soundManager.m_fxVolume);
            }

        }
        // case2: ... if the holder is NOT empty and can release
        else if (m_holder.CanRelease())
        {
            Shape temp = m_activeShape;

            // release and move to the Spawner position
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;

            // put the active shape into the holding area
            m_holder.Catch(temp);

            // clear the active shape from the board 
            m_gameBoard.ClearShapeFromGrid(temp);

            // play the button sound here
            if (m_soundManager)
            {
                m_soundManager.PlayClipAtPoint(m_holder.buttonClip, transform.position, m_soundManager.m_fxVolume);
            }

        }
        // case3: ... the holder is NOT empty but cannot release yet
        else
        {
            //Debug.LogWarning("HOLDER WARNING:  Wait for cool down");
            // play an error sound here
            if (m_soundManager)
            {
                m_soundManager.PlayClipAtPoint(m_holder.errorClip, transform.position, m_soundManager.m_fxVolume);
                Debug.Log("Playing error sound.");
            }
        }

        // reset the ghost object whenever hold is triggered
        if (m_ghost)
            m_ghost.Reset();
    }

    // start the game again
    public void Restart()
    {
        isPaused = false;
        Time.timeScale = 1;

        if (m_soundManager)
        {
            m_soundManager.m_musicSource.volume = m_soundManager.m_musicVolume;
        }


        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(0);

    }

    // pause the game, lower the volume and show the pause panel
    public void Pause()
    {
        isPaused = !isPaused;

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(isPaused);

            if (m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
            }

            Time.timeScale = (isPaused) ? 0 : 1;
        }
    }



}
