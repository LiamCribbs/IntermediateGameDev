using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;

public class Tree : MonoBehaviour
{
    Random random;
    int seed;
    int growIterations = 0;

    public new SpriteRenderer renderer;
    Texture2D texture;
    Sprite sprite;

    ColorMap colorMap;

    public bool growInstantly;
    [HideInInspector] public bool fullyGrown;

    [Space(20)]
    public TreeSettings settings;

    public List<Particle> particles = new List<Particle>();
    public List<Particle> cells = new List<Particle>();
    ConcurrentBag<int> particlesToRemove = new ConcurrentBag<int>();

    public class ColorMap
    {
        //SEPARATE DICTIONARY FOR TRUNK/BRANCH/LEAF to deal with overlapping
        public readonly ConcurrentDictionary<Vector2Int, Color> colorsTrunk;
        public readonly ConcurrentDictionary<Vector2Int, Color> colorsBranch;
        public readonly ConcurrentDictionary<Vector2Int, Color> colorsLeaf;

        public int xMin, xMax;
        public int yMin, yMax;

        public int Width
        {
            get => xMax - xMin;
        }

        public int Height
        {
            get => yMax - yMin;
        }

        public Color this[Vector2Int position, ParticleType type]
        {
            get
            {
                return colorsTrunk[position];
            }
            set
            {
                if (position.x > xMax)
                {
                    xMax = position.x;
                }
                else if (position.x < xMin)
                {
                    xMin = position.x;
                }
                if (position.y > yMax)
                {
                    yMax = position.y;
                }
                else if (position.y < yMin)
                {
                    yMin = position.y;
                }

                switch (type)
                {
                    case ParticleType.Trunk:
                        colorsTrunk[position] = value;
                        break;
                    case ParticleType.Branch:
                        colorsBranch[position] = value;
                        break;
                    case ParticleType.Leaf:
                        colorsLeaf[position] = value;
                        break;
                }
            }
        }

