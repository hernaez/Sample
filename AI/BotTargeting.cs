using UnityEngine;
using System.Collections;

public class BotTargeting : MonoBehaviour {
	
	const int DISTANCE_MAX = 250;
	const int DISTANCE_MIN = 150;
	
	static float EMP_SPHERE_CAST_RADIUS = 30;
	static int LAYER = 1 << LayerMask.NameToLayer("Submarines");
		
	Transform thisTransform;
	
	public GameObject torpedoPrefab;
	
	FireActionsManager fireActionManager;
			
	public float time;
	float currentTime;
	
	bool hasProximityWeapon;
	bool hasDistanceWeapon;
	bool hasEMP;
	
	float forwardRadius = 20;
	float forwardDistance = 500;
	
	float backwardRadius = 20;
	float backwardDistance = 200;
	
	// Use this for initialization
	void Start () {
		currentTime = time;
		thisTransform = this.transform;
		
		hasProximityWeapon = false;
		hasDistanceWeapon = false;
		hasEMP = false;
		
		this.fireActionManager = GetComponent<FireActionsManager>();
		fireActionManager.PowerUpIsEnabled +=  PowerUpWasCatched;
		
		switch((int) fireActionManager.SecondaryWeaponType){
			case (int) WeaponType.INK_MINE: hasProximityWeapon = true; return;
			case (int) WeaponType.PROXIMITY_MINE: hasProximityWeapon = true; return;			
			case (int) WeaponType.GUIDED_TORPEDO: hasDistanceWeapon = true; return;
			case (int) WeaponType.STUN_TORPEDO: hasDistanceWeapon = true; return;
		}
	}
	
	void PowerUpWasCatched(EnableChangedEventArgs e){

		if(fireActionManager.PowerUpType == PowerUpType.EMP) hasEMP = true;
		else hasEMP = false;
	}
	
	// Update is called once per frame
	void Update () {	
		
		currentTime -= Time.deltaTime;
		if(currentTime <= 0){
			currentTime = time;
			
			bool weaponFired = FireEMP();
			if(!weaponFired) weaponFired = FireBackward();
			if(!weaponFired) weaponFired = FireForward();
		}
		
	}
	
	bool FireEMP(){
		if(!hasEMP) return false;
		
		Collider[] hits = Physics.OverlapSphere(thisTransform.position, EMP_SPHERE_CAST_RADIUS, LAYER);
	
		int ownPlayerNumer = thisTransform.GetComponent<SubmarineCore>().PlayerNumber;
		
		int enemys = 0;
		
		foreach(Collider hit in hits){
					
			SubmarineCore core = hit.GetComponent<SubmarineCore>();
			if(core != null && core.PlayerNumber != ownPlayerNumer) 
				++enemys;
		}
		
		if(enemys > 0){
			fireActionManager.PowerUp();
			return true;
		}
		
		return false;
	}
	
	bool FireBackward(){
		if(!hasProximityWeapon) return false;
		
		//Check Backward submarines
		RaycastHit[] hits = Physics.SphereCastAll(new Ray(thisTransform.position, -1 * thisTransform.forward), 
													backwardRadius, backwardDistance, LAYER);
	
		foreach(RaycastHit hit in hits){
					
			GameObject target = hit.collider.gameObject;
			FireSubmarine(target, false);
		}
			
		return hits.Length != 0;
	}
	
	bool FireForward(){
		
		//Check Forward submarines
		RaycastHit[] hits = Physics.SphereCastAll(new Ray(thisTransform.position, thisTransform.forward), 
													forwardRadius, forwardDistance, LAYER); 
	
		foreach(RaycastHit hit in hits){
					
			GameObject target = hit.collider.gameObject;
			FireSubmarine(target, true);
		}
		
		return hits.Length != 0;
	}
	
	void FireSubmarine(GameObject target, bool forward){
		
		//Check submarine team
		SubmarineCore botSC = gameObject.GetComponent<SubmarineCore>();
		SubmarineCore targetSC = target.GetComponent<SubmarineCore>();
		if(botSC.TeamNumber == targetSC.TeamNumber){
			return;
		}
		
		//Calculate target position in relation to impact position
		Vector3 initialTargetPos = target.transform.position;
		Vector3 targetVelocity = target.rigidbody.velocity;				
	
		Vector3 initialProjectilePos = this.transform.position;				
		Vector3 impactPos = PredictPosition(initialTargetPos, targetVelocity, initialProjectilePos, Torpedo.FAST_TORPEDO_SPEED);
		
		float vectorProduct = 0;
		if(forward) vectorProduct = Vector3.Dot((impactPos - initialTargetPos).normalized, this.transform.forward);
		else vectorProduct = Vector3.Dot(initialTargetPos.normalized, -1 * this.transform.forward);
	
		if(vectorProduct > 0.2) return;
		
		//Evaluate the use of primary or secondary weapon
		float distance = Vector3.Distance(target.transform.position, transform.position);
		
		bool fireProximityWeapon = distance <= DISTANCE_MIN && hasProximityWeapon && !forward;
		bool fireDistanceWeapon = distance >= DISTANCE_MAX && hasDistanceWeapon && forward;
		
		if (!fireProximityWeapon && !fireDistanceWeapon && forward) {
			GetComponent<FireActionsManager>().FirePrimaryWeapon(); 
			
		}else if(fireDistanceWeapon){
			GetComponent<FireActionsManager>().FireSecondaryWeapon();
			
		} else if(fireProximityWeapon){
			GetComponent<FireActionsManager>().FireSecondaryWeapon();
			
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
