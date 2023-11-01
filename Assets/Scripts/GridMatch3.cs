using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMatch3 : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOW,
        COUNT,
    };


    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };

    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    }

    public int xDim;
    public int yDim;
    public float fillTime;          //time to fill the board
    private bool inverse = false;

    public Level level;

    // This will hold the piece type and gameobject together
    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    public PiecePosition[] initialPieces;

    private Dictionary<PieceType, GameObject> piecePrefabDict;

    //A 2D array to hold all the game pieces
    public GamePiece[,] pieces;


    //storing the piece we clicked on and the one we let go off
    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    private bool gameOver = false;
    private bool isFilling = false;

    public bool IsFilling
    {
        get { return isFilling; }
    }


    public BuffManager buffManagerComponent;
    public bool isBuffClicked = false;



    // Start is called before the first frame update
    void Awake()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        // this is to store the gameobject and type into a dictionary
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        //instatiate the background prefab to each cell of the grid
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x,y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        //instantiate all the game pieces in the grid
        pieces = new GamePiece[xDim, yDim];

        //spawn the initial pieces before the empty ones
        for (int i = 0; i < initialPieces.Length; i++)
        {
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }


        //looping through the colunms and ten the rows
        for (int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                //first checks if there is an existing piece in the location before spawning empty
                if(pieces[x,y] == null)
                {
                    // this creates empty pieces
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }



        //fill the board
        StartCoroutine(Fill());
    }

    // Update is called once per frame
    void Update()
    {
        if(buffManagerComponent.isBombBuffActive == true)
        {
            isBuffClicked = true;
        }
        else if (buffManagerComponent.isBombBuffActive == false)
        {
            isBuffClicked = false;
        }
    }

    //This fills the board by calling FillStep() until the board is full
    public IEnumerator Fill()
    {
        Debug.Log("Fill");
        bool needsRefill = true;
        isFilling = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep() || buffManagerComponent.isBombBuffActive == true)
            {
                buffManagerComponent.isBombBuffActive = false;
                Debug.Log("FillStep");
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }

            needsRefill = ClearAllValidMatches();
        }

        isFilling = false;
    }

    ///this will only move each piece by one space
    public bool FillStep()
    {
        //return true if any pieces were moved
        bool movedPiece = false;
        
        for (int y = yDim-2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                //added loopX for diagonal filling to fill between block pieces
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];
                if(piece.IsMoveable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if(pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MoveableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else //move diagonal
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if(inverse)
                                {
                                    diagX = x - diag;
                                }

                                if(diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if(pieceAbove.IsMoveable())
                                            {
                                                break;
                                            }
                                            else if(!pieceAbove.IsMoveable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MoveableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];
            if(pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);

                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MoveableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;

    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim/2.0f + x, transform.position.y + yDim/2.0f - y);
    } 

    //THIS SPAWNS A NEW PIECE
    //takes the x and y coordinates of the grid and the piece type
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        //this instatiates a new GameObject
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        //stores the gamepiece component in the pieces array
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }

    //checks if there is a candy next to the object in row or column
    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    //swaps the 2 pieces if they are adjacent
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)   //if the game is over stop the player from swaping pieces
        {
            return;
        }

        if (piece1.IsMoveable() && piece2.IsMoveable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            //checks if the swapped pieces create a match
            if(GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                || piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW) //if the piece is matcing with a rainboww piece
            {

                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MoveableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MoveableComponent.Move(piece1X, piece1Y, fillTime);

                //cheacks for piece 1 Rainbow piece
                if(piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if(clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }

                //cheacks for piece 2 Rainbow piece
                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }


                //clear other matches
                ClearAllValidMatches();

                if(piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }


                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }

                pressedPiece = null;
                enteredPiece = null;

                //fill the empty spaces again
                StartCoroutine(Fill());
                //tells the game that you have move a piece
                level.OnMove();
            }
            else
            {
                //if theres is no match we put the pieces back in their original positions
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }

        }
    }

    //assignes the state(pressed or entered) to the piece
    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }
    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    //This function returns a list of the moving piece will match with
    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        //checks if the piece is colored
        if(piece.IsColored())
        {
            //stores a list of adjancent possible matches
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //tranverse the list first horizontally left and right, then vertically
            horizontalPieces.Add(piece);
            //loop that changes the direction we are going
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0) //this means we are going left
                    {
                        x = newX - xOffset;
                    }
                    else // this means we are going right
                    {
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }

                    //we check if the adjecent piece is a match
                    //we are traversing horizontally right now
                    if(pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        //if the piece is a match we add it to the horizontal pieces list
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else //otherwise we hace run into a piece that is not a match
                    {
                        break;
                    }
                }
            }

            //checks if there are the minimum number of pieces for a match
            if(horizontalPieces.Count >= 3)
            {
                //we added the pieces to the matching pieces list
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            //after finding an horizontal match of 3 we now need to find a vertical match that share the same piece
            //TLDR an L shape

            //tranverse vertically and check for L or T
            if (horizontalPieces.Count >= 3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    for(int dir = 0; dir <= 1; dir++)
                    {
                        for(int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;

                            if(dir == 0) // Check Up
                            {
                                y = newY - yOffset;
                            }
                            else // if not check down
                            {
                                y = newY + yOffset;
                            }

                            if(y < 0 || y >= yDim)
                            {
                                break;
                            }

                            //this check if the adjascent piece is colored and it matches
                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else //if it doesnt match
                            {
                                break;
                            }
                        }
                    }

                    //checks if thers is enogh vertical piece to form a match
                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else //if enough pieces for a match and them to the matching pieces list
                    {
                        for(int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
            }


            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }



            //After going horizontally, we didnt find any matches
            //so now we go vertically

            horizontalPieces.Clear();
            verticalPieces.Clear();

            verticalPieces.Add(piece);
            //loop that changes the direction we are going
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0) //this means we are going up
                    {
                        y = newY - yOffset;
                    }
                    else // this means we are going down
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    //we check if the adjecent piece is a match
                    //we are traversing horizontally right now
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        //if the piece is a match we add it to the horizontal pieces list
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else //otherwise we hace run into a piece that is not a match
                    {
                        break;
                    }
                }
            }


           
            //checks if there are the minimum number of pieces for a match
            if (verticalPieces.Count >= 3)
            {
                //we added the pieces to the matching pieces list
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }



            //tranverse horizontally and check for L or T
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;

                            if (dir == 0) // Check Left
                            {
                                x = newX - xOffset;
                            }
                            else // if not check Right
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }

                            //this check if the adjascent piece is colored and it matches
                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                //fixed code bug here
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else //if it doesnt match
                            {
                                break;
                            }
                        }
                    }

                    //checks if thers is enogh vertical piece to form a match
                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else //if enough pieces for a match and them to the matching pieces list
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }

                        break;
                    }
                }
            }



            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

        }

        //no match was found with this piece
        return null;
    }

    public bool ClearPiece(int x, int y)
    {
        if (pieces[x,y].IsClearable() && !pieces[x,y].ClearableComponent.IsBeingCleared)
        {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            ClearObstacles(x,y);
            FindObjectOfType<HUD>().gameSoundSource.PlayOneShot(FindObjectOfType<HUD>().matchSound, 0.1f);
            return true;
        }

        return false;
    }

    public bool ClearAllValidMatches() //clear other matches too and refills the board
    {
        bool needsRefill = false;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x,y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if(match != null)
                    {
                        //determine if we should spawn a special piece and what type a piece it should be
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        if (match.Count == 4)
                        {
                            if(pressedPiece == null || enteredPiece == null)
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
                            }
                            else if(pressedPiece.Y == enteredPiece.Y)
                            {
                                //if the pieces are in the same row then the piece will be a row clearing piece
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else
                            {//if in the same colum it will be a coluunm clearing piece
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                        }
                        else if(match.Count == 5) // checks if there are five matching pieces to create a rainbow piece
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if(ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if(match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }

                        if(specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR) && newPiece.IsColored() && match[0].IsColored())
                            {
                                newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }
                            else if(specialPieceType == PieceType.RAINBOW && newPiece.IsColored()) //sets the color of the rainbow piece to any
                            {
                                newPiece.ColorComponent.SetColor(ColorPiece.ColorType.ANY);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    public void ClearObstacles(int x, int y)
    {
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if(adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BUBBLE && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if(pieces[x, adjacentY].Type == PieceType.BUBBLE && pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int row)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }

    public void ClearColor(ColorPiece.ColorType color)
    {
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                if(pieces[x,y].IsColored() && (pieces[x,y].ColorComponent.Color == color || color == ColorPiece.ColorType.ANY))
                {
                    ClearPiece(x,y);
                }
            }
        }
    }

    public void GameOver()
    {
        gameOver = true;
    }

    public List<GamePiece> GetPieceOfType(PieceType type)
    {
        //returns a list of all the pieces on the board of a given type
        List<GamePiece> piecesOfType = new List<GamePiece>();

        for (int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                if(pieces[x,y].Type == type)
                {
                    piecesOfType.Add(pieces[x, y]);
                }
            }
        }
        return piecesOfType;
    }



    public IEnumerator FillBomb()
    {
        bool needsRefill = true;
        isFilling = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStepForBomb())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
                buffManagerComponent.isBombBuffActive = false;
            }

            needsRefill = ClearAllValidMatches();
        }

        isFilling = false;
    }

    public bool FillStepForBomb()
    {
        //return true if any pieces were moved
        bool movedPiece = false;

        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                //added loopX for diagonal filling to fill between block pieces
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];
                if (piece.IsMoveable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MoveableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else //move diagonal
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMoveable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMoveable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MoveableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);

                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MoveableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;

    }
}
