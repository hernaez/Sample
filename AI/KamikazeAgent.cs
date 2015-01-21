using UnityEngine;
using System.Collections;

public class KamikazeAgent : Agent {

	protected override void Start () {
		base.Start();
		
		currentGoal = new HuntTarget();
		currentGoal.owner = this;
	}
	
	public override void Update () {
		base.Update();
		
	}
	protected virtual void OnCollisionEnter(Collision other) { 
		Transform collisionTransform = other.transform;
		SubmarineCore submarineHit = other.transform.GetComponent<SubmarineCore>();
		if (null != submarineHit){
			if(submarineHit.PlayerNumber == 0){
				//die
				GetComponent<Health>().ReduceHP(100, Health.ChangeCause.SUBMARINE,this.gameObject,WeaponType.PROXIMITY_MINE);
				// damage!
				submarineHit.GetComponent<Health>().ReduceHP(30, Health.ChangeCause.SUBMARINE, this.gameObject,WeaponType.PROXIMITY_MINE);
				//submarineHit.GetComponent<Navigator>().addImpact(this.transform.forward,constForce);//todo: magic number
			}
		}
		
	}
	
	protected override float EvaluateFindHealth(){
		return 0;
	}
	protected override float EvaluateFindEMP(){
		return 0;
	}
	protected virtual float EvaluateWander(){
		return 0;
	}
}

