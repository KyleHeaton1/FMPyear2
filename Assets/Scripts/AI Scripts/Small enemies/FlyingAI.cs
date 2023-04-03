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
	[SerializeField] private int _numberOfRays;
	[SerializeField] private float _angle;
	[SerializeField] private float _rayRange;

 
    [HideInInspector] [SerializeField] private float _initialDistanceToTarget;
    [HideInInspector] [SerializeField] private float _distanceToTarget;
 
    public void Start()
    {
        _initialDistanceToTarget = (_target.position - transform.position).magnitude;
        _targetVelocity = 0;
    }
 

    void Update()
    {
        //have a delay in rotation based on the target speed
		Quaternion rotTarget = Quaternion.LookRotation(_target.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, _rotationSpeed * Time.deltaTime);

        //distance from target
        Vector3 deltaMovement = _target.position - transform.position;
        _distanceToTarget = deltaMovement.magnitude;
 
        //speed based on the target distance
        _targetVelocity = _DistanceVersusSpeedGraph.Evaluate((_initialDistanceToTarget - _distanceToTarget) / _initialDistanceToTarget);
 
        //normalize the velocity if the enemy gets close to the player
        if (_distanceToTarget > _targetVelocity) deltaMovement = deltaMovement.normalized * _targetVelocity;

		Vector3 deltaPosition = Vector3.zero;
		for (int i = 0; i < _numberOfRays; ++i)
		{
			//angle of the area of the rays
			Quaternion rotation = this.transform.rotation;
			Quaternion rotationMod = Quaternion.AngleAxis((i / ((float)_numberOfRays - 1)) * _angle * 2 - _angle, Vector3.up);
			Vector3 direction = rotation * rotationMod * Vector3.forward;

			//actual raycasts from the angle area
			Ray ray = new Ray(this.transform.position, direction);
			RaycastHit hitInfo;
            //moves the player if ray collides with object
			if (Physics.Raycast(ray, out hitInfo, _rayRange)) deltaPosition -= (2.0f / _numberOfRays) * _targetVelocity * direction;
            //moves the player if ray collides with object
			else deltaPosition += (2.0f / _numberOfRays) * _targetVelocity * direction;
			//constant moves player
			this.transform.position += deltaPosition * Time.deltaTime;
		}
	}
}
