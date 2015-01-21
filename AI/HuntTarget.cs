using UnityEngine;
using System.Collections.Generic;

public class HuntTarget : CompositeGoal {
	
	List<SubmarineCore> enemySubmarines;
	
	public static float maxDistance = 300;
	
	public HuntTarget(){
		//Debug.Log("HuntTarget Goal");
	}
	
	public override void Activate () {
		
		base.Activate();
		
		enemySubmarines = new List<SubmarineCore>(4);
		
		GameObject[] submarines = GameObject.FindGameObjectsWithTag("Submarine");		
		foreach (GameObject submarine in submarines) {
			SubmarineCore submarineCore = submarine.GetComponent<SubmarineCore>();
			if (submarineCore.TeamNumber != owner.GetComponent<SubmarineCore>().TeamNumber) {
				enemySubmarines.Add(submarineCore);
			}
		}				
	}
	
	public override GoalStatus Process () {
		
		ActivateIfInactive();
		
		GoalStatus gs = ProcessSubGoals();
		
		if (gs == GoalStatus.COMPLETED){
			
			SubmarineCore target = EvaluateTargets(owner);
			
			if (target != null) {
				AttackTarget attackTarget = new AttackTarget(target);
				attackTarget.owner = owner;
				subGoals.Add(attackTarget);
				Status = Goal.GoalStatus.ACTIVE;
			}
		}
				
		if (gs == GoalStatus.FAILED)
			Status = GoalStatus.FAILED;	
		
		if (subGoals.Count == 0)
			Status = GoalStatus.COMPLETED;
			
		return Status;
	}
	
	public static SubmarineCore EvaluateTargets(Agent owner) {
		
		SubmarineCore target = null;
		
		List<SubmarineCore> enemySubmarines = new List<SubmarineCore>(4);
		
		GameObject[] submarines = GameObject.FindGameObjectsWithTag("Submarine");		
		foreach (GameObject submarine in submarines) {
			SubmarineCore submarineCore = submarine.GetComponent<SubmarineCore>();
			if (submarineCore.TeamNumber != owner.GetComponent<SubmarineCore>().TeamNumber) {
				enemySubmarines.Add(submarineCore);
			}
		}	
		
		if (enemySubmarines.Count > 0) {
			float targetPoints = 0;
			
			foreach (SubmarineCore sc in enemySubmarines) {
				if (sc.enabled) {
					float hp = sc.GetComponent<Health>().HP;
					float d = Vector3.Distance(sc.transform.position, owner.transform.position);
					float tp = hp / d;
					if (tp > targetPoints) {
						target = sc;					
					}
				}
			}		
		}
		
		return target;
	}
}
