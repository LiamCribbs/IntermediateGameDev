using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrower : MonoBehaviour
{
    const float GrowTimestep = 0.0001f;
    float lastGrowTime;

    public Tree tree;
    public bool grow;
    public bool growOnce;
    public int steps = 1;

    void Update()
    {
        if (grow && tree && !tree.fullyGrown)
        {
            if (Time.time - lastGrowTime >= GrowTimestep)
            {
                lastGrowTime = Time.time;
                tree.DoGrowth(steps);
            }
        }
        else if (growOnce)
        {
            growOnce = false;
            tree.DoGrowth(steps);
        }
    }

    void Reset()
    {
        tree = GetComponent<Tree>();
    }
}
