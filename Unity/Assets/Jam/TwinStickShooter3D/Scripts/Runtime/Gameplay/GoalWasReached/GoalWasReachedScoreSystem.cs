﻿using RMC.DOTS.SystemGroups;
using RMC.DOTS.Systems.Player;
using RMC.DOTS.Systems.Scoring;
using Unity.Burst;
using Unity.Entities;

namespace RMC.DOTS.Samples.Games.TwinStickShooter3D
{
    [UpdateInGroup(typeof(PauseableSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct GoalWasReachedScoreSystem : ISystem
    {
        // This query is for all the pickup entities that have been picked up this frame
        private EntityQuery _pickupQuery;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoalWasReachedSystemAuthoring.GoalWasReachedSystemIsEnabledTag>();
            state.RequireForUpdate<ScoringComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerTag, goalWasReached) in 
                     SystemAPI.Query<PlayerTag, GoalWasReachedTag>())
            {
                var pickupCounter = SystemAPI.GetSingleton<ScoringComponent>();
                pickupCounter.ScoreComponent01.ScoreCurrent += 1;
                SystemAPI.SetSingleton(pickupCounter);
            }
        }
    }
}