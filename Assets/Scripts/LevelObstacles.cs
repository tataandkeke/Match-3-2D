using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacles : Level
{
    public int numMoves;
    public GridMatch3.PieceType[] obstacleTypes;

    public int movesUsed = 0;
    private int numObstaclesLeft;

    public bool isLevelObstacle = false;

    private void Awake()
    {
        isLevelObstacle = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.OBSTACLE;

        //loop through all the obstacles and add them to list
        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            //increment the number of obstacles left by the pieces in the list
            numObstaclesLeft += grid.GetPieceOfType(obstacleTypes[i]).Count;
        }

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(numObstaclesLeft);
        hud.SetRemaining(numMoves);
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<BuffManager>().isMoveBuffActive == true)
        {
            MoveBuffCheck();
        }
    }

    public override void OnMove()
    {
        movesUsed++;
        hud.SetRemaining(numMoves - movesUsed);

        if (numMoves - movesUsed == 0 && numObstaclesLeft > 0)
        {
            GameLose();
        }
    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        //loops through the obstacle types and checks if this piece matches any of them
        for(int i = 0; i < obstacleTypes.Length; i++)
        {
            if(obstacleTypes[i] == piece.Type)      //if the cleared piece is an obstacle we decrease the number of obstacles
            {
                numObstaclesLeft--;
                hud.SetTarget(numObstaclesLeft);

                if (numObstaclesLeft == 0)          //checks if all the obstacle has been cleared
                {
                    currentScore += 1000 * (numMoves - movesUsed);  //give bonus score for moves left

                    if (currentScore >= score1Star)
                    {
                        hud.SetScore(currentScore);
                        GameWin();
                    }
                    else if(currentScore < score1Star)
                    {
                        hud.SetScore(currentScore);
                        GameLose();
                    }
                }
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
