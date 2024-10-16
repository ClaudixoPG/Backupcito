using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Variables
    public float speed = 5.5f;
    public float fireRate = 0.25f;
    public int lives = 3;
    public int shieldsAmount = 3;
    public float canFire = 0.0f; //Time to fire again
    public float shieldDuration = 5.0f;

    public GameObject BulletPref;

    //To use audio
    public AudioManager audioManager;
    public AudioSource actualAudio;

    public GameObject shield;
    public int actualWeapon = 0;

    private void Start()
    {
        shield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckBoundaries();
        ChangeWeapon();
        UseShields();
        Fire();
        //ChangeShipState();
    }


    void UseShields()
    {
        if (Input.GetKeyDown(KeyCode.Z) && shieldsAmount > 0)
        {
            //Use the shield to destroy all enemies on the screen
            shieldsAmount--;
            shield.SetActive(true);
            //Desactive player collision
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

        if (shield.activeSelf)
        {
            shieldDuration -= Time.deltaTime;
            if (shieldDuration < 0)
            {
                shield.SetActive(false);
                shieldDuration = 5.0f;
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    //Character Movement, use WASD keys to move the player
    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.right * speed * horizontalInput * Time.deltaTime);
        transform.Translate(Vector3.up * speed * verticalInput * Time.deltaTime);

    }

    void CheckBoundaries()
    {
        //Check for boundaries of the game, use Main Camera to set the boundaries
        var cam = Camera.main;
        float xMax = cam.orthographicSize * cam.aspect;
        float yMax = cam.orthographicSize;
        if (transform.position.x > xMax)
        {
            transform.position = new Vector3(-xMax, transform.position.y, 0);
        }
        else if (transform.position.x < -xMax)
        {
            transform.position = new Vector3(xMax, transform.position.y, 0);
        }
        if (transform.position.y > yMax)
        {
            transform.position = new Vector3(transform.position.x, -yMax, 0);
        }
        else if (transform.position.y < -yMax)
        {
            transform.position = new Vector3(transform.position.x, yMax, 0);
        }
    }

    //Player Fire
    void Fire()
    {
        /*if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
        {
            Instantiate(BulletPref, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            canFire = Time.time + fireRate;
            //Play the sound of the bullet
            actualAudio.Play();
        }*/

        //Change pitch audio in a random value

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
        {
            

            //change the fire method to use the bullet prefab
            switch (BulletPref.name)
            {
                case "Bullet": //Instantiate the bullet in the center
                    Instantiate(BulletPref, transform.position + new Vector3(0, 0.8f, 0), 
                        Quaternion.identity);
                    canFire = Time.time + fireRate;
                    //Play the sound of the bullet
                    actualAudio.pitch = 1;
                    actualAudio.Play();
                    break;
                case "Missile": //Instantiate 3 bullets, one in the center and the other two in diagonal, change the direction of the bullets

                    //Instantiate the bullet in the center
                    var bullet1 = Instantiate(BulletPref, transform.position + 
                        new Vector3(0, 0.8f, 0), Quaternion.identity);
                    bullet1.GetComponent<Missile>().direction = Vector2.up;

                    //Instantiate the bullet in the right
                    var bullet2 = Instantiate(BulletPref, transform.position + 
                        new Vector3(0.5f, 0.8f, 0), Quaternion.identity);
                    bullet2.GetComponent<Missile>().direction = new Vector2(0.5f, 1);

                    //Instantiate the bullet in the left
                    var bullet3 = Instantiate(BulletPref, transform.position + 
                        new Vector3(-0.5f, 0.8f, 0), Quaternion.identity);
                    bullet3.GetComponent<Missile>().direction = new Vector2(-0.5f, 1);

                    canFire = Time.time + fireRate;
                    //Play the sound of the bullet
                     
                    actualAudio.Play();
                    break;
                case "Energy Ball": //Instantiate the bullet in a wave pattern
                    Instantiate(BulletPref, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                    canFire = Time.time + fireRate;
                    //Play the sound of the bullet
                    actualAudio.pitch = Random.Range(0.5f, 1f);
                    actualAudio.Play();
                    break;
            }
        }
    }
    //public GameObject BulletPref; ----> esta es la bala que se va a disparar

    public List<Bullet> bullets;

    public void ChangeWeapon()
    {
        //For changing weapons, use the number keys 1, 2, 3

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BulletPref = bullets[0].gameObject;
            actualWeapon = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BulletPref = bullets[1].gameObject;
            actualWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BulletPref = bullets[2].gameObject;
            actualWeapon = 2;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                //Destroy the enemy
                Destroy(collision.gameObject);
                ChangeShipState();
                if (lives > 1)
                {
                    lives--;
                    Debug.Log("Lives: " + lives);
                    //change the ship state
                }
                else
                {
                    lives--;
                    //Destroy the player
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public enum ShipState
    {
        FullHealth,
        SlightlyDamaged,
        Damaged,
        HeavilyDamaged,
        Destroyed
    }

    public ShipState shipState;
    public List<Sprite> shipSprites = new List<Sprite>();
    void ChangeShipState()
    {
        var currentState = shipState;
        Debug.Log(currentState);

        //Search by name
        //var newSprite = shipSprites.Find(x => x.name == currentState.ToString());

        //Search by id
        var newSprite = shipSprites[(int)currentState];

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = newSprite;

        switch (currentState)
        {
            case ShipState.FullHealth:
                shipState = ShipState.SlightlyDamaged;
                break;
            case ShipState.SlightlyDamaged:
                shipState = ShipState.Damaged;
                break;
            case ShipState.Damaged:
                shipState = ShipState.HeavilyDamaged;
                break;
            case ShipState.HeavilyDamaged:
                shipState = ShipState.Destroyed;
                break;
            case ShipState.Destroyed:
                break;
        }

    }





}
