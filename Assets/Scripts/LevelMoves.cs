using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    public int numMoves;
    public int targetScore;

    public int movesUsed = 0;
    public bool isLevelMove = false;


    private void Awake()
    {
        isLevelMove = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.MOVES;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(targetScore);
        hud.SetRemaining(numMoves);


    }

    // Update is called once per frame
    void Update()
    {
        if(FindObjectOfType<BuffManager>().isMoveBuffActive == true)
        {
            MoveBuffCheck();
        }
        
    }

    public override void OnMove()
    {
        //increments move used when you swap a piece
        movesUsed++;
        hud.SetRemaining(numMoves - movesUsed);

        //checks if you have moved the same time as the numberof moves allowed
        if (numMoves - movesUsed == 0)
        {
            //checks first if the current score is more than the target score
            if (currentScore > targetScore)
            {
                GameWin();      //you have won
            }
            else 
            {
                GameLose();     //you have lost
            }
        }
    }

    public void MoveBuffCheck()
    {
        hud.SetRemaining((numMoves - movesUsed) + 5);
        numMoves += 5;
        //after effect done reset back to default
        FindObjectOfType<BuffManager>().moveBuffCount -= 1;
        PlayerPrefs.SetInt("Move Buff", FindObjectOfType<BuffManager>().moveBuffCount);
        FindObjectOfType<BuffManager>().isMoveBuffActive = false;
    }
}
