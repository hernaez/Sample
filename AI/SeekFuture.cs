using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SeekFuture : Goal {
	
	static float SPHERE_CAST_RADIUS = 150;
	static int SPHERE_CAST_LAYER = 1 << LayerMask.NameToLayer("Submarines");
	
	Steering agentSteering;
	Navigator target;

	public SeekFuture(Agent owner) {
		this.owner = owner;
		this.agentSteering = owner.GetComponent<Steering>();
		this.target = FindTarget(owner).GetComponent<Navigator>();
	}
	
	public override void Activate() {
		base.Activate();
		agentSteering.ActivatePursuitFuture(target);
	}
	
	public override GoalStatus Process() {
		
		ActivateIfInactive();
		
		if (!target.enabled) {
			Status = GoalStatus.COMPLETED;
		
		}
		
		return Status;
	}
	
	public override void Terminate() {
		agentSteering.DeactivatePursuitFuture();
	}
	
	
	public static bool EvaluateSeekFuture(Agent owner){
		return FindTarget(owner) != null;
	}
	
	static Transform FindTarget(Agent owner){
		
		int ownerPlayerNumber = owner.GetComponent<SubmarineCore>().TeamNumber;
		
		Collider[] hits = Physics.OverlapSphere(owner.transform.position, SPHERE_CAST_RADIUS, SPHERE_CAST_LAYER);
		foreach(Collider h in hits){
		
			Transform target = h.transform;
			int targetPlayerNumber = target.GetComponent<SubmarineCore>().TeamNumber;
			
			if( targetPlayerNumber != ownerPlayerNumber){
				return target;
			}	
		}
		
		return null;
		
	}
}