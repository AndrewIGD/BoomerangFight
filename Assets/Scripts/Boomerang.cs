using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public Vector2 onlineNewPos;
    public Vector2 onlineOldPos;

    //Variables
    [SerializeField] float damage;
    public GameObject parentHand;
    public Player parent;
    //Local Variables
    float initialRot;
    Vector2 pivot;
    Vector2 targetPos;
    bool canRotate = false;
    bool reachedTarget = false;
    float speed;
    public float speedMultiplier = 1;
    Vector3 diff;

    //Components
    BoxCollider2D coll;
    Rigidbody2D rb;

    public float SpeedMultiplier
    { 
        get
        {
            return speedMultiplier;
        }
        set
        {
            speedMultiplier = value;
        }
    }



    // Start is called before the first frame update

    void Awake()
    {
        coll = GetComponentInChildren<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        hitPlayers = new List<Player>();
    }

    public float timeAlive = 0;

    public Vector3 movementDirection;

    public float interpolationTime = -1;

    public Vector3 interpolationCurrentPos;

    public Vector3 interpolationTarget;

    private void OnEnable()
    {
        timeAlive = 0;

        foreach (Player player in FindObjectsOfType<Player>())
        {
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(player.thrownBoomerang.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
        }
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        onlineOldPos = onlineNewPos;
        onlineNewPos = transform.position;

        if (FindObjectOfType<GameHost>() == null)
        {

            if (interpolationTime != -1)
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
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(new Vector3(0, 0, 720 * Time.deltaTime));

        if (canRotate && FindObjectOfType<GameHost>() != null)
        {
            if(reachedTarget)
            {
                float initRot = transform.eulerAngles.z;

                Vector2 diff = transform.position - parentHand.transform.position;
                diff.Normalize();

                float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, rot-90);

                rb.velocity = -transform.up * speedMultiplier;

                transform.eulerAngles = new Vector3(0, 0, initRot);

                if (Vector2.Distance(parentHand.transform.position, transform.position) < 0.25f && parent.HasBoomerang() == false)
                {
                    canRotate = false;
                    parent.BoomerangReturned();
                    gameObject.SetActive(false);
                    canRotate = false;

                    hitPlayers.Clear();
                }
            }
            else
            {
                float initRot = transform.eulerAngles.z;

                Vector2 diff = (Vector2)transform.position - targetPos;
                diff.Normalize();

                float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, rot - 90);

                rb.velocity = -transform.up * speedMultiplier;

                transform.eulerAngles = new Vector3(0, 0, initRot);

            }
        }
    }

    List<Player> hitPlayers;

    public void ProcessCollision(Collider2D collision)
    {
        if (FindObjectOfType<GameHost>() != null)
        {
            if (collision.gameObject == parent.gameObject && reachedTarget)
            {
                canRotate = false;
                parent.BoomerangReturned();
                gameObject.SetActive(false);
                canRotate = false;

                hitPlayers.Clear();
            }
            else if (collision.gameObject.GetComponent<Player>() != null)
            {
                if (collision.gameObject.GetComponent<Player>().team != parent.team && hitPlayers.Contains(collision.gameObject.GetComponent<Player>()) == false)
                {
                    hitPlayers.Add(collision.gameObject.GetComponent<Player>());
                    collision.gameObject.GetComponent<Player>().DecreaseHp(damage);
                }
            }
            else if(collision.gameObject.GetComponent<PlayerTarget>() != null)
            {
                if(reachedTarget == false && collision.gameObject.GetComponent<PlayerTarget>().playerNumber == parent.playerNumber)
                {
                    reachedTarget = true;

                    coll.enabled = false;
                    coll.enabled = true;
                }
            }
            else if(collision.gameObject.GetComponent<Boomerang>() == false && collision.isTrigger == false)
            {
                reachedTarget = true;

                coll.enabled = false;
                coll.enabled = true;
            }
        }
    }

    //Methods

    /// <summary>
    /// Makes boomerang go to target position and return
    /// </summary>
    /// <param name="pos"></param>
    public void GoAt(Vector2 pos)
    {
        targetPos = pos;

        canRotate = true;
        reachedTarget = false;

        float initRot = transform.eulerAngles.z;

        Vector2 diff = (Vector2)transform.position - pos;
        diff.Normalize();

        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, rot-90);

        rb.velocity = -transform.up * speedMultiplier;

        transform.eulerAngles = new Vector3(0,0,initRot);
    }
}
