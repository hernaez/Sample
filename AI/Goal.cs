using UnityEngine;
using System.Collections;

public abstract class Goal {
	
	public enum GoalStatus {
		INACTIVE,
		ACTIVE,
		COMPLETED,
		FAILED
	};
	
	public Agent owner;
	GoalStatus goalStatus;
	
	public virtual void Activate() {
		goalStatus = Goal.GoalStatus.ACTIVE;
	}
	
	public abstract GoalStatus Process();
	
	public abstract void Terminate();
	
	public GoalStatus Status {
		get {
			return goalStatus;
		}
		set {
			goalStatus = value;
		}
	}
	
	protected void ActivateIfInactive() {
		if (Status == GoalStatus.INACTIVE)
			Activate();
	}
	
}
