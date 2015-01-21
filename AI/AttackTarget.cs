using UnityEngine;
using System.Collections;

public class AttackTarget : Goal {
	
	SubmarineCore target;
	float targetHP;
	
	float acceptedTimeMin = 3;
	float acceptedTimeMax = 5;
	
	float acceptedTime;
	float currentTime;
	
	public AttackTarget(SubmarineCore target) {
		this.target = target;
		this.targetHP = target.GetComponent<Health>().HP;
		
		this.acceptedTime = Random.Range(acceptedTimeMin, acceptedTimeMax);
		this.currentTime = 0;
	}
	
	public override void Activate() {
		base.Activate();
		owner.GetComponent<Steering>().ActivatePursuit(target.GetComponent<Navigator>());
	}
	
	public override GoalStatus Process() {
		
		ActivateIfInactive();
		
		if (!target.enabled) {
			Status = GoalStatus.COMPLETED;
		} else {
		
			currentTime += Time.deltaTime;
			
			bool sameHP = target.GetComponent<Health>().HP == targetHP;
			bool timeFinish = currentTime >= acceptedTime;
			
			if(timeFinish && sameHP ){
				Status = GoalStatus.FAILED;
						
			}else if(timeFinish){
				
				currentTime = 0;
				targetHP = target.GetComponent<Health>().HP;
			}
		}
			
		return Status;
	}
	
	public override void Terminate() {
		owner.GetComponent<Steering>().DeactivatePursuit();
	}
	
}
