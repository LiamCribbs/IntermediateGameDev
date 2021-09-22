using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TreeSettings : ScriptableObject
{
    public int seed;

    [Space(10)]
    [Range(0f, 0.5f)] public float maxTrunkCurveStart = 0.02f;
    [Range(0f, 0.5f)] public float maxTrunkCurveEnd = 0.02f;

    [Space(10)]
    public AnimationCurve trunkAgeGrowthCurve;
    [Range(0f, 1f)] public float trunkSplitChance = 0.001f;

    [Space(10)]
    [Range(0f, 1f)] public float trunkBranchChance = 0.1f;
    public AnimationCurve trunkBranchChanceCurve = AnimationCurve.Constant(1, 1, 0);
    [Range(0f, 1f)] public float branchBranchChance = 0f;

    [Space(10)]
    [Range(0f, 2f)] public float minBranchAngle = 0.1f;
    [Range(0f, 2f)] public float maxBranchAngle = 0.45f;
    public bool curveBranches;
    [Range(0f, 4f)] public float minTargetBranchAngle;
    [Range(0f, 4f)] public float maxTargetBranchAngle;
    public AnimationCurve branchTargetAngleCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve branchAngleJitterCurve = AnimationCurve.Linear(0, 0, 1, 0.35f);

    [Space(10)]
    public bool spawnBranchAtTopOfTrunk = true;
    public bool spawnLeafAtTopOfTrunk = true;
    public bool setLeafColorPerBranch;

    [Range(0f, 2f)] public float preferredLeafAngle = 1.5f;
    public AnimationCurve preferredLeafAngleCurve;

    [Range(0f, 1f)] public float branchLeafChance = 0.05f;
    public AnimationCurve branchLeafChanceCurve;
    public AnimationCurve leafAgeGrowthCurve;

    [Space(10)]
    public ParticleSettings trunkSettings;
    [Space(10)]
    public ParticleSettings branchSettings;
    [Space(10)]
    public ParticleSettings leafSettings;

    [Space(10)]
    [Range(0f, 1f)] public float flowerChance;
    public ParticleSettings flowerSettings;

    [ContextMenu("SwitchColors")]
    public void SwitchLeafColors()
    {
        Color[] startCols = leafSettings.startColors;
        leafSettings.startColors = leafSettings.endColors;
        leafSettings.endColors = startCols;
    }

    [System.Serializable]
    public class ParticleSettings
    {
        public float minSpeed, maxSpeed;

        [Space(10)]
        public Color[] startColors;
        public Color[] endColors;
        public bool blendColors;
        public bool blendSeparately = false;

        [Space(10)]
        public int minLifetime, maxLifetime;
        public int minCellLifetime, maxCellLifetime;
        public float ageGrowthMultiplier;

        [Space(10)]
        public bool setColorByParticleLife;
        public Vector3 colorJitter = new Vector3(0.02f, 0.04f, 0.04f);
    }
}