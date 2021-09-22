using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;

    public int numTrees;
    public Vector2 rootOffset;
    [Range(0f, 1f)] public float terrainSlantWeight;

    public List<Vector2> points = new List<Vector2>();

    [Space(30)]
    public Transform player;
    public List<EdgeCollider2D> terrainChunks = new List<EdgeCollider2D>();
    public List<GameObject[]> trees = new List<GameObject[]>(3);

    [Space(30)]
    public TreeSettings[] treeSettings;
    public List<Sprite>[] treeSprites;
    TreeData generatingTree;
    const int MaxTreesPerType = 20;

    int currentTreeType;
    public int nextTreeType;

    TreeSettings RandomSettings()
    {
        return treeSettings[Random.Range(0, treeSettings.Length)];
    }

    void Start()
    {
        treeSprites = new List<Sprite>[treeSettings.Length];
        for (int i = 0; i < treeSprites.Length; i++)
        {
            treeSprites[i] = new List<Sprite>(MaxTreesPerType);
        }

        for (int j = 0; j < treeSprites.Length; j++)
        {
            TreeData treeData = new TreeData(treeSettings[j]);
            treeData.Start();
            int index = System.Array.IndexOf(treeSettings, treeData.settings);
            treeSprites[index].Add(treeData.Draw());
            if (treeSprites[index].Count > MaxTreesPerType)
            {
                treeSprites[index].RemoveAt(0);
            }

            //treeData = new TreeData(treeSettings[j]);
            //treeData.Start();
            //index = System.Array.IndexOf(treeSettings, treeData.settings);
            //treeSprites[index].Add(treeData.Draw());
            //if (treeSprites[index].Count > MaxTreesPerType)
            //{
            //    treeSprites[index].RemoveAt(0);
            //}
        }

        for (int i = 0; i < terrainChunks.Count; i++)
        {
            trees.Add(new GameObject[numTrees]);

            LoadNewChunk(terrainChunks[i], i);
        }
    }

    void Update()
    {
        EdgeCollider2D currentChunk = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < terrainChunks.Count; i++)
        {
            float distance = (player.position - terrainChunks[i].transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                currentChunk = terrainChunks[i];
            }
        }

        if (currentChunk == terrainChunks[terrainChunks.Count - 1])
        {
            EdgeCollider2D chunkToMove = terrainChunks[0];
            terrainChunks.Remove(chunkToMove);
            terrainChunks.Add(chunkToMove);
            var treesToMove = trees[0];
            trees.Remove(treesToMove);
            trees.Add(treesToMove);

            chunkToMove.transform.position = terrainChunks[terrainChunks.Count - 2].transform.position + new Vector3(3.2f * 2f, -1.8f * 2f, 0f);

            LoadNewChunk(chunkToMove, terrainChunks.Count - 1);
        }

        if (generatingTree == null)
        {
            generatingTree = TreeData.GrowTreeOnNewThread(RandomSettings());
        }
        else
        {
            if (generatingTree.fullyGrown)
            {
                int index = System.Array.IndexOf(treeSettings, generatingTree.settings);
                treeSprites[index].Add(generatingTree.Draw());
                if (treeSprites[index].Count > MaxTreesPerType)
                {
                    treeSprites[index].RemoveAt(0);
                }

                generatingTree = null;
            }
        }
    }

    void LoadNewChunk(EdgeCollider2D chunk, int index)
    {
        currentTreeType = nextTreeType;
        nextTreeType = Random.Range(0, treeSprites.Length);

        points.Clear();
        int numPoints = chunk.GetPoints(points);

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

            Destroy(trees[index][i]);
            GameObject tree = Instantiate(treePrefab, (Vector2)chunk.transform.position + spawnPoint + rootOffset, angle);

            int treeType = Random.Range(currentTreeType, nextTreeType + 1);
            int treeIndex = Random.Range(0, treeSprites[treeType].Count);
            tree.GetComponent<SpriteRenderer>().sprite = treeSprites[treeType][treeIndex];
            trees[index][i] = tree;
        }
    }
}