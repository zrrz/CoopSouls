using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour {

    // movement config
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
	public float ledgeClimbSpeed = 0.5f;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    private CharacterController2D _controller;
//    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;

	Transform character;

	bool movementLocked = false;

	DamageHandler damageHandler;

    void Awake()
    {
//        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;

		character = transform.FindChild("Graphics");

		_controller.warpToGrounded();
    }

	void Start() {
		damageHandler = GetComponent<DamageHandler>();
		damageHandler.OnDeath.AddListener(OnDeath);
	}


    #region Event Listeners
//	static int counter = 0;
    void onControllerCollider( RaycastHit2D hit )
    {
        // bail out on plain old ground hits cause they arent very interesting
        if( hit.normal.y == 1f )
            return;

		//Ledge climbing
		if((_controller.collisionState.left || _controller.collisionState.right) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
			Vector2 size = GetComponent<BoxCollider2D>().size;
//			if(Mathf.Abs(hit.point.y - transform.position.y + (size.y/2f)) > .2f) 
//			if(hit.point.y < transform.position.y + (size.y*.4f))
//				return; //I don't care about anything but the top cast
			int direction = _controller.collisionState.left ? -1 : 1; //Is ledge left or right?
			Debug.DrawRay((Vector3)hit.point + Vector3.back, Vector2.right * direction, Color.blue, 1f);

			//TODO I could try to find the perfect height to start the raycast from
//			float yDiff = hit.point.y - transform.position.y;
			Vector2 offset = size;
			offset.x *= direction;
			RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + offset, Vector2.down, size.y*1.5f, LayerMask.GetMask(new string[1] {"Ground"}));
			Debug.DrawRay((Vector2)transform.position + offset, Vector2.down * size.y*1.5f, Color.magenta, 0.2f);
			if(hit2.collider != null) {
				float floorY = hit2.point.y;
				Vector2 pos = new Vector2(transform.position.x + (size.x * direction), floorY + size.y/2f);

				if(Physics2D.BoxCast(pos, size, 0f, Vector2.down, 0f, LayerMask.GetMask(new string[1] {"Ground"}))) {
					DebugHelper.DrawRect(pos, size, Color.red, 1f);
					StartCoroutine(ClimbLedge(pos));
				} else {
					DebugHelper.DrawRect(pos, size, Color.cyan, 1f);
				}
			}
		}

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
//        Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent( Collider2D col )
    {
        Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
    }


    void onTriggerExitEvent( Collider2D col )
    {
        Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
    }

    #endregion


    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void Update()
	{
        if( _controller.isGrounded )
            _velocity.y = 0;

		if(movementLocked)
			return;

		if(Input.GetKey(KeyCode.F)) {
			StartCoroutine(UseSpell(Input.mousePosition));
		}

		float horizontal = Input.GetAxis("Horizontal");

		if( horizontal > .9f )
//		if(Input.GetButton("Right"))
        {
            normalizedHorizontalSpeed = 1;
			if( character.localScale.x < 0f )
				character.localScale = new Vector3( -character.localScale.x, character.localScale.y, character.localScale.z );

//            if( _controller.isGrounded )
//                _animator.Play( Animator.StringToHash( "Run" ) );
        }
		else if( horizontal < -.9f )
//		else if(Input.GetButton("Left"))
        {
            normalizedHorizontalSpeed = -1;
			if( character.localScale.x > 0f )
				character.localScale = new Vector3( -character.localScale.x, character.localScale.y, character.localScale.z );

//            if( _controller.isGrounded )
//                _animator.Play( Animator.StringToHash( "Run" ) );
        }
        else
        {
            normalizedHorizontalSpeed = 0;

//            if( _controller.isGrounded )
//                _animator.Play( Animator.StringToHash( "Idle" ) );
        }


        // we can only jump whilst grounded
		if( _controller.isGrounded && (Input.GetButtonDown("Jump") || Input.GetAxis("Vertical") > 0f))
        {
            _velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
//            _animator.Play( Animator.StringToHash( "Jump" ) );
        }


        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets uf jump down through one way platforms
		if( _controller.isGrounded && Input.GetAxis("Vertical") < 0f )
        {
            _velocity.y *= 3f;
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }

        _controller.move( _velocity * Time.deltaTime );

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }

	IEnumerator ClimbLedge(Vector2 endPos) {
		movementLocked = true;

		Vector2 startPos = transform.position;

		for(float t = 0f; t < 1f; t += Time.deltaTime/ledgeClimbSpeed) {
			transform.position = Vector3.Lerp(startPos, endPos, t);
			yield return null;
		}

		_velocity = Vector3.zero;
		movementLocked = false;
	}

	public GameObject spellPrefab;

	IEnumerator UseSpell(Vector2 position) {
		movementLocked = true;
		yield return new WaitForSeconds(0.2f);

		Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(position) - (Vector2)transform.position).normalized;

//		Debug.DrawRay(transform.position, direction, Color.red, 1f);

		GameObject spell = (GameObject)Instantiate(spellPrefab, (Vector2)transform.position + (direction*0.2f), Quaternion.identity);
		spell.GetComponent<Rigidbody2D>().velocity = direction * 18f;
		_velocity.x = 0f;
		movementLocked = false;
	}

	void OnDeath() {
		Destroy(gameObject);

		//Trigger game restart
	}
}