        public ColorMap()
        {
            colorsTrunk = new ConcurrentDictionary<Vector2Int, Color>();
            colorsBranch = new ConcurrentDictionary<Vector2Int, Color>();
            colorsLeaf = new ConcurrentDictionary<Vector2Int, Color>();
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    float RandomRange(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    float RandomValue()
    {
        return (float)random.NextDouble();
    }

    #region Initialization
    void Awake()
    {
        Application.runInBackground = true;
    }

    void Start()
    {
        transform.position += new Vector3(0f, 0f, UnityEngine.Random.value);
        StartGrowth(growInstantly ? 0 : 1);
    }

    public void StartGrowth(int steps)
    {
        if (settings.seed == 0)
        {
            //UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            seed = UnityEngine.Random.Range(-1000000, 1000000);
        }
        else
        {
            seed = settings.seed;
        }

        random = new Random(seed);

        Setup();
        Grow();
        DoGrowth(steps);
    }

    public void DoGrowth(int steps)
    {
        if (steps < 1)
        {
            while (particles.Count > 0 || cells.Count > 0)
            {
                Grow();
            }

            fullyGrown = true;
        }
        else
        {
            int i = 0;
            while (i < steps && (particles.Count > 0 || cells.Count > 0))
            {
                Grow();
                i++;
            }

            if (particles.Count == 0 && cells.Count == 0)
            {
                fullyGrown = true;
            }
        }

        Draw();
    }

    void Setup()
    {
        particles.Clear();
        cells.Clear();
        colorMap = new ColorMap();

        uint seed = RandomHash();

        // Trunk
        Particle particle = new Particle()
        {
            seed = seed,
            position = Vector2.zero,
            drawnX = 0,
            drawnY = 0,
            initialAngle = (0.5f + RandomRange(-settings.maxTrunkCurveStart, settings.maxTrunkCurveStart)) * Mathf.PI,
            targetAngle = (0.5f + RandomRange(-settings.maxTrunkCurveEnd, settings.maxTrunkCurveEnd)) * Mathf.PI,
            speed = RandomRange(settings.trunkSettings.minSpeed, settings.trunkSettings.maxSpeed),
            type = ParticleType.Trunk,
            lifetime = random.Next(settings.trunkSettings.minLifetime, settings.trunkSettings.maxLifetime),
            startColor = GetRandomParticleColor(settings.trunkSettings, seed, out Color endColor),
            endColor = endColor,
            canBranch = true
        };

        particle.direction = AngleToDirection(particle.initialAngle);

        particles.Add(particle);
    }
    #endregion

    public void ExportTexture()
    {
        string path = Application.dataPath + "/ExportedTrees";
        System.IO.Directory.CreateDirectory(path);
        System.IO.File.WriteAllBytes(path + "/" + settings.name + " " + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".png", texture.EncodeToPNG());
    }

    // Draw data to texture
    void Draw()
    {
        int width = colorMap.xMax - colorMap.xMin + 1;
        int height = colorMap.yMax - colorMap.yMin + 1;

        if (!texture)
        {
            texture = new Texture2D(width, height)
            {
                filterMode = FilterMode.Point
            };
        }
        else
        {
            texture.Resize(width, height);
        }

        Color[] pixels = new Color[width * height];
        System.Threading.Tasks.Parallel.ForEach(colorMap.colorsTrunk, cell =>
        {
            pixels[width * (cell.Key.y - colorMap.yMin) + (cell.Key.x - colorMap.xMin)] = cell.Value;
        });

        System.Threading.Tasks.Parallel.ForEach(colorMap.colorsBranch, cell =>
        {
            pixels[width * (cell.Key.y - colorMap.yMin) + (cell.Key.x - colorMap.xMin)] = cell.Value;
        });

        System.Threading.Tasks.Parallel.ForEach(colorMap.colorsLeaf, cell =>
        {
            pixels[width * (cell.Key.y - colorMap.yMin) + (cell.Key.x - colorMap.xMin)] = cell.Value;
        });

        texture.SetPixels(pixels);
        texture.Apply();

        if (sprite)
        {
            Destroy(sprite);
        }

        sprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(-colorMap.xMin / (float)width, -colorMap.yMin / (float)height), 100f, 1u, SpriteMeshType.FullRect);

        renderer.sprite = sprite;
    }

    #region Growth
    void Grow()
    {
        growIterations++;

        // Simulate cells in parallel
        System.Threading.Tasks.Parallel.For(0, cells.Count, SimulateCell);

        // Remove expired cells
        List<int> sortedParticlesToRemove = particlesToRemove.ToList();
        sortedParticlesToRemove.Sort();
        for (int i = sortedParticlesToRemove.Count - 1; i >= 0; i--)
        {
            cells.RemoveAt(sortedParticlesToRemove[i]);
        }
        particlesToRemove = new ConcurrentBag<int>();

        // Simulate particles
        for (int i = 0; i < particles.Count; i++)
        {
            Particle particle = particles[i];
            particle = particle.type switch
            {
                ParticleType.Trunk => SimulateTrunkParticle(particle),
                ParticleType.Branch => SimulateBranchParticle(particle),
                ParticleType.Leaf => SimulateLeafParticle(particle)
            };

            if (particle.age > particle.lifetime)
            {
                particles.RemoveAt(i);
                i--;
            }
            else
            {
                particles[i] = particle;
            }
        }
    }

    void SimulateCell(int i)
    {
        Particle cell = cells[i];
        cell = cell.type switch
        {
            ParticleType.Trunk => SimulateTrunkCell(cell, i),
            ParticleType.Branch => SimulateBranchCell(cell, i),
            ParticleType.Leaf => SimulateLeafCell(cell, i)
        };

        colorMap[new Vector2Int(cell.drawnX, cell.drawnY), cell.type] = cell.startColor;

        if (cell.age > cell.lifetime)
        {
            particlesToRemove.Add(i);
        }
        else
        {
            cells[i] = cell;
        }
    }

    #region Trunk
    float CreateTrunk(Particle trunk)
    {
        float trunkAngle = Mathf.LerpUnclamped(trunk.initialAngle, trunk.targetAngle, (float)(trunk.age - trunk.splitAge) / trunk.lifetime);

        // Trunk
        Particle particle = new Particle()
        {
            seed = trunk.seed,
            position = trunk.position,
            drawnX = 0,
            drawnY = 0,
            initialAngle = trunkAngle,
            targetAngle = RandomRange(trunkAngle - Mathf.PI / 2, trunkAngle + Mathf.PI / 2),
            speed = trunk.speed,
            type = ParticleType.Trunk,
            age = trunk.age,
            splitAge = trunk.age,
            lifetime = trunk.lifetime,
            startColor = trunk.startColor,
            endColor = trunk.endColor,
            canBranch = false
        };

        //particle.targetAngle = particle.initialAngle;

        particle.direction = AngleToDirection(particle.initialAngle);

        particles.Add(particle);

        return particle.targetAngle;
    }

    Particle SimulateTrunkParticle(Particle particle)
    {
        Particle cell = particle;

        particle.position += particle.direction * particle.speed;
        float angle = Mathf.LerpUnclamped(particle.initialAngle, particle.targetAngle, (float)(particle.age - particle.splitAge) / particle.lifetime);
        particle.direction = AngleToDirection(angle);

        particle.drawnX = (int)particle.position.x;
        particle.drawnY = (int)particle.position.y;

        if (RandomValue() - settings.trunkBranchChanceCurve.Evaluate((float)particle.age / particle.lifetime) <= settings.trunkBranchChance)
        {
            CreateBranch(particle, settings.setLeafColorPerBranch ? RandomHash() : particle.seed);
        }

        // Add new cell if this particle's pixel position has changed
        if (true || particle.drawnX != cell.drawnX || particle.drawnY != cell.drawnY)
        {
            cell.age = 0;
            cell.lifetime = Mathf.RoundToInt(random.Next(settings.trunkSettings.minCellLifetime, settings.trunkSettings.maxCellLifetime)
                * settings.trunkAgeGrowthCurve.Evaluate((float)particle.age / particle.lifetime) * settings.trunkSettings.ageGrowthMultiplier);
            cell.direction = RotateDirection(cell.direction, -Mathf.PI / 2f);

            if (settings.trunkSettings.setColorByParticleLife)
            {
                cell.startColor = Color.Lerp(particle.startColor, particle.endColor, (float)particle.age / particle.lifetime);
            }

            cells.Add(cell);
            cell.direction = RotateDirection(cell.direction, Mathf.PI);
            cells.Add(cell);
        }

        particle.age++;

        if (particle.age > particle.lifetime)
        {
            if (settings.spawnLeafAtTopOfTrunk)
            {
                CreateLeaf(particle);
            }
            if (settings.spawnBranchAtTopOfTrunk)
            {
                CreateBranch(particle, settings.setLeafColorPerBranch ? RandomHash() : particle.seed);
            }
        }
        else
        {
            // Split trunk
            if (particle.canBranch && RandomValue() <= settings.trunkSplitChance)
            {
                float newAngle = CreateTrunk(particle);
                particle.splitAge = particle.age;
                particle.initialAngle = angle;

                particle.targetAngle = angle + (angle - newAngle) + RandomRange(-Mathf.PI / 2, Mathf.PI / 2);

                particle.canBranch = false;
            }
        }

        return particle;
    }

    Particle SimulateTrunkCell(Particle cell, int i)
    {
        cell.position += cell.direction * cell.speed;

        cell.drawnX = (int)cell.position.x;
        cell.drawnY = (int)cell.position.y;

        if (!settings.trunkSettings.setColorByParticleLife)
        {
            cell.startColor = Color.Lerp(cell.startColor, cell.endColor, cell.lifetime == 0f ? 0f : (float)cell.age / cell.lifetime);
        }

        cell.startColor = JitterColor(cell.startColor, settings.trunkSettings, i);

        cell.age++;

        return cell;
    }
    #endregion

    #region Branch
    void CreateBranch(Particle trunk, uint seed, bool canBranch = true)
    {
        float random = RandomValue();
        float trunkAngle = DirectionToAngle(trunk.direction) + (random <= 0.5f ? RandomRange(Mathf.PI * settings.minBranchAngle, Mathf.PI * settings.maxBranchAngle) : RandomRange(-Mathf.PI * settings.maxBranchAngle, -Mathf.PI * settings.minBranchAngle));

        Particle branch = new Particle()
        {
            seed = seed,
            position = trunk.position,
            drawnX = trunk.drawnX,
            drawnY = trunk.drawnY,
            direction = AngleToDirection(trunkAngle),
            speed = RandomRange(settings.branchSettings.minSpeed, settings.branchSettings.maxSpeed),
            type = ParticleType.Branch,
            lifetime = this.random.Next(settings.branchSettings.minLifetime, settings.branchSettings.maxLifetime),
            startColor = GetRandomParticleColor(settings.branchSettings, seed, out Color endColor),
            endColor = endColor,
            canBranch = canBranch,

            initialAngle = trunkAngle,
            targetAngle = Mathf.LerpUnclamped(trunk.initialAngle, trunk.targetAngle, (float)(trunk.age - trunk.splitAge) / trunk.lifetime) + ((trunkAngle < Mathf.PI - trunkAngle ? -1f : 1f) * (RandomRange(settings.minTargetBranchAngle, settings.maxTargetBranchAngle) * Mathf.PI + settings.branchAngleJitterCurve.Evaluate(RandomValue())))
        };

        particles.Add(branch);
    }

    Particle SimulateBranchParticle(Particle particle)
    {
        Particle cell = particle;

        particle.position += particle.direction * particle.speed;

        float angle = settings.curveBranches ? Mathf.LerpUnclamped(particle.initialAngle, particle.targetAngle, settings.branchTargetAngleCurve.Evaluate((float)particle.age / particle.lifetime)) : particle.initialAngle;
        particle.direction = AngleToDirection(angle);

        particle.drawnX = (int)particle.position.x;
        particle.drawnY = (int)particle.position.y;

        if (settings.branchLeafChanceCurve.Evaluate((float)particle.age / particle.lifetime) * RandomValue() <= settings.branchLeafChance)
        {
            CreateLeaf(particle);
        }

        if (particle.canBranch && RandomValue() < settings.branchBranchChance)
        {
            CreateBranch(particle, particle.seed, false);
        }

        // Add new cell if this particle's pixel position has changed
        if (true || particle.drawnX != cell.drawnX || particle.drawnY != cell.drawnY)
        {
            cell.age = 0;
            cell.lifetime = Mathf.RoundToInt(random.Next(settings.branchSettings.minCellLifetime, settings.branchSettings.maxCellLifetime)
                * settings.trunkAgeGrowthCurve.Evaluate((float)particle.age / particle.lifetime) * settings.branchSettings.ageGrowthMultiplier);
            cell.direction = RotateDirection(cell.direction, -Mathf.PI / 2f);

            if (settings.branchSettings.setColorByParticleLife)
            {
                cell.startColor = Color.Lerp(cell.startColor, cell.endColor, (float)particle.age / particle.lifetime);
            }

            cells.Add(cell);
            cell.direction = RotateDirection(cell.direction, Mathf.PI);
            cells.Add(cell);
        }

        particle.age++;

        return particle;
    }

    Particle SimulateBranchCell(Particle cell, int i)
    {
        cell.position += cell.direction * cell.speed;

        cell.drawnX = (int)cell.position.x;
        cell.drawnY = (int)cell.position.y;

        if (!settings.branchSettings.setColorByParticleLife)
        {
            cell.startColor = Color.Lerp(cell.startColor, cell.endColor, cell.lifetime == 0f ? 0f : (float)cell.age / cell.lifetime);
        }

        cell.startColor = JitterColor(cell.startColor, settings.branchSettings, i);

        cell.age++;

        return cell;
    }
    #endregion

    #region Leaf
    void CreateLeaf(Particle branch)
    {
        Particle leaf = new Particle()
        {
            seed = branch.seed,
            position = branch.position,
            drawnX = branch.drawnX,
            drawnY = branch.drawnY,
            direction = AngleToDirection(Mathf.Lerp(RandomValue() * Mathf.PI * 2f, settings.preferredLeafAngle * Mathf.PI, settings.preferredLeafAngleCurve.Evaluate(RandomValue()))),
            speed = RandomRange(settings.leafSettings.minSpeed, settings.leafSettings.maxSpeed),
            type = ParticleType.Leaf,
            lifetime = random.Next(settings.leafSettings.minLifetime, settings.leafSettings.maxLifetime),
            //startColor = GetRandomParticleColor(RandomValue() <= settings.flowerChance ? settings.flowerSettings : settings.leafSettings, branch.seed, out Color endColor),
            startColor = GetRandomParticleColor(RandomValue() <= settings.flowerChance ? settings.flowerSettings : settings.leafSettings, branch.seed, out Color endColor),
            endColor = endColor
        };

        particles.Add(leaf);
    }

    Particle SimulateLeafParticle(Particle particle)
    {
        Particle cell = particle;

        particle.position += particle.direction * particle.speed;

        particle.drawnX = (int)particle.position.x;
        particle.drawnY = (int)particle.position.y;

        // Add new cell if this particle's pixel position has changed
        if (true || particle.drawnX != cell.drawnX || particle.drawnY != cell.drawnY)
        {
            cell.age = 0;
            cell.lifetime = Mathf.RoundToInt(random.Next(settings.leafSettings.minCellLifetime, settings.leafSettings.maxCellLifetime)
                * settings.leafAgeGrowthCurve.Evaluate((float)particle.age / particle.lifetime) * settings.leafSettings.ageGrowthMultiplier);
            cell.direction = RotateDirection(cell.direction, -Mathf.PI / 2f);

            if (settings.leafSettings.setColorByParticleLife)
            {
                cell.startColor = Color.Lerp(cell.startColor, cell.endColor, (float)particle.age / particle.lifetime);
            }

            cells.Add(cell);
            cell.direction = RotateDirection(cell.direction, Mathf.PI);
            cells.Add(cell);
        }

        particle.age++;

        return particle;
    }

    Particle SimulateLeafCell(Particle cell, int i)
    {
        cell.position += cell.direction * cell.speed;

        cell.drawnX = (int)cell.position.x;
        cell.drawnY = (int)cell.position.y;

        if (!settings.leafSettings.setColorByParticleLife)
        {
            cell.startColor = Color.Lerp(cell.startColor, cell.endColor, cell.lifetime == 0f ? 0f : (float)cell.age / cell.lifetime);
        }

        cell.startColor = JitterColor(cell.startColor, settings.leafSettings, i);

        cell.age++;

        return cell;
    }
    #endregion
    #endregion

    #region HelperFunctions
    Color GetRandomParticleColor(TreeSettings.ParticleSettings settings, uint seed, out Color endColor)
    {
        int length = settings.startColors.Length;

#if UNITY_EDITOR
        if (settings.startColors == null || length == 0 || settings.startColors.Length != settings.endColors.Length)
        {
            endColor = Color.black;
            return Color.black;
        }
#endif

        if (length == 1)
        {
            endColor = settings.endColors[0];
            return settings.startColors[0];
        }

        int i = Mathf.RoundToInt(Mathf.Lerp(0, length - 1, NormalizeHash(seed)));
        if (settings.blendColors)
        {
            seed = HashUint(seed);
            endColor = Color.Lerp(settings.endColors[i], settings.endColors[(i + 1) % length], NormalizeHash(seed));

            seed = HashUint(seed);
            if (settings.blendSeparately)
            {
                i = Mathf.RoundToInt(Mathf.Lerp(0, length - 1, NormalizeHash(seed)));
            }
            seed = HashUint(seed);
            return Color.Lerp(settings.startColors[i], settings.startColors[(i + 1) % length], NormalizeHash(seed));
        }
        else
        {
            endColor = settings.endColors[i];
            return settings.startColors[i];
        }
    }

    static Vector2 RotateDirection(Vector2 direction, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        return new Vector2((cos * direction.x) - (sin * direction.y), (sin * direction.x) + (cos * direction.y));
    }

    static Vector2 AngleToDirection(float theta)
    {
        return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
    }

    static float DirectionToAngle(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x);
    }

