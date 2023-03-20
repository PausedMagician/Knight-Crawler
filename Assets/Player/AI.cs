using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : Humanoid
{

    new void FixedUpdate() {
        CheckDirections();
        //For loop that finds highest float in IDictionary
        //Then sets movementDirection to that key
        //Then calls Move()
        float lowest = weights.Values.ElementAt(0);
        for (int i = 0; i < weights.Count; i++)
        {
            if(weights.Values.ElementAt(i) < lowest)
            {
                lowest = weights.Values.ElementAt(i);
                movementDirection = weights.Keys.ElementAt(i);
            }
        }
        // Debug.Log(movementDirection);
        base.FixedUpdate();
    }

    IDictionary<Vector2, float> weights = new Dictionary<Vector2, float>();
    public List<Vector2> points = new List<Vector2>();
    public int pointsDuplicator = 2;
    public void CheckDirections()
    {
        points.Clear();
        int amountPoints = pointsDuplicator;
        if(points.Count != pointsDuplicator || points.Count == 0 || points.Count % 2 != 0 || points.Count != pointsDuplicator+1) {
            points.Clear();
            if(amountPoints % 2 != 0)
            {
                amountPoints++;
            }
            for (int i = 1; i <= amountPoints; i++)
            {
                points.Add(new Vector2(Mathf.Cos(Mathf.PI * 2 * i / amountPoints), Mathf.Sin(Mathf.PI * 2 * i / amountPoints)).normalized);
            }
        }

        // points.Add(new Vector2(1, 0));
        // points.Add(new Vector2(1, 1).normalized);
        // points.Add(new Vector2(0, 1));
        // points.Add(new Vector2(-1, 1).normalized);
        // points.Add(new Vector2(-1, 0));
        // points.Add(new Vector2(-1, -1).normalized);
        // points.Add(new Vector2(0, -1));
        // points.Add(new Vector2(1, -1).normalized);

        List<float> weightsList = new List<float>();
        for (int i = 0; i < points.Count; i++)
        {
            weightsList.Add(CheckWeight(points[i]));
        }
        float ratio = amountPoints / weightsList.Min();
        List<float> normalizedList = weightsList.Select(i => i * ratio).ToList();
        // List<float> normalizedList = weightsList;
        weights.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            weights.Add(points[i], normalizedList[i]);
        }
    }
    public float CheckWeight(Vector2 point)
    {
        float weight = 0;
        float AIWeight = 0.25f;
        float WallWeight = 100f;
        float playerWeight = 5f;
        Vector2 worldSpace = (Vector2)transform.position + point;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, worldSpace - (Vector2)transform.position, 5f);
        Debug.DrawRay(transform.position, worldSpace - (Vector2)transform.position, Color.red, 0.1f);
        if(hit.collider) {
            if(hit.GetType() == typeof(AI)) {
                weight += AIWeight * hit.distance;
            } else {
                weight += WallWeight * hit.distance;
            }
        }
        Vector2 player = Player.GetInstance().transform.position;
        weight += Vector2.Distance(worldSpace, player) * playerWeight;
        return weight;
    }
    private void OnDrawGizmos() {
        for (int i = 0; i < weights.Count; i++)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, weights.ElementAt(i).Value / weights.Values.Max());
            Gizmos.DrawSphere((Vector2)transform.position + weights.ElementAt(i).Key, 0.1f);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}