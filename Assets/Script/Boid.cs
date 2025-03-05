using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    public float speed = 5f;
    public float neighborRadius = 3f;
    public float separationDistance = 1.5f;

    [Header("Random Speed")] 
    public bool needRandom;
    public float minSpeed;
    public float maxSpeed;
    public float randomTime;
    private float currentRandom;

    private Vector3 velocity;
    private List<Boid> neighbors;

    public float CoeffSeparation = 1.0f;
    public float CoeffAligment = 0.05f;
    public float CoeffCohesion = 0.01f;

    private void Start()
    {
        velocity = Random.insideUnitSphere * speed;
    }

    private void Update()
    {
        neighbors = GetNeighbors();
        Vector3 separation = CalculateSeparation();
        Vector3 aligment = CalculateAligment();
        Vector3 cohesion = CalculateCohesion();

        velocity += separation + aligment + cohesion;
        velocity = velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;

        if (!needRandom) return;
        currentRandom += Time.deltaTime;
        if (currentRandom >= randomTime)
        {
            speed = Random.Range(minSpeed, maxSpeed);
            currentRandom = 0;
        }
    }

    private List<Boid> GetNeighbors()
    {
        List<Boid> nearbyBoids = new List<Boid>();
        foreach (Boid boid in FindObjectsByType<Boid>(FindObjectsSortMode.None))
        {
            if (boid != this && Vector3.Distance(transform.position, boid.transform.position) < neighborRadius)
            {
                nearbyBoids.Add(boid);
            }
        }
        return nearbyBoids;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        foreach (Boid boid in neighbors)
        {
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < separationDistance)
            {
                separationForce += (transform.position - boid.transform.position).normalized / distance;
            }
        }
        return separationForce * CoeffSeparation;
    }
    
    Vector3 CalculateAligment()
    {
        if (neighbors.Count == 0) return Vector3.zero;
        Vector3 avgVelocity = Vector3.zero;
        foreach (Boid boid in neighbors)
        {
            avgVelocity += boid.velocity;
        }
        avgVelocity /= neighbors.Count;
        return (avgVelocity - velocity) * CoeffAligment;
    }

    Vector3 CalculateCohesion()
    {
        if (neighbors.Count == 0) return Vector3.zero;
        Vector3 centerOfMass = Vector3.zero;
        foreach (Boid boid in neighbors)
        {
            centerOfMass += boid.transform.position;
        }
        centerOfMass /= neighbors.Count;
        return (centerOfMass - transform.position) * CoeffCohesion;
    }
    
    
}
