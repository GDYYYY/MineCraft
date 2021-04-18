using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor editor;
    private MaterialProperty[] properties;
    private Material target;

    enum SpecularChoice
    {
        True, False
    }

    enum ShaderType
    {
        NORMAL_ONLY, BLINN_PHONG
    }
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        this.target=editor.target as Material;

        ShaderType shaderType = ShaderType.BLINN_PHONG;
        if (target.IsKeywordEnabled("NORMAL_ONLY"))
            shaderType = ShaderType.NORMAL_ONLY;
        EditorGUI.BeginChangeCheck();
        shaderType = (ShaderType)EditorGUILayout.EnumPopup(
            new GUIContent("Shade Type"), shaderType);
        if (EditorGUI.EndChangeCheck())
        {
            if (shaderType == ShaderType.NORMAL_ONLY)
            {
                target.EnableKeyword("NORMAL_ONLY");
            }
            else
            {
                target.DisableKeyword("NORMAL_ONLY");
            }
        }

        if(shaderType==ShaderType.BLINN_PHONG)
        {
            MaterialProperty mainTex = FindProperty("_MainTex", properties);
            GUIContent mainTexLabel = new GUIContent(mainTex.displayName);
            editor.TextureProperty(mainTex, mainTexLabel.text);

            MaterialProperty diffuseColor = FindProperty("_DiffuseColor", properties);
            GUIContent diffuseLabel = new GUIContent(diffuseColor.displayName);
            editor.ColorProperty(diffuseColor, diffuseLabel.text);

            SpecularChoice specularChoice = SpecularChoice.False;
            if (target.IsKeywordEnabled("USE_SPECULAR"))
                specularChoice = SpecularChoice.True; //修改选项显示

            EditorGUI.BeginChangeCheck();
            specularChoice = (SpecularChoice) EditorGUILayout.EnumPopup(
                new GUIContent("Use Specular?"), specularChoice);

            if (EditorGUI.EndChangeCheck())
            {
                if (specularChoice == SpecularChoice.True)
                {
                    target.EnableKeyword("USE_SPECULAR");
                }
                else
                {
                    target.DisableKeyword("USE_SPECULAR");
                }

            }

            if (specularChoice == SpecularChoice.True)
            {
                MaterialProperty shininess = FindProperty("_Shininess", properties);
                GUIContent shininessLabel = new GUIContent(shininess.displayName);
                editor.FloatProperty(shininess, "   Specular Factor");
                MaterialProperty specularColor = FindProperty("_SpecularColor", properties);
                GUIContent specularLabel = new GUIContent(specularColor.displayName);
                editor.ColorProperty(specularColor, "   "+specularLabel.text);
            }
        }
    }
    
}
