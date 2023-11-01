using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffManager : MonoBehaviour
{
    public LevelMoves levelMoves;
    public LevelObstacles levelObstacles;
    public GridMatch3 grid;

    public bool isMoveBuffActive = false;
    public bool isRainbowBuffActive = false;
    public bool isBombBuffActive = false;

    public Button moveBuffButton;
    public Button rainbowBuffButton;
    public Button bombBuffButton;

    public int selectedNormalPieceIndex;
    private int rainbowX;
    private int rainbowY;

    public List<GamePiece> currentNormalPieces;

    public TextMeshProUGUI moveBuffCountText;
    public TextMeshProUGUI rainbowBuffCountText;
    public TextMeshProUGUI bombBuffCountText;

    public int moveBuffCount;
    private int rainbowBuffCount;
    private int bombBuffCount;


    private void Awake()
    {
        InitializeBuffAndCoins();
        moveBuffCount = PlayerPrefs.GetInt("Move Buff");
        rainbowBuffCount = PlayerPrefs.GetInt("Rainbow Buff");
        bombBuffCount = PlayerPrefs.GetInt("Bomb Buff");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCountText();
        UpdateFillAmount();

        if(isRainbowBuffActive == true)
        {
            RainbowBuffEffect();
        }
    }

    public void ActivateMoveBuff()
    {
        moveBuffCount = PlayerPrefs.GetInt("Move Buff");
        //checks if the image fill is full
        if (moveBuffCount >= 1)
        {
            isMoveBuffActive = true;
        }

    }

    public void ActivateRainbowBuff()
    {
        rainbowBuffCount = PlayerPrefs.GetInt("Rainbow Buff");
        //checks if the fill is full
        if (rainbowBuffCount >= 1)
        {
            isRainbowBuffActive = true;
            rainbowBuffCount -= 1;
            PlayerPrefs.SetInt("Rainbow Buff", rainbowBuffCount);
        }
    }

    public void ActivateBombBuff()
    {
        bombBuffCount = PlayerPrefs.GetInt("Bomb Buff");
        if (bombBuffCount >= 1)
        {
            isBombBuffActive = true;
            //after effect done reset back to default
            bombBuffCount -= 1;
            PlayerPrefs.SetInt("Bomb Buff", bombBuffCount);
        }
        
    }

    public void RainbowBuffEffect()
    {
        //ths first calls a function that returns all the piece in the GRID of a certain type
        currentNormalPieces = grid.GetPieceOfType(GridMatch3.PieceType.NORMAL);

        //after it gets a list use the Random Function to randomize the index you want to pick
        selectedNormalPieceIndex = Random.Range(0, currentNormalPieces.Count);

        //store the X and Y Value of the list Element
        rainbowX = currentNormalPieces[selectedNormalPieceIndex].X;
        rainbowY = currentNormalPieces[selectedNormalPieceIndex].Y;

        //Destroy the element at the random index
        Destroy(currentNormalPieces[selectedNormalPieceIndex].gameObject);

        //Spawn the RainBow Piece at the element Xand Y location
        grid.SpawnNewPiece(rainbowX, rainbowY, GridMatch3.PieceType.RAINBOW);

        isRainbowBuffActive = false;

    }

    public void InitializeBuffAndCoins()
    {
        //Initial Load/Save
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

    public void UpdateCountText()
    {
        moveBuffCountText.text = PlayerPrefs.GetInt("Move Buff").ToString();
        rainbowBuffCountText.text = PlayerPrefs.GetInt("Rainbow Buff").ToString();
        bombBuffCountText.text = PlayerPrefs.GetInt("Bomb Buff").ToString();
    }

    public void UpdateFillAmount()
    {
        moveBuffCount = PlayerPrefs.GetInt("Move Buff");
        if (moveBuffCount == 0)
        {
            moveBuffButton.image.fillAmount = 0;
        }
        else if (moveBuffCount >= 1)
        {
            moveBuffButton.image.fillAmount = 1;
        }

        bombBuffCount = PlayerPrefs.GetInt("Bomb Buff");
        if (bombBuffCount == 0)
        {
            bombBuffButton.image.fillAmount = 0;
        }
        else if (bombBuffCount >= 1)
        {
            bombBuffButton.image.fillAmount = 1;
        }

        rainbowBuffCount = PlayerPrefs.GetInt("Rainbow Buff");
        if (rainbowBuffCount == 0)
        {
            rainbowBuffButton.image.fillAmount = 0;
        }
        else if (rainbowBuffCount >= 1)
        {
            rainbowBuffButton.image.fillAmount = 1;
        }
    }
}
