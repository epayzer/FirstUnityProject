using UnityEngine;
using System.Collections;

[System.Serializable]
public class Done_Boundary 
{
	public float xMin, xMax, yMin, yMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
    public float cruisingVelocity;
    public float acceleration;
    public float deceleration;
    public float maxVelocity;

    public float yawAcceleration;
    public float maxYawVelocity;
    public float pitchAcceleration;
	public float tilt;
    public float maxTilt;
	public float radianPerSecondTurnSpeed;
	public Done_Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	
	private float nextFire;
    private float velocity = 0.0f;
    private float accelerationAmount = 0.0f;
    private Vector3 accelerationVector = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 velocityVector = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 previousVelocityVector = new Vector3(0.0f, 0.0f, 0.0f);
    private float directionalDragFactor = 0.8f;
    // TODO: correspond this to the actual initial direction model is facing!
    private Vector3 forwardVector = new Vector3(0.0f, 0.0f, 1.0f);
	private float yaw = 0.0f;
    private float yawVelocity = 0.0f;
	private float pitch = 0.0f;
    private float pitchVelocity = 0.0f;

    enum SpeedState
    {
        Cruising,
        Accelerating,
        Braking
    };
    private SpeedState speedState = SpeedState.Cruising;

    void Update ()
	{
		//if (Input.GetButton("Fire1") && Time.time > nextFire) 
		//{
		//	nextFire = Time.time + fireRate;
		//	Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		//	GetComponent<AudioSource>().Play ();
		//}
	}

	//void FixedUpdate ()
	//{
 //       HandleAcceleration(Time.deltaTime);

 //       yaw += Input.GetAxis ("Horizontal");
 //       yaw = PreventRotationOverflow(yaw, 360.0f);

 //       pitch -= Input.GetAxis ("Vertical");
	//	pitch = Mathf.Clamp (pitch, -45.0f, 45.0f);// TODO: what is the best clamp value here? 89 is too cose to 90 apparently?

 //       // NOTE: delta time is in seconds.
 //       //float forwardMovement = Time.deltaTime * forwardMovementRateInSeconds;
 //       //Vector3 movement = new Vector3 (0.0f, 0.0f, speed);
 //       //rigidbody.velocity = movement * speed;
 //       //rigidbody.velocity = movement;

 //       GetComponent<Rigidbody>().position = new Vector3
	//	(
	//		Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
	//		Mathf.Clamp (GetComponent<Rigidbody>().position.y, boundary.yMin, boundary.yMax), 
	//		Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
	//	);

	//	GetComponent<Rigidbody>().rotation = Quaternion.Euler (pitch, yaw, 0.0f);

	//	// TODO: delete below test code!
	//	Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);
 //       forwardVector = GetComponent<Rigidbody>().transform.forward;
	//	if (forwardVector.x != 0.0f ||
	//	    forwardVector.y != 0.0f) {
	//		//movement =  forwardVector * velocity;
	//	}
	//	else{
	//		//movement = new Vector3 (0.0f, 0.0f, velocity);
	//	}

 //       //GetComponent<Rigidbody>().velocity = movement;
 //   }

    void FixedUpdate()
    {
        HandleAcceleration(Time.deltaTime);

        HandleYawAcceleration(Time.deltaTime);

        yaw += Input.GetAxis("Horizontal") * yawVelocity;
        yaw = PreventRotationOverflow(yaw, 360.0f);

        pitch -= Input.GetAxis("Vertical");
        pitch = Mathf.Clamp(pitch, -45.0f, 45.0f);// TODO: what is the best clamp value here? 89 is too cose to 90 apparently?

        HandleTilt(Time.deltaTime);

        // NOTE: delta time is in seconds.
        //float forwardMovement = Time.deltaTime * forwardMovementRateInSeconds;
        //Vector3 movement = new Vector3 (0.0f, 0.0f, speed);
        //rigidbody.velocity = movement * speed;
        //rigidbody.velocity = movement;

        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(GetComponent<Rigidbody>().position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        );

        // TODO: the longer you hold down the turn left or right keys the faster you should turn! And the more you should tilt
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(pitch, yaw, -tilt);

        // TODO: delete below test code!
        Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);
        forwardVector = GetComponent<Rigidbody>().transform.forward;
        if (forwardVector.x != 0.0f ||
            forwardVector.y != 0.0f)
        {
            //movement =  forwardVector * velocity;
        }
        else
        {
            //movement = new Vector3 (0.0f, 0.0f, velocity);
        }

