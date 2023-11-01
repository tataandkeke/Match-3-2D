using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int score;    //score variable
    private int x;
    private int y;

    public int X
    {
        get {return x;}
        set //can only set a new value if it is moveable
        {
            if(IsMoveable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get {return y;}
        set //can only set a new value if it is moveable
        {
            if (IsMoveable())
            {
                y = value;
            }
        }
    }

    private GridMatch3.PieceType type;

    public GridMatch3.PieceType Type
    {
        get {return type;}
    }

    private GridMatch3 grid;

    public GridMatch3 GridRef
    {
        get { return grid;}
    }

    private MoveablePiece moveableComponent;
    //this is to help check if an object has a moveable component
    public MoveablePiece MoveableComponent
    {
        get { return moveableComponent; }
    }

    private ColorPiece colorComponent;
    //this is the reference to the color piece compnent
    public ColorPiece ColorComponent
    {
        get { return colorComponent; }
    }


    //reference to cleareable piece component
    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent
    {
        get { return clearableComponent; }
    }

    private BuffManager buffManagerComponent;
    public BuffManager BuffManagerComponent
    {
        get { return buffManagerComponent; }
    }
    private void Awake()
    {
        //gets and check if an object has the moveable script
        moveableComponent = GetComponent<MoveablePiece>();
        //gets a reference to the ColorPiece component
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
        buffManagerComponent = GetComponent<BuffManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Initialize some variables, takes the x,y position, the grid and the piecetype
    public void Init(int _x, int _y, GridMatch3 _grid, GridMatch3.PieceType _type)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;
    }

    private void OnMouseEnter()
    {
        grid.EnterPiece(this);
    }

    private void OnMouseDown()
    {
        grid.PressPiece(this);
        if (grid.isBuffClicked == true)
        {
            clearableComponent.Clear();
            grid.SpawnNewPiece(X, Y, GridMatch3.PieceType.EMPTY);
            grid.StartCoroutine(grid.Fill());
        }
        
    }

    private void OnMouseUp()
    {
        grid.ReleasePiece();
    }

    //bool to check if it is moveable
    public bool IsMoveable()
    {
        return moveableComponent != null; //returns true if it is there, false otherwise
    }

    //checks if a piece is colored
    public bool IsColored()
    {
        return colorComponent != null;
    }

    //checks if the piece can be cleared
    public bool IsClearable()
    {
        return clearableComponent != null;
    }
}
