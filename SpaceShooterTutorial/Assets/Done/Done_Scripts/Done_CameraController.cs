using UnityEngine;
using System.Collections;

public class Done_CameraController : MonoBehaviour {
	// TODO: expose camera displayment as config props.
	private Vector3 cameraDisplayment = new Vector3 (0.0f, 1.0f, -4.0f);
	private Done_PlayerController playerController;

	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		if (playerObject != null)
		{
			playerController = playerObject.GetComponent <Done_PlayerController>();
		}
		if (playerController == null)
		{
			Debug.Log ("Cannot find 'Done_Player' script");
		}
	}

	// Update is called once per frame
	void Update () {
		// TODO: hook up to eventing instead of polling?
		transform.position = playerController.GetComponent<Rigidbody>().position + 
			(playerController.GetComponent<Rigidbody>().transform.right * cameraDisplayment.x) + 
		    (playerController.GetComponent<Rigidbody>().transform.up * cameraDisplayment.y) +
		    (playerController.GetComponent<Rigidbody>().transform.forward * cameraDisplayment.z);

		transform.LookAt(playerController.GetComponent<Rigidbody>().position);
	}
}