    static bool IsAngleLeft(float theta, float axis = 0f)
    {
        theta %= Mathf.PI * 2f;
        return theta > (Mathf.PI * 0.5 + axis) % Mathf.PI * 2f && theta < (Mathf.PI * 1.5f + axis) % Mathf.PI * 2f;
    }

    Vector2 RandomBranchDirection()
    {
        return AngleToDirection(((RandomValue() * 0.9f + 0.05f) * Mathf.PI));
    }

    Color JitterColor(Color color, TreeSettings.ParticleSettings settings, int i)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        uint rand = (uint)(seed * growIterations + i);
        //h += RandomRange(-settings.colorJitter.x, settings.colorJitter.x);
        //s += RandomRange(-settings.colorJitter.y, settings.colorJitter.y);
        //v += RandomRange(-settings.colorJitter.z, settings.colorJitter.z);
        h += NormalizeHash(HashUint(ref rand)) * settings.colorJitter.x * 2f - settings.colorJitter.x;
        s += NormalizeHash(HashUint(ref rand)) * settings.colorJitter.y * 2f - settings.colorJitter.y;
        v += NormalizeHash(HashUint(ref rand)) * settings.colorJitter.z * 2f - settings.colorJitter.z;

        return Color.HSVToRGB(h, s, v);
    }
    #endregion

