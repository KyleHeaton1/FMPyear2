using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAI : MonoBehaviour
{
    [Header("Target Options")]
	[SerializeField] private Transform _target;

    [Header("AI Movement Settings")]
	[SerializeField] private float _targetVelocity;
    [SerializeField] private float _rotationSpeed= 2;
    [SerializeField] private AnimationCurve _DistanceVersusSpeedGraph;
    
    [Header("Angle Infomation")]
	[SerializeField] private int _numberOfRays = 17;
	[SerializeField] private float _angle = 90;
	[SerializeField] private float _rayRange = 2;

 
    private float _initialDistanceToTarget;
    private float _distanceToTarget;
 
    public void Start()
    {
        _initialDistanceToTarget = (_target.position - transform.position).magnitude;
        _targetVelocity = 0;
    }
 

    void Update()
    {
		Quaternion rotTarget = Quaternion.LookRotation(_target.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, _rotationSpeed * Time.deltaTime);

        // Calculate our distance from target
        Vector3 deltaMovement = _target.position - transform.position;
        _distanceToTarget = deltaMovement.magnitude;
 
        // Update our speed based on our distance from the target
        _targetVelocity = _DistanceVersusSpeedGraph.Evaluate((_initialDistanceToTarget - _distanceToTarget) / _initialDistanceToTarget);
 
        // If we need to move father than we can in t$$anonymous$$s update, then limit how much we move
        if (_distanceToTarget > _targetVelocity)
        {
            deltaMovement = deltaMovement.normalized * _targetVelocity;
        }


		Vector3 deltaPosition = Vector3.zero;
		for (int i = 0; i < _numberOfRays; ++i)
		{
			//angle of the area of the rays
			Quaternion rotation = this.transform.rotation;
			Quaternion rotationMod = Quaternion.AngleAxis((i / ((float)_numberOfRays - 1)) * _angle * 2 - _angle, Vector3.up);
			Vector3 direction = rotation * rotationMod * Vector3.forward;
			Debug.DrawRay(this.transform.position, direction);

			//actual raycasts from the angle area
			Ray ray = new Ray(this.transform.position, direction);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, _rayRange))
			{
				//moves the player if ray collides with object
				deltaPosition -= (2.0f / _numberOfRays) * _targetVelocity * direction;
			}
			else
			{
				//moves the player if ray collides with object
				deltaPosition += (2.0f / _numberOfRays) * _targetVelocity * direction;
			}
			//constant moves player
			this.transform.position += deltaPosition * Time.deltaTime;
		}

 


        /*
        float distance = Vector3.Distance(this.transform.position, _target.transform.position);
        if(distance < 2)
        {
            this.transform.position = Vector3.zero;
        }else
        {
            _targetVelocity = 1.5f;
        }
        */

	}
}
