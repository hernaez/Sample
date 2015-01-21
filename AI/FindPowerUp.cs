using UnityEngine;
using System.Collections;

public class FindPowerUp : Goal {
	
	GameObject target;

	float acceptedTime = 3;
	float currentTime;

	public FindPowerUp(PowerUpType type, Agent owner) {
		this.owner = owner;
		this.target = EvaluateTargets(type, owner);
		
		this.currentTime = 0;
		//Debug.Log("FindPowerUp Goal " + type + " - Player: " + owner.GetSubmarine().PlayerNumber);
	}
	
	public override void Activate() {
		base.Activate();
		
		if(target != null) owner.GetComponent<Steering>().ActivateGoTo(target.transform.position);
		else Status = GoalStatus.FAILED;
	}
	
	public override GoalStatus Process() {
	
		ActivateIfInactive();
		
		currentTime += Time.deltaTime;
		
		if(target == null || currentTime >= acceptedTime ){
			
			Terminate();
			Status = GoalStatus.COMPLETED;
		}
	
		return Status;
	}
	
	public override void Terminate() {
		owner.GetComponent<Steering>().DeactivateGoTo();
	}
	
	public static GameObject EvaluateTargets(PowerUpType type, Agent owner){
		
		GameObject possibleTarget = null;
		
		GameObject[] allPowerUps = GameObject.FindGameObjectsWithTag("PowerUp");	
		
		float minDistance = float.MaxValue;
		
		foreach (GameObject powerUpGO in allPowerUps) {
			
			if(powerUpGO.GetComponent<PowerUp>().Type == type){
				
				float distance = Vector3.Distance(powerUpGO.transform.position, owner.transform.position);
			
				if (minDistance > distance) {
					minDistance = distance;
					possibleTarget = powerUpGO;
				}
			}
		}
		
		return possibleTarget;
	}
	
}
