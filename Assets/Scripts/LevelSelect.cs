using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelect : MonoBehaviour
{

    [System.Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject gameObject;
        public string playerPrefKey;
    }

    public GameObject spinPanel;
    public GameObject rewardedPanel;

    // Energy System
    public Energy energy;

    //GoldCoins
    public TextMeshProUGUI goldText;


    public ButtonPlayerPrefs[] buttons;

    // level stars
    private int firstLevelStars;
    private int secondLevelStars;
    private int thirdLevelStars;
    private int fourthLevelStars;
    private int fifthLevelStars;

    private int sixthLevelStars;
    private int sevenLevelStars;
    private int eightLevelStars;
    private int ninthLevelStars;
    private int tenthLevelStars;

    private int totalLevelStars;

    public TextMeshProUGUI totalStarsText;
    public TextMeshProUGUI allStarsText;

    public Animator world1LevelAnim;
    public Animator world2LevelAnim;



    // Start is called before the first frame update
    void Start()
    {
        GetLevelStarPrefs();
        InitializeBuffAndCoins();

        //assigns star value based on star high score
        for (int i = 0; i < buttons.Length; i++)
        {
            int score = PlayerPrefs.GetInt(buttons[i].playerPrefKey, 0);

            for (int starIdx = 1; starIdx <=3; starIdx++)
            {
                Transform star = buttons[i].gameObject.transform.Find("Star" + starIdx);

                if(starIdx <= score)
                {
                    star.gameObject.SetActive(true);
                }
                else
                {
                    star.gameObject.SetActive(false);
                }
            }
        }

        totalStarsText.text = PlayerPrefs.GetInt("Total Level Stars").ToString() + "/10";
        allStarsText.text = PlayerPrefs.GetInt("Total Level Stars").ToString();




    }

    // Update is called once per frame
    void Update()
    {
        goldText.text = PlayerPrefs.GetInt("Gold Coin").ToString();
    }

    public void OnButtonPress(string levelName)
    {
        if (energy.currentEnergy > 0)
        {
            SceneManager.LoadScene(levelName);
        }
    }


    public void InitializeBuffAndCoins()
    {
        //Initial Load/Save
        if (!PlayerPrefs.HasKey("Gold Coin"))
        {
            PlayerPrefs.SetInt("Gold Coin", 0);
        }
        else
        {
            PlayerPrefs.GetInt("Gold Coin");
        }


        if (!PlayerPrefs.HasKey("Move Buff"))
        {
            PlayerPrefs.SetInt("Move Buff", 0);
        }
        else
        {
            PlayerPrefs.GetInt("Move Buff");
        }



        if (!PlayerPrefs.HasKey("Rainbow Buff"))
        {
            PlayerPrefs.SetInt("Rainbow Buff", 0);
        }
        else
        {
            PlayerPrefs.GetInt("Rainbow Buff");
        }


        if (!PlayerPrefs.HasKey("Bomb Buff"))
        {
            PlayerPrefs.SetInt("Bomb Buff", 0);
        }
        else
        {
            PlayerPrefs.GetInt("Bomb Buff");
        }
    }
   
    public void EnableWheelPanel()
    {
        spinPanel.SetActive(true);
    }

    public void DisableWheelPanel()
    {
        if(FindObjectOfType<SpinWheelScript>().isSpinning == false)
        {
            rewardedPanel.SetActive(false);
            spinPanel.SetActive(false);
        }
        
    }

    public void GetLevelStarPrefs()
    {
        totalLevelStars = PlayerPrefs.GetInt("Total Level Stars", 0);
        firstLevelStars = PlayerPrefs.GetInt("Level1 Moves");
        secondLevelStars = PlayerPrefs.GetInt("Level2 Obstacles");
        thirdLevelStars = PlayerPrefs.GetInt("Level3 Timer");
        fourthLevelStars = PlayerPrefs.GetInt("Level4 Moves");
        fifthLevelStars = PlayerPrefs.GetInt("Level5 Moves", 0);
        sixthLevelStars = PlayerPrefs.GetInt("Level6 Timer", 0);
        sevenLevelStars = PlayerPrefs.GetInt("Level7 Obstacles", 0);
        eightLevelStars = PlayerPrefs.GetInt("Level8 Timer", 0);
        ninthLevelStars = PlayerPrefs.GetInt("Level9 Obstacles", 0);
        tenthLevelStars = PlayerPrefs.GetInt("Level10 Moves");

        totalLevelStars = firstLevelStars + secondLevelStars + thirdLevelStars + fourthLevelStars + fifthLevelStars + sixthLevelStars + sevenLevelStars + eightLevelStars + ninthLevelStars + tenthLevelStars;
        PlayerPrefs.SetInt("Total Level Stars", totalLevelStars);
    }

    public void OnNextWorld()
    {
        if(PlayerPrefs.GetInt("Total Level Stars") >= 10)
        {
            world1LevelAnim.SetTrigger("Hide");
            world2LevelAnim.SetTrigger("Show");
            //restof the function

        }
        

    }

    public void OnBackLevel()
    {
        world1LevelAnim.SetTrigger("Show");
        world2LevelAnim.SetTrigger("Hide");
    }
}
