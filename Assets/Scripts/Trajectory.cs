//Author: Kritik Patel

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory
{
    public float maxHeight;
    public float maxHeightTime;
    public float maxFallTime;
    public Vector2[] points;
    public float maxTime = 2f;          //Total time to record trajectory data
    public float timeStep = 0.1f;       //Time interval between data points
    public int numPoints;               //Number of points in trajectory
    public int belowStartPoint;          //First point that goes below the start position

    public Vector2 startPos;            //Starting position of trajectory
    public float xVelocity;
    public float jumpSpeed;
    public float fallMultiplier;

    public Trajectory(Vector2 start, float xVelocity, float jumpSpeed, float fallMultiplier, bool canFallBelowStart)
    {
        //Debug.Log("Trajectory from " + start.x + ", " + start.y);

        bool findBelowStart = true;
        startPos = start;
        this.xVelocity = xVelocity;
        this.jumpSpeed = jumpSpeed;
        this.fallMultiplier = fallMultiplier;

        maxHeightTime = jumpSpeed / -Physics2D.gravity.y;
        maxHeight = start.y + jumpSpeed * maxHeightTime + 0.5f * Physics2D.gravity.y * Mathf.Pow(maxHeightTime, 2);
        maxFallTime = Mathf.Sqrt((2 * maxHeight) / (-Physics2D.gravity.y * fallMultiplier));

        if (!canFallBelowStart)
        {
            maxTime = maxHeightTime + maxFallTime;
        }

        numPoints = (int) (maxTime / timeStep) + 1;
        points = new Vector2[numPoints];
        float time = 0f;
        int midPoint = (int) ((maxHeightTime / timeStep) + 1f);
        
        //Obtain points before max height
        for(int point = 0; point < midPoint; point++)
        {
            time = point * timeStep;
            points[point].x = start.x + xVelocity * time;
            points[point].y = start.y + jumpSpeed * time + 0.5f * Physics2D.gravity.y * Mathf.Pow(time, 2);

            //Debug.Log("Position of point " + point + ": " + points[point].x + ", " + points[point].y);
        }

        //Obtain points after max height (accounting for a fall multiplier)
        for(int point = midPoint; point < numPoints - 1; point++)
        {
            time = point * timeStep;
            points[point].x = start.x + xVelocity * time;
            points[point].y = maxHeight + 0.5f * Physics2D.gravity.y * fallMultiplier * Mathf.Pow(time - maxHeightTime, 2);
            
            //Store the first point that goes below the initial start position
            if (findBelowStart && points[point].y < start.y)
            {
                belowStartPoint = point;
                findBelowStart = false;
            }

            //Debug.Log("Position of point " + point + ": " + points[point].x + ", " + points[point].y);
        }
        
        //Final point at maxTime
        points[numPoints - 1].x = start.x + xVelocity * maxTime;
        points[numPoints - 1].y = maxHeight + 0.5f * Physics2D.gravity.y * fallMultiplier * Mathf.Pow(maxTime - maxHeightTime, 2);
        //Store the first point that goes below the initial start position
        if (findBelowStart && points[numPoints - 1].y < start.y)
        {
            belowStartPoint = numPoints - 1;
            findBelowStart = false;
        }
        //Debug.Log("Position of final point " + (numPoints - 1) + ": " + points[numPoints - 1].x + ", " + points[numPoints - 1].y);
    }
    
}