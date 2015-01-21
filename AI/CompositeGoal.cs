using UnityEngine;
using System.Collections.Generic;

public class CompositeGoal : Goal {
	
	protected List<Goal> subGoals;
	
	protected CompositeGoal() {
		subGoals = new List<Goal>();
	}
		
	public override GoalStatus Process() {
		return GoalStatus.ACTIVE;
	}
	
	public override void Terminate() {
	}	
	
	protected GoalStatus ProcessSubGoals() {
		while (subGoals.Count > 0 &&
		       ((subGoals[0].Status == GoalStatus.COMPLETED) ||
		       (subGoals[0].Status == Goal.GoalStatus.FAILED))) {
			
			subGoals[0].Terminate();
			subGoals.RemoveAt(0);         
		}
		
		if (subGoals.Count > 0) {
			Status = subGoals[0].Process();
			
			if (Status == GoalStatus.COMPLETED && subGoals.Count > 1)
				Status = GoalStatus.ACTIVE;
						
		} else {			
			Status = GoalStatus.COMPLETED;
		}
		return Status;
	}	
}
