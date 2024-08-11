using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCoverAvailableNode : Node
{
    private Cover[] availableCovers;
    private Transform target;
    private BeetleAI ai;

    public IsCoverAvailableNode(Cover[] availableCovers, Transform target, BeetleAI ai)
    {
        this.availableCovers = availableCovers;
        this.target = target;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        // Find the best cover spot and set it in the AI
        Transform bestSpot = PickRandomCoverSpot();
        ai.SetBestCoverSpot(bestSpot);

        // Return SUCCESS if a valid spot is found, otherwise return FAILURE
        return bestSpot != null ? NodeState.SUCCESS : NodeState.FAILURE;
    }

    //Ger en random cover
    private Transform PickRandomCoverSpot()
    {
        if (ai.GetBestCoverSpot() != null && CheckIfSpotIsValid(ai.GetBestCoverSpot()))
        {
            return ai.GetBestCoverSpot();
        }

        if (availableCovers.Length == 0)
        {
            return null;
        }

        // Filter available covers to exclude those already occupied by enemies
        List<Cover> availableCoversWithoutEnemies = new List<Cover>();
        foreach (Cover cover in availableCovers)
        {
            if (!IsCoverOccupiedByEnemy(cover))
            {
                availableCoversWithoutEnemies.Add(cover);
            }
        }

        if (availableCoversWithoutEnemies.Count == 0)
        {
            return null;
        }

        // Pick a random cover from filtered availableCovers array
        Cover randomCover = availableCoversWithoutEnemies[UnityEngine.Random.Range(0, availableCoversWithoutEnemies.Count)];

        Transform[] availableSpots = randomCover.GetCoverSpots();

        if (availableSpots.Length == 0)
        {
            return null;
        }

        // Pick a random spot from availableSpots array
        Transform randomSpot = availableSpots[UnityEngine.Random.Range(0, availableSpots.Length)];

        // Check if the randomly picked spot is valid
        if (CheckIfSpotIsValid(randomSpot))
        {
            return randomSpot;
        }
        else
        {
            return null;
        }
    }

    private bool CheckIfSpotIsValid(Transform spot)
    {
        RaycastHit hit;
        Vector3 direction = target.position - spot.position;

        if (Physics.Raycast(spot.position, direction, out hit))
        {
            if (hit.collider.transform != target)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsCoverOccupiedByEnemy(Cover cover)
    {
        foreach (Transform spot in cover.GetCoverSpots())
        {
            if (Vector3.Distance(spot.position, target.position) < 20f)
            {
                return true;
            }
        }
        return false;
    }
}