    #region Random
    uint HashUint(uint state)
    {
        state ^= 2747636419u;
        state *= 2654435769u;
        state ^= state >> 16;
        state *= 2654435769u;
        state ^= state >> 16;
        state *= 2654435769u;
        return state;
    }

    uint HashUint(ref uint state)
    {
        state ^= 2747636419u;
        state *= 2654435769u;
        state ^= state >> 16;
        state *= 2654435769u;
        state ^= state >> 16;
        state *= 2654435769u;
        return state;
    }

    float NormalizeHash(uint num)
    {
        return num / 4294967295f;
    }

    uint RandomHash()
    {
        return (uint)(random.NextDouble() * uint.MaxValue);
    }
    #endregion

    #region Particle
    public struct Particle
    {
        public uint seed;

        public Vector2 position;
        public int drawnX;
        public int drawnY;

        public Vector2 direction;
        public float initialAngle;
        public float targetAngle;
        public float speed;

        public ParticleType type;

        public int age;
        public int splitAge;
        public int lifetime;

        public Color currentColor;
        public Color startColor;
        public Color endColor;

        public bool canBranch;
    }

    public enum ParticleType
    {
        Trunk, Branch, Leaf
    }
    #endregion

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!sprite)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawCube(transform.position + new Vector3(0f, 0.5f, 0f), new Vector3(0.1f, 1f, 0f));
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Tree)), CanEditMultipleObjects()]
public class TreeEditor : Editor
{
    Editor settingsEditor;
    bool settingsFoldout = true;

