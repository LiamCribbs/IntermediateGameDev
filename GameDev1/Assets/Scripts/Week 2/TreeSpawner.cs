using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;

    public EdgeCollider2D terrain;
    public int numTrees;
    public Vector2 rootOffset;
    [Range(0f, 1f)] public float terrainSlantWeight;

    public List<Vector2> points = new List<Vector2>();

    [Space(30)]
    public int initialTrees;
    public List<Sprite> treeSprites = new List<Sprite>();

    [Space(30)]
    public Transform player;
    public List<Transform> terrainChunks = new List<Transform>();

    void Start()
    {
        SetTrees();
    }

    void Update()
    {
        Transform currentChunk = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < terrainChunks.Count; i++)
        {
            float distance = (player.position - terrainChunks[i].position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                currentChunk = terrainChunks[i];
            }
        }

        if (currentChunk == terrainChunks[terrainChunks.Count - 1])
        {
            Transform chunkToMove = terrainChunks[0];
            terrainChunks.Remove(chunkToMove);
            terrainChunks.Add(chunkToMove);

            chunkToMove.transform.position = terrainChunks[terrainChunks.Count - 2].transform.position + new Vector3(3.2f * 2f, -1.8f * 2f, 0f);
        }
    }

    void SpawnInitialTrees()
    {

    }

    void SetTrees()
    {
        points.Clear();
        int numPoints = terrain.GetPoints(points);

        for (int i = 0; i < numTrees; i++)
        {
            // Pick two random sequential points
            int pointIndex = Random.Range(0, numPoints - 1);
            Vector2 firstPoint = points[pointIndex];
            Vector2 secondPoint = points[pointIndex + 1];

            // Choose a point somewhere in between our two points
            Vector2 spawnPoint = Vector2.LerpUnclamped(firstPoint, secondPoint, Random.value);

            // Slant tree according to terrain
            Quaternion angle = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(0f, Vector2.SignedAngle(Vector2.right, secondPoint - firstPoint), terrainSlantWeight));

            Instantiate(treePrefab, (Vector2)terrain.transform.position + spawnPoint + rootOffset, angle);
        }
    }
}