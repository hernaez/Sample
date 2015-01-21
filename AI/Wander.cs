using UnityEngine;
using System.Collections.Generic;

public class Wander : CompositeGoal {
	
	List<SubmarineCore> enemySubmarines;
	
	float acceptedTimeMin = 1;
	float acceptedTimeMax = 2.5f;
	
	float acceptedTime;
	float currentTime;
	
	Steering steering;

	public Wander() {
		this.acceptedTime = Random.Range(acceptedTimeMin, acceptedTimeMax);
		this.currentTime = 0;
		//Debug.Log("Wander Goal");
	}
	
	public override void Activate () {
		base.Activate();
		
		owner.GetComponent<Steering>().ActivateWander();
	}
	
	public override void Terminate() {
		owner.GetComponent<Steering>().DeactivateWander();
	}
	
	public override GoalStatus Process () {
		
		ActivateIfInactive();
		
		currentTime += Time.deltaTime;
		
		if (currentTime >= acceptedTime){
			Status = GoalStatus.COMPLETED;
			Terminate();
		}
			
		return Status;
	}
}
