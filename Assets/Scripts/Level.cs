using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType  //Type of level for other level class to derive from
    {
        TIMER,
        OBSTACLE,
        MOVES,
    };

    public GridMatch3 grid;
    public HUD hud;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    protected LevelType type;

    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;
    protected bool didWin;



    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        hud.SetScore(currentScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GameWin()
    {
        //Debug.Log("You Win");
        grid.GameOver();
        didWin = true;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void GameLose()
    {
        //Debug.Log("You Lose");
        grid.GameOver();
        didWin = false;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void OnMove()
    {
        //Debug.Log("You Moved");
    }

    public virtual void OnPieceCleared(GamePiece piece)
    {
        //this is where we update score
        //increment the score based on each pieces score
        currentScore += piece.score;
        hud.SetScore(currentScore);
    }

    //waits for game to fill before calling the GameOver or GameWin panel
    protected virtual IEnumerator WaitForGridFill()
    {
        while (grid.IsFilling)
        {
            yield return 0;
        }

        yield return new WaitForSeconds(1.0f);
        if(didWin)
        {
            hud.OnGameWin(currentScore);
        }
        else
        {
            hud.OnGameLose();
        }
    }

}
