using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
	public Transform target;
	public Vector3 offset = new Vector3(0, 12, -8);
	public float followLerp = 6f;

	void LateUpdate()
	{
		if (target == null) return;
		var desired = target.position + offset;
		transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followLerp);
		transform.rotation = Quaternion.Euler(65, 0, 0);
	}
}


