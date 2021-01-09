using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int enemyDamage = 1;
    public float restartLevelDelay = 1f;
    public Text foodText;

    private int food;
    private GameObject[] enemies;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private int nbCombo = 0;
    private bool wait = false;

    protected override void Start()
    {   
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: "+ food;
        base.Start();
    }

    private void OnDisable(){
        GameManager.instance.playerFoodPoints = food;
    }

    void Update()
    {
        if( !GameManager.instance.playersTurn || wait ){
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if( horizontal != 0 ){
            vertical = 0;
        }

        if( horizontal != 0 || vertical != 0 ){
            
            bool alreadyMove = false;

            if( horizontal < 0 ){
                spriteRenderer.flipX = true;
            }else if ( horizontal > 0 ){
                spriteRenderer.flipX = false;
            }

            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemie in enemies){
                if( enemie.transform.position.x == (transform.position.x + horizontal) && enemie.transform.position.y == (transform.position.y + vertical) ){
                    wait = true;
                    alreadyMove =  AttemptMove<Enemy>(horizontal,vertical);
                }
            } 
            
            if ( !alreadyMove ){
                nbCombo = 0;
                wait  = true;
                AttemptMove<Wall>(horizontal,vertical);
                GameManager.instance.movePosition.Add(transform.position+new Vector3(horizontal,vertical,0f));
            }
        }
    }

    protected override bool AttemptMove <T> (int xDir, int yDir)
    {
        food--;
        foodText.text  = "Food: "+food;
        bool moved = base.AttemptMove <T> (xDir, yDir);

        if( moved ){
            SoundManager1.instance.RandomizeSfx(moveSound1,moveSound2);
            GameManager.instance.playersTurn = false;
            wait = false;
        }else{
            StartCoroutine(waitAnimation());
        }

        CheckIfGameOver();

        return true;
    }

    private void OnTriggerEnter2D( Collider2D other)
    {
        if( other.tag == "Exit"){
            Invoke ("Restart",restartLevelDelay);
            enabled = false;
        }else if(other.tag == "Soda"){
            food += pointsPerSoda;
            foodText.text =  "+" + pointsPerSoda + " Food: " + food;
            SoundManager1.instance.RandomizeSfx(drinkSound1,drinkSound2);
            other.gameObject.SetActive(false);
        }else if(other.tag == "Food"){
            food += pointsPerFood;
            foodText.text =  "+ " + pointsPerFood + " Food: " + food;
            SoundManager1.instance.RandomizeSfx(eatSound1,eatSound2);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove <T> (T component,Vector2 end)
    {   
        base.OnCantMove(component,end);    
        if ( component.GetType() == typeof(Enemy)){
            nbCombo++;
            string attack = "playerAttack1";
            switch (nbCombo)
            {
            case 1:
                attack = "playerAttack1";
                break;
            case 2:
                attack = "playerAttack2";
                break;
            default:
                attack = "playerAttack3";
                break;
            }
            Enemy enemie = component as Enemy;
            enemie.EnemyDamage(enemyDamage);
            animator.SetTrigger(attack);
        }else{
            Wall hitWall = component as Wall;
            hitWall.WallDamage(wallDamage);
            animator.SetTrigger("playerAttack1");
        }
    }

    IEnumerator waitAnimation(){
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.playersTurn = false;
        wait = false;
    }


    public void LoseFood(int loss){
        animator.SetTrigger("playerHit");
        base.flash();
        food -= loss;
        foodText.text = "- " + loss + " Food: " + food;
        CheckIfGameOver();
    }

    public void Restart()
    {       
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }


    private void CheckIfGameOver(){
        if( food <= 0 ){
            SoundManager1.instance.PlaySingle(gameOverSound);
            SoundManager1.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
