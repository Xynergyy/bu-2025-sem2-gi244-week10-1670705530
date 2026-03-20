using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float gravityModifier;

    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;

    public AudioClip jumpSfx;
    public AudioClip crashSfx;

    public bool isDashing = false;
    public bool gameOver = false;

    public int health = 3;

    private Rigidbody rb;
    private InputAction jumpAction;
    private InputAction dashAction;

    private bool isOnGround = true;

    private Animator playerAnim;
    private AudioSource playerAudio;

    private int jumpCount = 0;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics.gravity *= gravityModifier;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) return;

        HandleJump();
        HandleDash();
    }
    void HandleJump()
    {
        if (jumpAction.triggered && jumpCount < 2 && !gameOver)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);

            jumpCount++;

            isOnGround = false;
            playerAnim.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSfx);
        }
    }

    void HandleDash()
    {
        if (dashAction.IsPressed() && !gameOver)
        {
            isDashing = true;
            playerAnim.SetFloat("Speed_f", 2.0f);
        }
        else
        {
            isDashing = false;
            playerAnim.SetFloat("Speed_f", 1.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            jumpCount = 0;
            if (!gameOver) dirtParticle.Play();

        }
        else if (collision.gameObject.CompareTag("Obstacle") && !gameOver)
        {
            explosionParticle.Clear();
            explosionParticle.Play();
            playerAudio.PlayOneShot(crashSfx);
            health--;

            Destroy(collision.gameObject);

            if (health <= 0)
            {
                gameOver = true;
                playerAnim.SetBool("Death_b", true);
                playerAnim.SetInteger("DeathType_int", 1);
                dirtParticle.Stop();
                explosionParticle.Play();
            }
        }
    }

}