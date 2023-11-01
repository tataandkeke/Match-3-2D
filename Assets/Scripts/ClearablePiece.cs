using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    public AnimationClip clearAnimation;

    private bool isBeingCleared = false;
    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    //needs acces to the game piece we are attached too
    protected GamePiece piece;


    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Clear()
    {
        piece.GridRef.level.OnPieceCleared(piece);  //this increments the score
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length); //waits for full animation to play
            Destroy(gameObject);
        }
    }
}
