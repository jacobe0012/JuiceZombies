using System;
using System.Text.RegularExpressions;
using Main;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Stateful;
using UnityEditor;
using UnityEngine;
using Material = UnityEngine.Material;


public class NormalPic2Prefab
{
    [Title("贴图输入路径:")] [FolderPath] public string inputPath = "Assets/ApesGang/Prefabs/Map/MapTexturesNew";

    [Title("预制件输出路径:")] [FolderPath] public string outputPath = "Assets/ApesGang/Prefabs/Map/MapTexturesNew/Prefabs";

    //[Title("是否对根据有效像素归一化预制件尺寸:")] public bool isNormalizePrefab = true;
    [Title("转为普通预制件:")] public bool normal = false;

    [Title("转为特效类技能预制件:")] public bool toVFXSkill = false;
    [Title("转为特效类弹幕预制件:")] public bool toVFXBullet = false;
    [Title("转为地形序列帧预制件:")] public bool toArea = false;
    //[Title("转为地图预制件:")] public bool toMap = false;

    private string meshPath = "Assets/ApesGang/Prefabs/CommonMesh";

    [Button(name: "一键转换")]
    public void Button()
    {
        // if (outputPath == "")
        // {
        //     outputPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(skeletonDataAsset))
        //         .Replace('\\', '/') + "/Baked";
        //     System.IO.Directory.CreateDirectory(outputPath);
        // }
        // else
        // {
        // }

        //System.IO.Directory.CreateDirectory(outputPath);

        string[] imagePaths = AssetDatabase.FindAssets("t:texture2D", new[] { inputPath });
        EditorUtility.DisplayProgressBar("Replacing...", "Start replace", 0);
        int index = 0;
        foreach (var path in imagePaths)
        {
            index++;
            EditorUtility.DisplayProgressBar("Replacing...", "Start replace", index / imagePaths.Length);

            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            Debug.Log($"{texture.name}:{texture.width}x{texture.height}");
            //SaveRotatedImg(assetPath, texture, 90);
            var sc = PackTexture.GetPrefabScale(texture);
            var name = texture.name.Replace("(", "").Replace(")", "");
            var newgo = new GameObject(name);
            var meshFilter = newgo.AddComponent<MeshFilter>();
            var meshRenderer = newgo.AddComponent<MeshRenderer>();
            //meshFilter.mesh =
            var newmeshDic = $"{outputPath}/Mesh";

            var mapmeshPath = $"{meshPath}/mesh_{texture.width}{texture.height}.mesh";

            Mesh mesh;

            mapmeshPath = $"{meshPath}/mesh_001001.mesh";
            mesh = AssetDatabase.LoadAssetAtPath<Mesh>(mapmeshPath);

            // if (toVFXSkill || toVFXBullet)
            // {
            //     mapmeshPath = $"{meshPath}/mesh_001001.mesh";
            //     mesh = AssetDatabase.LoadAssetAtPath<Mesh>(mapmeshPath);
            //     //Debug.LogError($"toVFX{mapmeshPath}");
            // }
            // else
            // {
            //     mesh = AssetDatabase.LoadAssetAtPath<Mesh>(mapmeshPath);
            // }

            if (mesh == null)
            {
                Debug.LogError($"mesh == null {texture.name}  {mapmeshPath}");

                var newMeshname = $"{newmeshDic}/mesh_{texture.width}{texture.height}.mesh";
                GenerateRectangleMesh(texture.width, texture.height, newMeshname);

                mesh = AssetDatabase.LoadMainAssetAtPath(newMeshname) as Mesh;
                //mesh = AssetDatabase.LoadAssetAtPath<Mesh>(newMeshname);
                //GameObject.DestroyImmediate(newgo);
                //continue;
            }

            //Debug.LogError($"{mesh.name}");
            meshFilter.mesh = mesh;
            Material mat = new Material(Shader.Find("JiYuStudio/JiYuDefaultShaderTest"));
            if (toVFXSkill || toVFXBullet || toArea)
            {
                var values = Get_rcOr_wh(name);
                mat = new Material(Shader.Find("JiYuStudio/JiYuGPUFrameAnimUnlitTest"));
                mat.SetInt("_FrameRow", (int)values.Item1);
                mat.SetInt("_FrameCol", (int)values.Item2);
                mat.SetInt("_JiYuFrameAnimLoop", 2);
            }

            mat.SetTexture("_MainTex", texture);
            var matPath = $"{outputPath}/Mat";
            System.IO.Directory.CreateDirectory(matPath);
            var prefabName = name;
            prefabName = Regex.Replace(prefabName, @"_rc(\d+)x(\d+)", "");
            prefabName = Regex.Replace(prefabName, @"_wh(\d+)x(\d+)", "");
            var matPath0 = $"{matPath}/{prefabName}_mat.mat";

            AssetDatabase.CreateAsset(mat, matPath0);
            var mat0 = AssetDatabase.LoadMainAssetAtPath(matPath0) as Material;
            meshRenderer.sharedMaterial = mat0;
            var jiYuSortAuthoring = newgo.AddComponent<JiYuSortAuthoring>();
            var jiYuPivotAuthoring = newgo.AddComponent<JiYuPivotAuthoring>();
            jiYuPivotAuthoring.PivotY = 0.5f;
            if (toVFXSkill || toVFXBullet || toArea)
            {
                jiYuSortAuthoring.layer = JiYuLayer.UpLayer0;
                jiYuSortAuthoring.sortIndex = 1;

                newgo.AddComponent<JiYuFrameAnimLoopAuthoring>();
                newgo.AddComponent<JiYuFrameAnimSpeedAuthoring>();
                newgo.AddComponent<JiYuStartTimeAuthoring>();
                newgo.AddComponent<SpecialEffectDataAuthoring>();
                var wh = Get_rcOr_wh(name, true);
                //float max = Mathf.Max(wh.Item1, wh.Item2);
                newgo.transform.localScale = new Vector3(wh.Item1, wh.Item2, 1f);
                if (toVFXBullet)
                {
                    newgo.AddComponent<StatefulTriggerEventAuthoring>();
                    newgo.AddComponent<BulletTagAuthoring>();
                    var shapeAuthoring = newgo.AddComponent<PhysicsShapeAuthoring>();
                    shapeAuthoring.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
                    shapeAuthoring.SetSphere(new SphereGeometry
                    {
                        Center = default,
                        Radius = 0.5f
                    }, quaternion.identity);
                    shapeAuthoring.BelongsTo = new PhysicsCategoryTags
                    {
                        Value = 64
                    };
                    shapeAuthoring.CollidesWith = PhysicsCategoryTags.Everything;

                    var bodyAuthoring = newgo.AddComponent<PhysicsBodyAuthoring>();
                    bodyAuthoring.MotionType = BodyMotionType.Kinematic;
                    bodyAuthoring.Smoothing = BodySmoothing.Interpolation;
                }

                if (toArea)
                {
                    jiYuSortAuthoring.layer = JiYuLayer.Area;
                    jiYuSortAuthoring.sortIndex = 1;
                    newgo.AddComponent<StatefulTriggerEventAuthoring>();
                    newgo.AddComponent<AreaTagAuthoring>();
                    newgo.AddComponent<MapElementDataAuthoring>();

                    var shapeAuthoring = newgo.AddComponent<PhysicsShapeAuthoring>();
                    shapeAuthoring.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
                    shapeAuthoring.SetSphere(new SphereGeometry
                    {
                        Center = default,
                        Radius = 0.5f
                    }, quaternion.identity);
                    shapeAuthoring.BelongsTo = PhysicsCategoryTags.Everything;
                    shapeAuthoring.CollidesWith = PhysicsCategoryTags.Everything;

                    var bodyAuthoring = newgo.AddComponent<PhysicsBodyAuthoring>();
                    bodyAuthoring.MotionType = BodyMotionType.Kinematic;
                    bodyAuthoring.Smoothing = BodySmoothing.Interpolation;
                }
            }
            else if (normal)
            {
                newgo.transform.localScale = new Vector3(sc.x, sc.y, 1f);
                jiYuSortAuthoring.layer = JiYuLayer.Main;
                // if (name.Contains("up") || name.Contains("down") || name.Contains("left") || name.Contains("right"))
                // {
                //     jiYuSortAuthoring.layer = JiYuLayer.Map;
                //     var physicsShape = newgo.AddComponent<PhysicsShapeAuthoring>();
                //     physicsShape.SetBox(new BoxGeometry
                //     {
                //         Center = default,
                //         Orientation = default,
                //         Size = new float3(texture.width / 100f, texture.height / 100f, 1),
                //         BevelRadius = 0.05f
                //     });
                //     physicsShape.Restitution = new PhysicsMaterialCoefficient
                //     {
                //         Value = 0.3f,
                //         CombineMode = Unity.Physics.Material.CombinePolicy.ArithmeticMean
                //     };
                //     physicsShape.Friction = new PhysicsMaterialCoefficient
                //     {
                //         Value = 0,
                //         CombineMode = Unity.Physics.Material.CombinePolicy.GeometricMean
                //     };
                //     //TODO:
                //     // physicsShape.BelongsTo = new PhysicsCategoryTags
                //     // {
                //     //     Value = 0
                //     // };
                //     var elementAuthoring = newgo.AddComponent<MapElementDataAuthoring>();
                //     //elementAuthoring.elementID = 2001;
                //     newgo.AddComponent<BoardDataAuthoring>();
                //     var physicsBodyAuthoring = newgo.AddComponent<PhysicsBodyAuthoring>();
                //     physicsBodyAuthoring.MotionType = BodyMotionType.Static;
                // }
                // else if (name.Contains("map"))
                // {
                //     jiYuSortAuthoring.layer = JiYuLayer.Map;
                //     newgo.AddComponent<MapBaseDataAuthoring>();
                // }
                // else if (name.Contains("bullet"))
                // {
                //     jiYuSortAuthoring.layer = JiYuLayer.UpLayer0;
                //     //
                //     //
                //     //     string newTextureName = $"model_{texture.name}";
                //     //     //string newAssetPath = assetPath.Replace(texture.name, newTextureName);
                //     //     AssetDatabase.RenameAsset(assetPath, newTextureName);
                //     //     //AssetDatabase.SaveAssets();
                // }
                // else
                // {
                //     jiYuSortAuthoring.layer = JiYuLayer.Main;
                //
                //     // string newTextureName = $"monster_weapon_{texture.name}_name";
                //     // //string newAssetPath = assetPath.Replace(texture.name, newTextureName);
                //     // AssetDatabase.RenameAsset(assetPath, newTextureName);
                // }
            }


            //name = $"{name}_{(float)texture.width / values.Item2}x{(float)texture.height / values.Item1}";
            PrefabUtility.SaveAsPrefabAsset(newgo, $"{outputPath}/{prefabName}.prefab");

            GameObject.DestroyImmediate(newgo);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        //EditorGUIUtility.PingObject(obj);
    }

    static (float, float) Get_rcOr_wh(string input, bool is_wh = false)
    {
        // 正则表达式，用于匹配 '_(\d+)x(\d+)$' 的模式
        string pattern = @"_rc(\d+)x(\d+)";
        if (is_wh)
        {
            pattern = @"_sc(\d+\.\d+)x(\d+\.\d+)";
        }

        Match match = Regex.Match(input, pattern);

        if (match.Success)
        {
            // 解析值
            float value1 = float.Parse(match.Groups[1].Value);
            float value2 = float.Parse(match.Groups[2].Value);

            return (value1, value2);
        }
        else
        {
            //throw new FormatException("Input string does not contain the expected format.");
            Debug.LogError($"没有找到尺寸后缀rc_ 使用默认值1");
        }

        return (1, 1);
    }

    public Mesh GenerateRectangleMesh(float width, float height, string savepath)
    {
        width /= 100f;
        height /= 100f;
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(width, 0, 0);
        vertices[1] = new Vector3(0, 0, 0);
        vertices[2] = new Vector3(0, height, 0);
        vertices[3] = new Vector3(width, height, 0);

        mesh.vertices = vertices;

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.triangles = triangles;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(1, 0);
        uv[1] = new Vector2(0, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;
        //mesh.bounds.center = 0;
        // 为了保证面正面朝向外部，可以添加以下代码
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


        // Save the mesh as an asset
        //string path = savepath;
        AssetDatabase.CreateAsset(mesh, savepath);
        AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
        return mesh;
    }

    public void SaveRotatedImg(string savePath, Texture2D texture, float angle)
    {
        if (texture != null)
        {
            // Get the pixel data
            Color[] pixels = texture.GetPixels();
            int width = texture.width;
            int height = texture.height;
            //float angle = 90f; // Specify the rotation angle in degrees

            // Calculate new width and height after rotation
            int newWidth = height;
            int newHeight = width;

            // Perform clockwise rotation
            Color[] rotatedPixels = new Color[newWidth * newHeight];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    float newX = Mathf.Cos(Mathf.Deg2Rad * angle) * x + Mathf.Sin(Mathf.Deg2Rad * angle) * y;
                    float newY = -Mathf.Sin(Mathf.Deg2Rad * angle) * x + Mathf.Cos(Mathf.Deg2Rad * angle) * y;

                    int newPixelX = Mathf.RoundToInt(newX);
                    int newPixelY = Mathf.RoundToInt(newY);

                    if (newPixelX >= 0 && newPixelX < newWidth && newPixelY >= 0 && newPixelY < newHeight)
                    {
                        int newIndex = newPixelY * newWidth + newPixelX;
                        rotatedPixels[newIndex] = pixels[index];
                    }
                }
            }

            // Apply rotated pixels
            Texture2D rotatedTexture = new Texture2D(newWidth, newHeight);
            rotatedTexture.SetPixels(rotatedPixels);
            rotatedTexture.Apply();

            // Save the rotated texture
            //string savePath = "Assets/RotatedTexture.png"; // Set the save path
            byte[] bytes = rotatedTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(savePath, bytes);

            // Refresh the Asset Database to see changes in Unity Editor
            //AssetDatabase.Refresh();
        }
    }
}