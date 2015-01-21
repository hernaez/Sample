using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FleeAgent : Agent {
	
	bool inFlee = false;

	protected override void Start () {
		base.Start();
	}
	
	public override void Update () {
		
		if(inFlee && currentGoal.Process() == Goal.GoalStatus.COMPLETED){
			inFlee = false;
		
		} else if (!inFlee && Flee.EvaluateFlee(this)){
			
			currentGoal = new Flee(this);
			inFlee = true;
			
		} else{
			
			base.Update();
		}
		  
	}
	
	protected override float EvaluateFindHealth(){
		return 0;
	}
	
	protected override float EvaluateFindEMP(){
		return 0;
	}
}
