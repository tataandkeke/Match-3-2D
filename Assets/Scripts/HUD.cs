using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public Level level;
    public GameOver gameOver;
    public Energy energy;

    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI targetSubText;
    public TextMeshProUGUI scoreText;
    public Image[] stars;

    private int starIdx = 0;
    private int goldCoinAmount;

    //sound
    public AudioSource gameSoundSource;
    public AudioClip matchSound;



    private void Awake()
    {
        goldCoinAmount = PlayerPrefs.GetInt("Gold Coin");
    }
    // Start is called before the first frame update
    void Start()
    {
        //loops through the stars and activates the 0 star on start
        for (int i = 0; i < stars.Length; i++)
        {
            if(i == starIdx)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }

        

      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        int visibleStar = 0;

        //checks the score to see which star to display
        if (score >= level.score1Star && score < level.score2Star)
        {
            visibleStar = 1;
        }
        else if (score >= level.score2Star && score < level.score3Star)
        {
            visibleStar = 2;
        }
        else if (score >= level.score3Star)
        {
            visibleStar = 3;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (i == visibleStar)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }

        starIdx = visibleStar;
    }

    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }

    public void SetRemaining(int remaining)      //sets remaining obstacle and moves int variables for obstacle/move levels
    {
        remainingText.text = remaining.ToString();
    }

    public void SetRemaining(string remaining)   //sets remaining time for the time levels
    {
        remainingText.text = remaining;
    }

    //sets the remainSubText and targetSubText variable depending on the level type
    public void SetLevelType(Level.LevelType type)
    {
        if (type == Level.LevelType.MOVES)
        {
            remainingSubText.text = "Moves remaining";
            targetSubText.text = "Target Score";
        }
        else if (type == Level.LevelType.OBSTACLE)
        {
            remainingSubText.text = "Moves remaining";
            targetSubText.text = "Bubbles remaining";
        }
        else if (type == Level.LevelType.TIMER)
        {
            remainingSubText.text = "Time remaining";
            targetSubText.text = "Target score";
        }
    }

    public void OnGameWin(int score)
    {
        IncreaseGoldCoinAmount();
        gameOver.ShowWin(score, starIdx);
        //store highest star value
        if(starIdx > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, starIdx);
        }
    }

    public void OnGameLose()
    {
        energy.UseEnergy();
        gameOver.ShowLose();
    }

    public void IncreaseGoldCoinAmount()
    {
        Debug.Log("increase");
        goldCoinAmount++;
        PlayerPrefs.SetInt("Gold Coin", goldCoinAmount);
    }

    public void DecreaseGoldAmount()
    {
        if (goldCoinAmount > 0)
        {
            goldCoinAmount--;
            PlayerPrefs.SetInt("Gold Coin", goldCoinAmount);
        }

    }

}
