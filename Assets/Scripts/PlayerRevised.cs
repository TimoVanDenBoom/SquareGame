using System.Collections;
using UnityEngine;

public class PlayerRevised : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveInput;
    public float jumpHeight; // A public float so we can change its value easily in the inspector
    private Animator anim;
    public float maxSpeed;

    private bool isGrounded;
    private float groundedRemember;
    private float groundedRememberTime;
    public Transform groundCheck;
    private float jumpPressedRemember;
    private float jumpPressedRememberTime;
    private float horizontalSpringRememberTime;
    private float horizontalSpringRemember;
    private bool horizontalSpringHit = false;
    private float gravityScale;
    public float fallMultiplier = 2.5f;
    

    private int rotateState = 0;
    public float checkRadius;
    public LayerMask whatIsGround;

    private bool isRotating;
    public float rotateSpeed;

    private bool hasBlue;
    private bool hasRed;
    private bool hasGreen;
    private bool hasYellow;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    public float wallHopForce;
    public float wallJumpForce;
    public float wallSlideSpeed;
    private bool isWallSlidingLeft;
    private bool isWallSlidingRight;

    private float dampWhenStoppingInitial;
    private float dampBasicInitial;
    private float dampWhenTurningInitial;

    public int lives;
    private int energy = 0;
    private int maxLives;

    public GameObject shot;
    public float shotTime;
    private float shotTimeOriginal;
    public Transform shotPoint;

    [SerializeField]
    float fHorizontalAcceleration = 1;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hasBlue = false;
        hasYellow = false;
        hasRed = false;
        hasGreen = false;
        isRotating = false;
        jumpPressedRemember = 0;
        jumpPressedRememberTime = 0.1f;
        groundedRemember = 0;
        groundedRememberTime = 0.1f;
        horizontalSpringRemember = 0;
        horizontalSpringRememberTime = 0.55f;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        gravityScale = rb.gravityScale;
        dampBasicInitial = fHorizontalDampingBasic;
        dampWhenStoppingInitial = fHorizontalDampingWhenStopping;
        dampWhenTurningInitial = fHorizontalDampingWhenTurning;
        maxLives = lives;
        shotTimeOriginal = shotTime;
    }

    // Update is called once per frame
    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        jumpPressedRemember -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressedRemember = jumpPressedRememberTime;
        }

        if (jumpPressedRemember > 0 && isGrounded) 
        {
            jumpPressedRemember = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            //rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }

        if (!isGrounded && Input.GetKeyDown(KeyCode.Space) && isWallSlidingLeft)
        {
            isWallSlidingLeft = false;
            rb.AddForce(new Vector2(wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce));
        }

        if (!isGrounded && Input.GetKeyDown(KeyCode.Space) && isWallSlidingRight)
        {
            isWallSlidingRight = false;
            rb.AddForce(new Vector2(-wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce));
        }

        horizontalSpringRemember -= Time.deltaTime;
        if (horizontalSpringRemember < 0 && horizontalSpringHit == true)
        {
            horizontalSpringHit = false;
            horizontalSpringRemember = 0;
            fHorizontalDampingBasic = dampBasicInitial;
            fHorizontalDampingWhenStopping = dampWhenStoppingInitial;
        }

        if(energy > 0 && Time.time > shotTime && Input.GetMouseButton(0))
        {
            energy--;
            shotTime = Time.time + shotTimeOriginal;
            if(!(rotateState == 3 && isGrounded))
            {
                Instantiate(shot, shotPoint.position, shotPoint.rotation);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            if (rotateState == 0)
            {
                rotateState = 3;
            }
            else
            {
                rotateState--;
            }
            StartCoroutine(RotateMe(Vector3.forward * 90, rotateSpeed));
        }

        if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            if (rotateState == 3)
            {
                rotateState = 0;
            }
            else
            {
                rotateState++;
            }
            StartCoroutine(RotateMe(Vector3.forward * -90, rotateSpeed));
        }


        float fHorizontalVelocity = rb.velocity.x;

        if (!isGrounded && !horizontalSpringHit)
        {
            fHorizontalVelocity += Input.GetAxisRaw("Horizontal") * 0.085f;
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            {
                 fHorizontalVelocity = maxSpeed * Mathf.Sign(rb.velocity.x);
            }

        }
        else if (!horizontalSpringHit)
        {
            
            fHorizontalVelocity += Input.GetAxisRaw("Horizontal") * fHorizontalAcceleration;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f )
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
        }
        else
        {
            fHorizontalVelocity += Input.GetAxisRaw("Horizontal") * 0.02f;
            //if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            //    fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 1.5f);
            //else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
            //    fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 2f);
            //else
            //    fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 1.55f);
        }
        

        rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);

        

        if (Mathf.Abs(fHorizontalVelocity) > 0.01f || !isGrounded)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

    }

    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        isRotating = true;
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        transform.rotation = toAngle;
        isRotating = false;
    }

    private void FixedUpdate()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, whatIsGround);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position, Vector2.left, 0.5f, whatIsGround);
        RaycastHit2D bottomSide = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, whatIsGround);
        RaycastHit2D topSide = Physics2D.Raycast(transform.position, Vector2.up, 0.5f, whatIsGround);
        if (rightSide.collider != null)
        {
            if (rightSide.collider.gameObject.CompareTag("BlueTile") && hasBlue && rotateState == 0)
            {
                rb.velocity = rightSide.normal * rightSide.collider.gameObject.GetComponent<BlueBlockScript>().launchingForce;
                horizontalSpringRemember = horizontalSpringRememberTime;
                fHorizontalDampingWhenStopping = 0;
                fHorizontalDampingBasic = 0;
                horizontalSpringHit = true;
            }
            if(rightSide.collider.gameObject.CompareTag("GreenTile") && hasGreen && rotateState == 1)
            {
                
                if (rb.velocity.y < -wallSlideSpeed && Input.GetAxisRaw("Horizontal") > 0)
                {
                    isWallSlidingRight = true;
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                }
                else if (rb.velocity.y > wallSlideSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideSpeed * 3.5f);
                }
            }
            else
            {
                isWallSlidingRight = false;
            }
        }
        else
        {
            isWallSlidingRight = false;
        }
        if (bottomSide.collider != null)
        {
            if (bottomSide.collider.gameObject.CompareTag("BlueTile") && hasBlue && rotateState == 1)
            {
                rb.velocity = new Vector2((1-bottomSide.collider.gameObject.GetComponent<BlueBlockScript>().xDampener) * rb.velocity.x, 0)
                    + bottomSide.normal * bottomSide.collider.gameObject.GetComponent<BlueBlockScript>().launchingForce;
            }
            if (bottomSide.collider.gameObject.CompareTag("GreenTile") && hasGreen && rotateState == 2)
            {
                horizontalSpringHit = false;
                horizontalSpringRemember = 0;
                fHorizontalDampingBasic = bottomSide.collider.gameObject.GetComponent<GreenBlockScript>().drag;
                fHorizontalDampingWhenStopping = bottomSide.collider.gameObject.GetComponent<GreenBlockScript>().drag;
                fHorizontalDampingWhenTurning = bottomSide.collider.gameObject.GetComponent<GreenBlockScript>().drag;
            }
            else
            {
                fHorizontalDampingBasic = dampBasicInitial;
                fHorizontalDampingWhenStopping = dampWhenStoppingInitial;
                fHorizontalDampingWhenTurning = dampWhenTurningInitial; 
            }
        }
        else
        {
            fHorizontalDampingBasic = dampBasicInitial;
            fHorizontalDampingWhenStopping = dampWhenStoppingInitial;
            fHorizontalDampingWhenTurning = dampWhenTurningInitial;
        }
        if (leftSide.collider != null)
        {
            if (leftSide.collider.gameObject.CompareTag("BlueTile") && hasBlue && rotateState == 2)
            {
                rb.velocity = leftSide.normal * leftSide.collider.gameObject.GetComponent<BlueBlockScript>().launchingForce;
                horizontalSpringRemember = horizontalSpringRememberTime;
                fHorizontalDampingWhenStopping = 0;
                fHorizontalDampingBasic = 0;
                horizontalSpringHit = true;
            }
            if (leftSide.collider.gameObject.CompareTag("GreenTile") && hasGreen && rotateState == 3)
            {
                
                if (rb.velocity.y < -wallSlideSpeed && Input.GetAxisRaw("Horizontal") < 0)
                {
                    isWallSlidingLeft = true;
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                }
                else if (rb.velocity.y > wallSlideSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideSpeed * 3.5f);
                }
            }
            else
            {
                isWallSlidingLeft = false;
            }
        }
        else
        {
            isWallSlidingLeft = false;
        }
        if (topSide.collider != null)
        {
            if (topSide.collider.gameObject.CompareTag("BlueTile") && hasBlue && rotateState == 3)
            {
                rb.velocity = topSide.normal * topSide.collider.gameObject.GetComponent<BlueBlockScript>().launchingForce;
            }
        }
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BlueUpgrade"))
        {
            Destroy(other.gameObject);
            anim.SetInteger("upgradesAmount", 1);
            hasBlue = true;
        }
        else if (other.gameObject.CompareTag("GreenUpgrade"))
        {
            Destroy(other.gameObject);
            anim.SetInteger("upgradesAmount", 2);
            hasGreen = true;
        }
        else if (other.gameObject.CompareTag("RedUpgrade"))
        {
            Destroy(other.gameObject);
            anim.SetInteger("upgradesAmount", 3);
            hasRed = true;
        }
        else if (other.gameObject.CompareTag("YellowUpgrade"))
        {
            Destroy(other.gameObject);
            anim.SetInteger("upgradesAmount", 4);
            hasYellow = true;
        }
    }

    public int getRotateState()
    {
        return rotateState;
    }
    public bool getHasRed()
    {
        return hasRed;
    }

    public void chargeEnergy()
    {
        if(energy < 3)
        {
            energy++;
        }
    }
}
