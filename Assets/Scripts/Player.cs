using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int type;

    [Header("Player Stats")]
    [SerializeField] float speed;
    public float health;
    public float maxHealth;

    [Space(2)]
    [Header("Player Objects")]
    public GameObject boomerang;
    public Boomerang thrownBoomerang;
    [SerializeField] GameObject bodyRotation;
    [SerializeField] GameObject name;
    public GameObject healthBar;
    [SerializeField] GameObject barResizer;
    [SerializeField] GameObject shoulder;
    GameObject shootTarget;

    public GameObject Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public GameObject HealthBar
    {
        get
        {
            return healthBar;
        }
        set
        {
            healthBar = value;
        }
    }

    public GameObject BarResizer
    {
        get
        {
            return barResizer;
        }
        set
        {
            barResizer = value;
        }
    }

    public Vector2 onlineNewPos;
    public Vector2 onlineOldPos;

    [Space(2)]
    [Header("Player Components")]
    public Animator armsAnimator;
    Animator animator;
    Rigidbody2D rb;

    //Other variables
    public GameObject target;
    bool canFollowTarget = false;
    bool canShoot = true;
    Vector2 boomerangTarget;

    public int team;

    public int Team
    {
        get
        {
            return team;
        }
        set
        {
            team = value;
        }
    }

    public int playerNumber;

    public int PlayerNumber
    {
        get
        {
            return playerNumber;
        }
        set
        {
            playerNumber = value;
        }
    }

    public bool controllable;

    public bool Controllable
    {
        get
        {
            return controllable;
        }
        set
        {
            controllable = value;
        }
    }

    public bool controlling;

    public bool Controlling
    {
        get
        {
            return controlling;
        }
        set
        {
            controlling = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //Get Components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        target = new GameObject();
        shootTarget = new GameObject();
        shootTarget.AddComponent<BoxCollider2D>();
        shootTarget.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
        shootTarget.AddComponent<PlayerTarget>();
        shootTarget.GetComponent<PlayerTarget>().playerNumber = playerNumber;
        shootTarget.GetComponent<BoxCollider2D>().isTrigger = true;
        colors = new List<Color32>();
        foreach(SpriteRenderer limb in limbs)
        {
            colors.Add(limb.color);
        }

        foreach(Player player in FindObjectsOfType<Player>())
        {
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), thrownBoomerang.GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(player.thrownBoomerang.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(player.thrownBoomerang.GetComponent<BoxCollider2D>(), thrownBoomerang.GetComponent<BoxCollider2D>());

        }

        maxHealth = health;

        thrownBoomerang = Instantiate(thrownBoomerang.gameObject).GetComponent<Boomerang>();
        thrownBoomerang.parentHand = shoulder;
        thrownBoomerang.parent = GetComponent<Player>();

        if (type == 1)
            armsAnimator.speed = 1.25f;
        else if (type == 3)
            armsAnimator.speed = 2f;
    }

    public float timeAlive = 0;

    public Vector3 movementDirection;

    public float interpolationTime = -1;

    public Vector3 interpolationCurrentPos;

    public Vector3 interpolationTarget;

    // Update is called once per frame
    void Update()
    {
        onlineOldPos = onlineNewPos;
        onlineNewPos = transform.position;

        timeAlive += Time.deltaTime;

        if (FindObjectOfType<GameHost>() == null && controllable && timeAlive > 1)
        {
            transform.position += movementDirection * Time.deltaTime;
            
            if (interpolationTime != -1 && controllable)
            {
                interpolationTime += Time.deltaTime;

                float time = interpolationTime / (0.05f);
                if (time > 1)
                    time = 1;

                transform.position = Vector3.Lerp(interpolationCurrentPos, interpolationTarget, time);

                if (time == 1)
                    interpolationTime = -1;
            }
        }


        if (controlling)
        {
            if (Input.GetMouseButton(1))
            {
                FindObjectOfType<GameClient>().Send("PlayerTarget " + playerNumber + " " + CursorPos.position.x + " " + CursorPos.position.y + "\n");
            }

            if (Input.GetMouseButton(0))
            {
                FindObjectOfType<GameClient>().Send("Shoot " + playerNumber + " " + CursorPos.position.x + " " + CursorPos.position.y + "\n");
            }
        }

        if(controllable && FindObjectOfType<GameHost>() != null)
        {
            if(thrownBoomerang.gameObject.activeInHierarchy == false)
                thrownBoomerang.transform.position = boomerang.transform.position;
            if (target != null)
            {
                if (Vector2.Distance(transform.position, target.transform.position) < 0.25f)
                {
                    canFollowTarget = false;
                    Idle();
                }
                else if(canFollowTarget)
                {
                    RotatePlayer(target.transform.position);
                    GoForward();
                }
            }
        }
    }

    public void ShootBoom(float x, float y)
    {
        if (canShoot && boomerang.activeInHierarchy)
        {
            Throw(x, y);
            canShoot = false;
        }
    }

    public void NewTarget(float x, float y)
    {
        GetNewTarget(x,y);
        canFollowTarget = true;
    }

    //Methods

    /// <summary>
    /// Gets new target
    /// </summary>
    void GetNewTarget(float x, float y)
    {
        target.transform.position = new Vector2(x,y);
    }

    /// <summary>
    /// Looks towards target position
    /// </summary>
    /// <param name="targetPosition">The position to look at</param>
    void RotatePlayer(Vector2 targetPosition)
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;
        diff.Normalize();

        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, rot + 90);
    }

    /// <summary>
    /// Makes player go forward
    /// </summary>
    void GoForward()
    {
        rb.velocity = -transform.up * speed;
        animator.Play("run");
    }

    /// <summary>
    /// Makes player go idle
    /// </summary>
    void Idle()
    {
        rb.velocity = Vector2.zero;
        animator.Play("idle");
    }

    /// <summary>
    /// Throws boomerang at mouse position
    /// </summary>
    void Throw(float x, float y)
    {
        shootTarget.transform.position = new Vector2(x, y);
        armsAnimator.Play("throw");
        boomerangTarget = new Vector2(x,y);
    }

    public void ThrowBoomerang()
    {
        boomerang.SetActive(false);
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), thrownBoomerang.GetComponent<BoxCollider2D>());
        thrownBoomerang.transform.position = boomerang.transform.position;
        thrownBoomerang.transform.eulerAngles = boomerang.transform.eulerAngles;
        thrownBoomerang.gameObject.SetActive(true);
        thrownBoomerang.transform.parent = null;
        thrownBoomerang.GoAt(boomerangTarget);
    }

    public void BoomerangReturned()
    {
        boomerang.SetActive(true);
    }

    /// <summary>
    /// Idles arms
    /// </summary>
    public void ArmsIdle()
    {
        canShoot = true;
        bodyRotation.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public bool HasBoomerang()
    {
        return boomerang.activeInHierarchy;
    }

    public List<SpriteRenderer> limbs;
    List<Color32> colors;

    public void DecreaseHp(float value)
    {
        health -= value;

        barResizer.transform.localScale = new Vector3(health / maxHealth, 1);

        CancelInvoke("ResetColors");

        foreach (SpriteRenderer limb in limbs)
            limb.color = new Color32(255, 0, 0, 255);

        Invoke("ResetColors", 0.25f);

        if (health <= 0 && FindObjectOfType<GameHost>() != null)
            FindObjectOfType<GameHost>().mesaj += "Destroy " + playerNumber + "\n";
        else if(FindObjectOfType<GameHost>() != null)
            FindObjectOfType<GameHost>().mesaj += "Damage " + playerNumber + "\n";
    }

    public void Damage()
    {
        if (FindObjectOfType<GameHost>() == null)
        {
            CancelInvoke("ResetColors");

            foreach (SpriteRenderer limb in limbs)
                limb.color = new Color32(255, 0, 0, 255);

            Invoke("ResetColors", 0.25f);
        }
    }

    void ResetColors()
    {
        for(int i=0;i<=limbs.Count;i++)
        {
            limbs[i].color = colors[i];
        }
    }
}
