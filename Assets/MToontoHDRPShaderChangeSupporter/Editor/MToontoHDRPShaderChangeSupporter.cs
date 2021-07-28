using UnityEngine;
using UnityEditor;
public class MToontoHDRPShaderChangeSupporter : EditorWindow
{
    [MenuItem("Window/MToontoHDRPShaderChangeSupporter")]

    static void Open()
    {
        var window = CreateInstance<MToontoHDRPShaderChangeSupporter>();
        window.Show();
        window.minSize = new Vector2(350, 80);
        window.maxSize = new Vector2(400, 100);
    }
    Shader HDRPLitShader;
    GameObject VRoidModel;
    private void OnGUI()
    {
        VRoidModel = EditorGUILayout.ObjectField("VRoidモデル", VRoidModel, typeof(GameObject), true) as GameObject;
        HDRPLitShader = (Shader)EditorGUILayout.ObjectField("Package内のLitシェーダー", HDRPLitShader, typeof(Shader), true) as Shader;
        if (GUILayout.Button("変更"))
        {
            HDRPLit = HDRPLitShader;
            Convert(VRoidModel);
        }
    }
    Shader HDRPLit;
    Texture NormalMap;
    Color EmissiveColor;
    Texture EmissiveColorMap;
    SkinnedMeshRenderer face;
    SkinnedMeshRenderer Body;
    SkinnedMeshRenderer Hair;
    
    public void Convert(GameObject VRoidModel)
    {
        face = VRoidModel.transform.Find("Face").GetComponent<SkinnedMeshRenderer>();
        Body = VRoidModel.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        Hair = VRoidModel.transform.Find("Hairs").transform.Find("Hair001").GetComponent<SkinnedMeshRenderer>();
        Change(face);
        Change(Body);
        Change(Hair);
        VRoidModel.SetActive(true);
    }

    public void Change(SkinnedMeshRenderer SMR)
    {


        var m = SMR.sharedMaterials;
        ForBun(SMR, m);
        SMR.materials  = m;
        SMR.UpdateGIMaterials();
        
    }

    public void ForBun(SkinnedMeshRenderer SMR,Material[] m)
    {
        for (int i = 0; i < SMR.materials.Length; i++)
        {
            Debug.Log("smr");
           

            if (m[i].shader == HDRPLit)
            {
                continue;
            }
            var b = false;
            var c = false;


            if (m[i].GetInt("_BlendMode") == 2)
            {
                b = true;
            }
            if (m[i].GetTag("RenderType", true) != "Opaque") b = true;


            if (m[i].GetFloat("_CullMode") == 0)
            {
                c = true;
            }

            var Basecolor = m[i].GetColor("_Color");
            var BaseMap = m[i].GetTexture("_MainTex");
            var AlphaClipping = false;
            if (m[i].GetInt("_BlendMode") != 0)
            {
                AlphaClipping = true;
            }
            if (NormalMap)
            {
                NormalMap = null;
            }
            if (EmissiveColorMap)
            {
                EmissiveColorMap = null;
                EmissiveColor = new Color(1, 1, 1, 1);
            }
            if (m[i].GetTexture("_BumpMap"))
            {
                NormalMap = m[i].GetTexture("_BumpMap");
            }
            if (m[i].GetTexture("_EmissionMap"))
            {
                
                EmissiveColorMap = m[i].GetTexture("_EmissionMap");
                EmissiveColor = m[i].GetColor("_EmissionColor");
            }

            m[i].shader = HDRPLit;
         
           
            m[i].EnableKeyword("_");

            //m[i].EnableKeyword("LIGHTMAP_ON");
            //m[i].EnableKeyword("_DEPTHOFFSET_ON");
            m[i].EnableKeyword("_NORMALMAP");
            m[i].EnableKeyword("_EMISSIVE_COLOR_MAP");

            m[i].SetTexture("_BaseColorMap", BaseMap);
            m[i].SetColor("_BaseColor", Basecolor);
            m[i].SetTexture("_NormalMap", NormalMap);
            m[i].SetTexture("_EmissiveColorMap", EmissiveColorMap);
            m[i].SetColor("_EmissiveColor", EmissiveColor);
            if (SMR == Hair)
            {
                b = false;
            }
            if (b)
            {
                m[i].EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m[i].EnableKeyword("_ALPHATEST_ON");
                m[i].EnableKeyword("_BLENDMODE_ALPHA");
                m[i].SetFloat("_SurfaceType", 1.0f);
                m[i].SetFloat("_BlendMode", 0.0f);
                m[i].SetFloat("_AlphaCutoffEnable", 1.0f);
            }

            if (c)
            {
                m[i].EnableKeyword("_DOUBLESIDED_ON");
                m[i].SetFloat("_DoubleSidedEnable", 1.0f);
            }
            
            if (AlphaClipping)
            {
                m[i].EnableKeyword("_ALPHATEST_ON");
                m[i].SetFloat("_AlphaCutoffEnable", 1.0f);
            }
            AlphaClipping = false;
            b = false;
            
            
        }
        
    }
}