        //GetComponent<Rigidbody>().velocity = movement;

        Vector3 tempPreviousVelocityVector = previousVelocityVector;
        previousVelocityVector = velocityVector;
        velocityVector = forwardVector.normalized * velocity;
        velocityVector += tempPreviousVelocityVector * directionalDragFactor;
        GetComponent<Rigidbody>().velocity = velocityVector;
    }

    void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Terrain")
		{
			yaw = 45.0f;
		}
	}

    //void HandleAcceleration(float deltaTime)
    //{
    //    // TODO: support configurable controls!
    //    // Braking take precedence
    //    if (Input.GetKey(KeyCode.LeftAlt))
    //    {
    //        speedState = SpeedState.Braking;
    //        if (velocity > 0.0f)
    //        {
    //            velocity -= deceleration * deltaTime;
    //        }
    //    }
    //    else if (Input.GetKey(KeyCode.LeftControl))
    //    {
    //        speedState = SpeedState.Accelerating;
    //        if (velocity < maxVelocity)
    //        {
    //            velocity += acceleration * deltaTime;
    //        }
    //    }
    //    else
    //    {
    //        speedState = SpeedState.Cruising;
    //        if (velocity > cruisingVelocity)
    //        {
    //            velocity -= deceleration * deltaTime;
    //        }
    //        else if (velocity < cruisingVelocity)
    //        {
    //            velocity += acceleration * deltaTime;
    //        } // TODO: else if we are close to cruising then just snap to cruising speed!
    //    }
    //}

    void HandleAcceleration(float deltaTime)
    {
        // TODO: support configurable controls!
        // Braking take precedence
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            speedState = SpeedState.Braking;
            if (velocity > 0.0f)
            {
                velocity -= deceleration * deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speedState = SpeedState.Accelerating;
            if (velocity < maxVelocity)
            {
                velocity += acceleration * deltaTime;
            }
        }
        else
        {
            speedState = SpeedState.Cruising;
            if (velocity > cruisingVelocity)
            {
                velocity -= deceleration * deltaTime;
            }
            else if (velocity < cruisingVelocity)
            {
                velocity += acceleration * deltaTime;
            } // TODO: else if we are close to cruising then just snap to cruising speed!
        }

        // TODO: We should never have negative velocity
        if (velocity < 0.0f)
        {
            velocity = 0.0f;
        }
    }

    void HandleYawAcceleration(float deltaTime)
    {
        if (Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            yawVelocity += yawAcceleration * deltaTime;
            if (yawVelocity > maxYawVelocity)
            {
                yawVelocity = maxYawVelocity;
            }
        }
        else
        {
            yawVelocity = 1.0f;
        }
    }

    void HandleTilt(float deltaTime)
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            tilt += 1.0f;
            if (tilt > maxTilt)
            {
                tilt = maxTilt;
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            tilt -= 1.0f;
            if (tilt < -maxTilt)
            {
                tilt = -maxTilt;
            }
        }
        else
        {
            if (tilt > 0.0f)
            {
                tilt -= 1.0f;
            }
            else if (tilt < 0.0f)
            {
                tilt += 1.0f;
            }
        }
    }

    float PreventRotationOverflow(float rotation, float absMaxRotation)
    {
        if (rotation > absMaxRotation)
        {
            rotation = rotation - absMaxRotation;
        }
        else if (rotation < -absMaxRotation)
        {
            rotation = rotation + absMaxRotation;
        }

        return rotation;
    }
}
