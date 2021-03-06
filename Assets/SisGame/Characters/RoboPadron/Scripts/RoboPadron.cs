﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SIS.States;
using SIS.Items;
using SIS.Items.Weapons;

namespace SIS.Characters.Robo
{
	[RequireComponent(typeof(Waypoints.WaypointNavigator))]
	public class RoboPadron : Character, IHittable
	{
		#region StateMachine Setup

		//Must Start Off in a State
		[SerializeField] private RoboPadronState startingState; 

		//Optional. Use StateActionComposite to run multiple actions on create
		[SerializeField] private RoboPadronStateActions initActionsBatch; 

		public StateMachine<RoboPadron> stateMachine;

		private new void Start()
		{
			base.Start();
			stateMachine = new StateMachine<RoboPadron>(this, startingState, initActionsBatch);

		}
		//Run State Machine Logic
		private void FixedUpdate()
		{
			stateMachine.FixedTick();
		}
		private void Update()
		{
			stateMachine.Tick();
			delta = stateMachine.delta;
		}
		public override void ChangeState(int transitionIndex)
		{
			var newState = stateMachine.currentState.transitions[transitionIndex].targetState;
			stateMachine.currentState = newState;
			stateMachine.currentState.OnEnter(this);
		}

		#endregion

		public float health = 1;

		[HideInInspector] public Transform headBone;
		[HideInInspector] public Transform gunModel;
		[HideInInspector] public bool isAiming = false;
		[HideInInspector] public bool canSeePlayer = false;

		[HideInInspector] public Waypoints.WaypointNavigator waypointNavigator;
		[HideInInspector] public Vision vision;

		#region Last Known Position
		[System.Serializable]
		public struct LastKnownLocation
		{
			public Vector3 position;
			public float timeSeen;
		}
		#endregion

		//Serializable Fields
		public LastKnownLocation playerLastKnownLocation;

		//Allows for initial setup, better to use InitActionBatch, but it's here if you don't want to make action
		protected override void SetupComponents()
		{
			waypointNavigator = GetComponent<Waypoints.WaypointNavigator>();
			vision = GetComponent<Vision>();
			headBone = mTransform.FindDeepChild("Head");
			gunModel = mTransform.FindDeepChild("Gun");

			if (headBone == null) Debug.LogWarning("Could not find Head bone on RoboPadron");
		}

		public void OnHit(Character shooter, Weapon weapon, Vector3 dir, Vector3 pos)
		{
			health--;
			rigid.AddForceAtPosition(dir, pos);

			if (health <= 0)
			{
				Destroy(gameObject);
			}
		}
	}
}