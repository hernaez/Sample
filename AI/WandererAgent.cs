using UnityEngine;
using System.Collections;

public class WandererAgent : Agent {
	
	protected override void Start () {
		base.Start();
	}
	
	public override void Update () {
		if (currentGoal != null && currentGoal.Process() == Goal.GoalStatus.ACTIVE) return;
		
		if(SeekFuture.EvaluateSeekFuture(this)){
			
			currentGoal = new SeekFuture(this);
			
		}else{
			
			base.Update();
		}
		
	}

	/*protected override void Start () {
		base.Start();
		
		currentGoal = new Wander();
		currentGoal.owner = this;
	}
	
	public override void Update () {
		base.Update();
	}
	
	protected override float EvaluateHuntTarget(){
		return 0.0f;
	}
	
	protected override float EvaluateWander(){
		return 0.7f;
	}*/
	
	protected override float EvaluateFindHealth(){
		return 0;
	}
	
	protected override float EvaluateFindEMP(){
		return 0;
	}
}
