using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLinePiece : ClearablePiece
{
    public bool isRow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //overrids the Clear function on the base class
    public override void Clear()
    {
        base.Clear();

        if (isRow)  //checks if it is a row piece
        {
            //clear row
            piece.GridRef.ClearRow(piece.Y);
        }
        else
        {
            //clear column
            piece.GridRef.ClearColumn(piece.X);
        }
    }
}
