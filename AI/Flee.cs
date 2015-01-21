using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Flee : Goal {
	
	static float SPHERE_CAST_RADIUS = 150;
	static int SPHERE_CAST_LAYER = 1 << LayerMask.NameToLayer("Submarines");
	
	Steering agentSteering;

	public Flee(Agent owner) {
		this.owner = owner;
		this.agentSteering = owner.GetComponent<Steering>();
		//Debug.Log("Flee Goal");
	}
	
	public override void Activate() {
		base.Activate();
		agentSteering.ActivateFlee();
	}
	
	public override GoalStatus Process() {
		
		ActivateIfInactive();
		
		List<Transform> targets = FindFleeTargets(owner); 
		
		if(targets.Count > 0){
			agentSteering.FleeTargets = targets;
			Status = GoalStatus.ACTIVE;
		} else {
			Status = GoalStatus.COMPLETED;
			Terminate();
		}

		return Status;
	}
	
	public override void Terminate() {
		agentSteering.DeactivateFlee();
	}
	
	
	public static bool EvaluateFlee(Agent owner){
		return FindFleeTargets(owner).Count > 0;
	}
	
	static List<Transform> FindFleeTargets(Agent owner){
		
		int ownerPlayerNumber = owner.GetComponent<SubmarineCore>().TeamNumber;
		List<Transform> targets = new List<Transform>();
		
		Collider[] hits = Physics.OverlapSphere(owner.transform.position, SPHERE_CAST_RADIUS, SPHERE_CAST_LAYER);
		foreach(Collider h in hits){
		
			Transform target = h.transform;
			int targetPlayerNumber = target.GetComponent<SubmarineCore>().TeamNumber;
			
			if( targetPlayerNumber != ownerPlayerNumber){
				targets.Add(target);
			}	
		}
		
		return targets;
		
	}
}