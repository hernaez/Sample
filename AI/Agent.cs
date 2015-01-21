using UnityEngine;
using System.Collections;

public class Agent : SubmarineController {
	
	protected Goal currentGoal;
	
	protected Health subHealth;
	protected Steering subSteering;
	protected FireActionsManager subFireManager;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		this.subHealth = GetComponent<Health>();
		this.subSteering = GetComponent<Steering>();
		
		subSteering.ActivateAvoidWalls();
		subSteering.ActivateAvoidMines();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (currentGoal != null && currentGoal.Process() == Goal.GoalStatus.ACTIVE) return;
		
		int evaluateHuntTarget = (int) (EvaluateHuntTarget() * 100);
		int evaluateFindHealth = (int) (EvaluateFindHealth() * 100);
		int evaluateWander = (int) (EvaluateWander() * 100);
		int evaluateFindEMP = (int) (EvaluateFindEMP() * 100);
		
		int[] valuations = new int[4];
		valuations[0] = evaluateHuntTarget;
		valuations[1] = evaluateFindHealth;
		valuations[2] = evaluateWander;
		valuations[3] = evaluateFindEMP;
		
		int max = Mathf.Max(valuations);
		
		if(max == evaluateHuntTarget ) currentGoal = new HuntTarget();
		else if(max == evaluateFindHealth ) currentGoal = new FindPowerUp(PowerUpType.HEALTH, this);
		else if(max == evaluateFindEMP ) currentGoal = new FindPowerUp(PowerUpType.EMP, this);
		else if(max == evaluateWander ) currentGoal  = new Wander();
		else currentGoal = new Wander();
		
		//currentGoal = new FindPowerUp(PowerUpType.EMP, this);
		currentGoal.owner = this;
	}
	
	protected virtual float EvaluateHuntTarget(){

		SubmarineCore possibleTarget = HuntTarget.EvaluateTargets(this);
		
		if(possibleTarget == null){
			return 0.0f;
		}
		
		float distance = Vector3.Distance(possibleTarget.transform.position, transform.position);
		if(distance > HuntTarget.maxDistance){
			return 0.3f;
		}
		
		return 0.75f;
	}
	
	protected virtual float EvaluateFindHealth(){
		
		GameObject target = FindPowerUp.EvaluateTargets(PowerUpType.HEALTH, this);
		float hpPercentage = subHealth.HP / subHealth.maxHealthPoints;
		
		if(target == null){
			return 0.0f;
		
		}else if( hpPercentage < 0.3){
			return 0.9f;
		}
		
		return 1 - hpPercentage;
	}
	
	protected virtual float EvaluateFindEMP(){
		
		GameObject target = FindPowerUp.EvaluateTargets(PowerUpType.EMP, this);
		
		if(target == null || fireActionsManager.PowerUpType != PowerUpType.NONE){
			return 0.0f;
		}
		
		return 0.85f;
	}
	
	protected virtual float EvaluateWander(){
		
		if( currentGoal != null && currentGoal.GetType() == typeof(HuntTarget) && 
		   	currentGoal.Process() == Goal.GoalStatus.FAILED)
			return 0.8f;
		
		return 0.2f;
	}
	
	protected override void OnFirePrimaryWeapon(float reloadTime) {
	}
	
	protected override void OnFireSecondaryWeapon(float reloadTime) {
	}
	
	protected override void OnSecondaryWeaponEnabled(bool enabled) {
	}
	
	protected override void OnDash(float reloadTime) {
	}
	
	protected override void OnPowerUpEnabled(bool enabled) {
	}
	
	protected override void OnPowerUpCatched(float reloadTime) {
	}
}
