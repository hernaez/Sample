using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Steering : MonoBehaviour {
	
	public float obstacleAvoidanceWeight = 0.8f;
	public float mineAvoidanceWeight = 0.95f;
	public float pursuitWeight = 0.3f;
	public float obstacleAvoidanceDistance = 250.0f;
	public float mineAvoidanceDistance = 250.0f;
	
	bool pursuit;
	bool seek;
	bool wander;
	bool avoidWalls;
	bool avoidMines;
	bool evade;
	bool goTo;
	bool flee;
	bool seekFuture;
	
	List<Transform> fleeTargets;
	Navigator pursuitTarget;
	Navigator pursuitFutureTarget;
	Transform seekTransform;
	Vector3 seekPosition;
	
	//cached
	Transform thisTransform;
	Rigidbody thisRigidbody;	
	Navigator thisNavigator;
						
	//debug
	Vector3 steeringDir;
	
	// Use this for initialization
	void Start () {
		thisTransform = this.transform;
		thisRigidbody = this.rigidbody;		
		thisNavigator = this.GetComponent<Navigator>();
	}
	
	// Update is called once per frame
	void Update () {
			
		steeringDir = Vector3.zero;		
		//float steeringWeight = 0;
		
		if (pursuit && pursuitTarget != null) {			
			steeringDir += Pursuit(pursuitTarget) * pursuitWeight;
			//steeringWeight += pursuitWeight;
		}
		
		if (seek && seekTransform != null) 
			steeringDir += Seek(seekTransform.position);
		
		if (goTo) 
			steeringDir += Seek(seekPosition);
		
		if (seekFuture) 
			steeringDir += PursuitFuture(pursuitFutureTarget);
		
		if (avoidWalls) {
			steeringDir += AvoidWalls() * obstacleAvoidanceWeight;
		}
		
		if (avoidMines) {
			steeringDir += AvoidMines() * mineAvoidanceWeight;
		}
		
		if(flee){
			steeringDir += Flee();
		}
						
		steeringDir.Normalize();
		
		float rot = 0;
		if (steeringDir != Vector3.zero) {
			float angle = Vector3.Angle(steeringDir, thisTransform.forward);
			rot = angle; /// thisNavigator.maxTurnAngle;
				
			if (Vector3.Cross(thisTransform.forward, steeringDir).y < 0) {
				//go left
				rot = -rot;
			}
		}
		
		thisNavigator.RotateNavigator(Mathf.Clamp(rot, -1, 1));
		
		//float rot = Mathf.Asin(-thisTransform.InverseTransformDirection(steeringDir).x) / (thisNavigator.maxTurnAngle * Mathf.Deg2Rad);
		//Debug.Log(rot);
			
	}
	
	public void ActivateWander(){
		wander = true;
		DeactivatePursuit();
	}
	
	public void DeactivateWander(){
		wander = false;
	}
	
	public void ActivatePursuit(Navigator target) {
		pursuitTarget = target;
		pursuit = true;
	}
	
	public void DeactivatePursuit() {
		pursuitTarget = null;
		pursuit = false;
	}
	
	public void ActivatePursuitFuture(Navigator target) {
		pursuitFutureTarget = target;
		seekFuture = true;
	}
	
	public void DeactivatePursuitFuture() {
		pursuitFutureTarget = null;
		seekFuture = false;
	}
	
	public void ActivateAvoidWalls() {
		avoidWalls = true;
	}
	
	public void DeactivateAvoidWalls() {
		avoidWalls = false;
	}
	
	public void ActivateFlee() {
		flee = true;
		if(fleeTargets == null) fleeTargets = new List<Transform>();
	}
	
	public void DeactivateFlee() {
		flee = false;
	}
	
	public void ActivateAvoidMines() {
		avoidMines = true;
	}
	
	public void DeactivateAvoidMines() {
		avoidMines = false;
	}
	
	public void ActivateGoTo(Vector3 goToPosition) {
		this.seekPosition = goToPosition;
		this.goTo = true;
	}
	
	public void DeactivateGoTo() {
		this.seekPosition = Vector3.zero;
		this.goTo = false;
	}
	
	Vector3 Pursuit (Navigator target) {
		
		Transform targetTransform = target.transform;
		
		Vector3 toEvader = targetTransform.position - thisTransform.position;		
		
		float relativeHeading = Vector3.Dot(thisTransform.forward, targetTransform.forward);
		
		if ((Vector3.Dot(toEvader.normalized, thisTransform.forward) > 0) &&
		    (relativeHeading < -0.95f)) {
			return Seek(targetTransform.position);	
		}
		
		float lookAheadTime = toEvader.magnitude / (thisNavigator.maxSpeed + target.GetComponent<Navigator>().maxSpeed);
		
		return Seek(targetTransform.position + target.rigidbody.velocity * lookAheadTime);
	}
	
	Vector3 PursuitFuture (Navigator target) {
		
		Vector3 predictedPosition = PredictPosition(target.transform.position, target.rigidbody.velocity, 
		                                            thisTransform.position, thisRigidbody.velocity.magnitude);
		
		return Seek(predictedPosition);
	}
	
	Vector3 AvoidWalls () {
		RaycastHit h;
		bool approachingWall = Physics.Raycast(thisTransform.position, 
		                                       thisTransform.forward, 
		                                       out h, 
		                                       obstacleAvoidanceDistance, 		                                       
		                                       1 << LayerMask.NameToLayer("Colliders"));
		if (approachingWall) {
			return h.normal * h.distance;
		}
		
		return Vector3.zero;
	}
	
	Vector3 AvoidMines () {
		RaycastHit h;
		bool approachingMine = Physics.SphereCast(thisTransform.position, 10,
		                                       thisTransform.forward, 
		                                       out h, 
		                                       mineAvoidanceDistance, 		                                       
		                                       1 << LayerMask.NameToLayer("Hazards"));
		if (approachingMine) {
			return h.normal * h.distance;
		}
		
		return Vector3.zero;
	}
	
	Vector3 Flee(){
		
		Vector3 fleeVector = Vector3.zero;
		
		foreach(Transform t in fleeTargets){
			fleeVector -= t.forward;
		}
		
		return fleeVector;
	}
	
	public void ActivateSeek(Transform transform) {
		seekTransform = transform;
		seek = true;
	}
	
	public void DeactivateSeek() {
		seekTransform = null;
		seek = false;
	}
			
	Vector3 Seek (Vector3 targetPos) {
		return targetPos - thisTransform.position;		
	}
	
	void OnDrawGizmos() {	
		if (steeringDir != Vector3.zero) {
			Gizmos.color = Color.red;
			Gizmos.DrawRay(this.transform.position, steeringDir*250);
		}
	}
	
	public List<Transform> FleeTargets{
		
		set{
			fleeTargets = value;
		}
		
	}
	
	Vector3 PredictPosition(Vector3 initialTargetPos, Vector3 targetVelocity, Vector3 initialProjectilePos, float projectileSpeed){
		
		float targetSpeed = Vector3.Magnitude(targetVelocity);
		float d = Vector3.Magnitude(initialProjectilePos - initialTargetPos);
		
		float cosAlpha = Vector3.Dot(targetVelocity.normalized, (initialProjectilePos - initialTargetPos).normalized);
		
		float a = projectileSpeed * projectileSpeed - targetSpeed * targetSpeed;
		float b = 2 * targetSpeed * d * cosAlpha;
		float c = - (d * d);
		
		float k = b * b - 4 * a * c;
		
		if (k < 0)
			return Vector3.zero;
		
		float t = (-b + Mathf.Sqrt(k)) / (2 * a);
		
		return initialTargetPos + targetVelocity * t;
	}
}
