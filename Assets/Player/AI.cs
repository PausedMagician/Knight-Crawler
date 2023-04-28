using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : Humanoid
{
    new void Start()
    {
        base.Start();
        EquipArmor(GameController.GetInstance().CreateArmor(Rarity.Common, 1));
        EquipWeapon(GameController.GetInstance().CreateMelee(Rarity.Common, 1));
    }
    new void FixedUpdate()
    {
        CheckDirections();

        base.FixedUpdate();
    }


    [Header("AI")]
    public List<Vector2> weights, feathers;

    public List<Vector2> points;
    public int amountOfPoints = 2;
    public Vector2 lastKnownLocation;
    public float viewDistance = 10f;

    public void CheckDirections()
    {
        if (points == null)
        {
            points = new Vector2[amountOfPoints].ToList();
        }
        if (weights == null)
        {
            weights = new Vector2[amountOfPoints].ToList();
            feathers = new Vector2[amountOfPoints].ToList();
        }
        if (points.Count != this.amountOfPoints || points.Count == 0 || points.Count % 2 != 0 || points.Count != this.amountOfPoints + 1)
        {
            points.Clear();
            if (amountOfPoints % 2 != 0)
            {
                amountOfPoints++;
            }
            for (int i = 1; i <= amountOfPoints; i++)
            {
                points.Add(new Vector2(Mathf.Cos(Mathf.PI * 2 * i / amountOfPoints), Mathf.Sin(Mathf.PI * 2 * i / amountOfPoints)).MNormalize());
            }
        }

        Vector2 player = (Vector2)Player.GetInstance().transform.position;

        //Check for player
        RaycastHit2D[] whits = Physics2D.RaycastAll(transform.position, player - (Vector2)transform.position, viewDistance);
        // Debug.DrawRay(transform.position, player - (Vector2)transform.position, Color.green, 0.1f);
        //Sort hits by distance
        whits = whits.OrderBy(x => Vector2.Distance(x.point, transform.position)).ToArray();
        foreach (RaycastHit2D hit in whits)
        {
            if (hit.collider)
            {
                if (hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.layer != 6)
                {
                    break;
                }
                else
                {
                    // Debug.Log("Player found!");
                    lastKnownLocation = player;
                }
            }
        }
        if (Vector2.Distance(transform.position, lastKnownLocation) > 1.5f)
        {
            if (weights.Count != points.Count)
            {
                weights.Clear();
                float PPDistance;
                float TPDistance = Vector2.Distance((Vector2)transform.position, lastKnownLocation);
                for (int i = 0; i < points.Count; i++)
                {
                    if ((PPDistance = Vector2.Distance((Vector2)transform.position + points[i], lastKnownLocation)) < TPDistance)
                    {
                        weights.Add(points[i] * (TPDistance - PPDistance));
                    }
                    else
                    {
                        weights.Add(Vector2.zero);
                    }
                }
            }
            else
            {
                float PPDistance;
                float TPDistance = Vector2.Distance((Vector2)transform.position, lastKnownLocation);
                for (int i = 0; i < points.Count; i++)
                {
                    if ((PPDistance = Vector2.Distance((Vector2)transform.position + points[i], lastKnownLocation)) < TPDistance)
                    {
                        weights[i] = points[i] * (TPDistance - PPDistance);
                    }
                    else
                    {
                        weights[i] = Vector2.zero;
                    }
                }
            }

            //Normalize list
            float max = weights.Max(x => x.magnitude);
            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] /= max;
            }
            // Debug.Log(weights.ToDebugString());
            feathers.Clear();
            //Check for walls
            for (int i = 0; i < points.Count; i++)
            {
                RaycastHit2D[] fhits = Physics2D.RaycastAll(transform.position, points[i], 5f);
                // Debug.DrawRay(transform.position, points[i], Color.red, 0.1f);
                //Sort hits by distance
                fhits = fhits.OrderBy(x => Vector2.Distance(x.point, transform.position)).ToArray();
                foreach (RaycastHit2D hit in fhits)
                {
                    if (hit.collider)
                    {
                        if (hit.collider.gameObject.layer == 6)
                        {
                            // Debug.Log("Player!");
                            continue;
                        }
                        else if (hit.collider.gameObject.GetComponent<AI>() == this)
                        {
                            continue;
                        }
                        else
                        {
                            feathers.Add(points[i] * (5f - hit.distance));
                            // Debug.Log(points[i] * (5f - hit.distance));
                            break;
                        }
                    }
                    else
                    {
                        feathers.Add(Vector2.zero);
                        break;
                    }
                }
                if (feathers.Count <= i)
                {
                    feathers.Add(Vector2.zero);
                }
            }

            //Normalize list
            max = feathers.Max(x => x.magnitude);
            for (int i = 0; i < feathers.Count; i++)
            {
                feathers[i] /= max;
            }

            // Subtract feathers magnitude from weights
            for (int i = 0; i < weights.Count; i++)
            {
                float diff = Mathf.Abs(weights[i].magnitude - feathers[i].magnitude);
                weights[i] *= diff;
            }

            Vector2 average = Vector2.zero;
            for (int i = 0; i < weights.Count; i++)
            {
                average += weights[i];
            }
            average /= weights.Count;
            average = average.MNormalize();
            movementDirection = average;
        }

    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.green, weights.ElementAt(i).magnitude);
            Gizmos.DrawSphere((Vector2)transform.position + points.ElementAt(i).MNormalize(), 0.1f);
            Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + weights.ElementAt(i));
            Gizmos.color = Color.Lerp(Color.green, Color.red, feathers.ElementAt(i).magnitude);
            Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + feathers.ElementAt(i));
        }
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(lastKnownLocation, 0.1f);
    }
}

public static class Vector2Extensions
{
    public static Vector2 MNormalize(this Vector2 v)
    {
        float distance = v.Length();

        if (distance > 0.000001f) // compare to small non-zero value
            v /= distance; // by definition of Normalize(), eliminates the second square root
        else
            v = new Vector2(0, 0);

        return v;
    }
    public static float Length(this Vector2 v)
    {
        return (float)Mathf.Sqrt(v.x * v.x + v.y * v.y);
    }
}