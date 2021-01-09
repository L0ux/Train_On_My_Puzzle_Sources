using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

    public float moveTime = .1f;
    public LayerMask blockingLayer;

    protected BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody2D;
    public SpriteRenderer spriteRenderer;
    protected Animator animator;
    private float inverseMoveTime;


    protected virtual void Start()
    {   
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move ( int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir,yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start,end,blockingLayer);
        boxCollider.enabled = true;


        if( hit.transform == null ){ 
            animator.SetTrigger("move");
            StartCoroutine(SmoothMovement(end));
            return true;
        } 
        return false;
    }

    protected virtual bool AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        
        RaycastHit2D hit;

        bool canMove = Move(xDir,yDir,out hit);

        if( hit.transform == null ){ 
            return true;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if( !canMove &&  hitComponent != null ){
            OnCantMove(hitComponent,endDirection(xDir,yDir)); 
        } 

        return false;
    }

    protected IEnumerator SmoothMovement ( Vector3 end )
    {
        float sqrRemainingDistance = ( transform.position - end ).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon){
            Vector3 newPosition = Vector3.MoveTowards( rigidBody2D.position,end,inverseMoveTime * Time.deltaTime);

            rigidBody2D.MovePosition(newPosition);

            sqrRemainingDistance = ( transform.position - end ).sqrMagnitude;

            yield return null;

        }
        transform.position = new Vector3(Mathf.Round(transform.position.x),Mathf.Round(transform.position.y),0f);
    }

    protected IEnumerator damageMovement ( Vector3 end ){
        Vector3 originalPosition = this.transform.position;

        float sqrRemainingDistance = ( transform.position - end ).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon){
            Vector3 newPosition = Vector3.MoveTowards( rigidBody2D.position,end,inverseMoveTime * Time.deltaTime);

            rigidBody2D.MovePosition(newPosition);

            sqrRemainingDistance = ( transform.position - end ).sqrMagnitude;

            yield return null;

        }

        sqrRemainingDistance = ( transform.position - originalPosition ).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon){
            Vector3 newPosition = Vector3.MoveTowards( rigidBody2D.position,originalPosition,inverseMoveTime * Time.deltaTime);

            rigidBody2D.MovePosition(newPosition);

            sqrRemainingDistance = ( transform.position - originalPosition ).sqrMagnitude;

            yield return null;

        }
    }

    protected Vector3 Pathfinding(List<Vector3> mouvements,Vector3 target){
        Vector3 bestMouvement = new Vector3(-10000,-10000,0f);
        foreach( Vector3 mouvement in mouvements ){
            if( Vector3.Distance(mouvement,target) < Vector3.Distance(bestMouvement,target) ){
                bestMouvement = mouvement;
            }
        }
        if ( bestMouvement == (new Vector3(-10000,-10000,0f)) ){
            return new Vector3(0,0,0);
        }else{
            return bestMouvement;
        }
    }

    public void flash(){  //Fait clignoter l'objet
        StartCoroutine(cFlash());
    }
    
    IEnumerator cFlash(){

        for(var n = 0; n < 5; n++){
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.01f);
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.01f);
        }
        spriteRenderer.enabled = true;
    }

    protected virtual void OnCantMove <T> ( T component,Vector2 end)
        where T : Component
    {
        StartCoroutine(damageMovement(end));
    }

    protected Vector2 endDirection(int xDir,int yDir){
        Vector2 start = transform.position;
        Vector2 end = new Vector2(0f,0f);
        if( xDir == 1 ){
            end = start + new Vector2(xDir-0.5f,0f);
        }else if( yDir == 1 ){
            end = start + new Vector2(0f,yDir-0.5f);
        }else if( yDir == -1 ){
            end = start + new Vector2(0f,yDir+0.5f);
        }else if( xDir== -1 ){
            end = start + new Vector2(xDir+0.5f,0f);
        }
        return end;
    }

}
