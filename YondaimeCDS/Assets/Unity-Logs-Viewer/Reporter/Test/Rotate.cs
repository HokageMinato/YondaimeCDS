using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
	private Vector3 angle;

	private void Start()
	{
		angle = transform.eulerAngles;
	}

	private void Update()
	{
		angle.y += Time.deltaTime * 100;
		transform.eulerAngles = angle;
	}

}
