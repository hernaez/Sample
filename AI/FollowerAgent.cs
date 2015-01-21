using UnityEngine;
using System.Collections;

public class FollowerAgent : Agent {
	
	protected override void Start () {
		base.Start();
		
		currentGoal = new HuntTarget();
		currentGoal.owner = this;
	}
	
	public override void Update () {
		base.Update();
		
	}
	
	protected override float EvaluateFindHealth(){
		return 0;
	}
	
	protected override float EvaluateFindEMP(){
		return 0;
	}
}
