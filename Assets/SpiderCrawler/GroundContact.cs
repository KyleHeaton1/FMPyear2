using UnityEngine;
using Unity.MLAgents;

namespace Unity.MLAgentsExamples
{
    //Parts of this script were built by Unity but has been adjusted to work into Frogzilla
    /// <summary>
    /// This class contains logic for locomotion agents with joints which might make contact with the ground.
    /// By attaching this as a component to those joints, their contact with the ground can be used as either
    /// an observation for that agent, and/or a means of punishing the agent for making undesirable contact.
    /// </summary>
    [DisallowMultipleComponent]
    public class GroundContact : MonoBehaviour
    {
        [HideInInspector] public Agent agent;

        [Header("Ground Check")] public bool agentDoneOnGroundContact; // Whether to reset agent on ground contact.
        public bool penalizeGroundContact; // Whether to penalize on contact.
        public float groundContactPenalty; // Penalty amount (ex: -1).
        public bool touchingGround;
        const string k_Ground = "ground"; // Tag of ground object.

        /// <summary>
        /// Check for collision with ground, and optionally penalize agent.
        /// </summary>
        void OnCollisionEnter(Collision col)
        {
            if (col.transform.CompareTag(k_Ground))
            {
                //This part of the script is built to make sure that the spider mech gets punished for failure
                touchingGround = true;
                if (penalizeGroundContact)
                {
                    //We use this to punish or reward the agent for touching the ground
                    agent.SetReward(groundContactPenalty);
                }

                //If the agent flips or the whole body contacts the floor, the spider will die
                if (agentDoneOnGroundContact)
                {
                    //agent.EndEpisode();
                    //DEATH!!
                }
            }
            //If the spider touches the obstacles, the traning episode will end and the agent will be punished
            if(col.transform.CompareTag("Buildings"))
            {
                agent.AddReward(-10f);
                //agent.EndEpisode();
            }
        }

        /// <summary>
        /// Check for end of ground collision and reset flag appropriately.
        /// </summary>
        void OnCollisionExit(Collision other)
        {
            if (other.transform.CompareTag(k_Ground))
            {
                touchingGround = false;
            }
        }
    }
}