    public override void OnInspectorGUI()
    {
        Tree instance = (Tree)target;

        if (Application.isPlaying)
        {
            GUILayout.BeginHorizontal();

            //if (GUILayout.Button(instance.fullyGrown ? "FULLY GROWN" : "Grow"))
            //{
            //    if (!instance.fullyGrown)
            //    {
            //        instance.DoGrowth(instance.steps);
            //    }
            //}

            //if (GUILayout.RepeatButton(instance.fullyGrown ? "FULLY GROWN" : "Grow (HOLD)"))
            //{
            //    var selectedObjects = Selection.objects;
            //    for (int i = 0; i < selectedObjects.Length; i++)
            //    {
            //        if (selectedObjects[i] is GameObject go && go.TryGetComponent(out Tree tree))
            //        {
            //            if (!tree.fullyGrown)
            //            {
            //                tree.DoGrowth(tree.steps);
            //            }
            //        }
            //    }
            //}

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Export Texture"))
            {
                if (instance.renderer.sprite)
                {
                    instance.ExportTexture();
                }
            } 
        }

        DrawDefaultInspector();

        if (instance.settings != null)
        {
            DrawSettingsEditor(instance.settings, ref settingsFoldout, ref settingsEditor);
            EditorPrefs.SetBool(nameof(settingsFoldout), settingsFoldout);
        }

        GUILayout.Space(10f);
    }

    void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
            }

        }
    }
}
#endif