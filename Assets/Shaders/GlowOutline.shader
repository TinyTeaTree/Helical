Shader "Custom/GlowOutline"
{
    Properties
    {
        _GlowColor ("Glow Color", Color) = (1, 0, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 3
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        [Toggle] _UseViewDirection ("Use View Direction", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
        }
        
        LOD 100
        
        // Render back faces only, cull front faces
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        // Blend One One // Alternative: additive blending for stronger glow
        
        Pass
        {
            Name "GlowOutline"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float fresnel : TEXCOORD2;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _GlowColor;
                float _GlowIntensity;
                float _OutlineWidth;
                float _UseViewDirection;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Transform normal to world space
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                float3 normalWS = normalInput.normalWS;
                
                // Get view direction in world space
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 viewDirWS = GetWorldSpaceViewDir(positionWS);
                viewDirWS = normalize(viewDirWS);
                
                // Calculate expansion direction
                float3 expandDir;
                if (_UseViewDirection > 0.5)
                {
                    // Expand along view direction (silhouette-based)
                    // Use normal to create perpendicular expansion
                    expandDir = normalize(normalWS - dot(normalWS, viewDirWS) * viewDirWS);
                    // Scale by how perpendicular the normal is to the view
                    float edgeFactor = 1.0 - abs(dot(normalWS, viewDirWS));
                    expandDir = normalWS + expandDir * edgeFactor;
                    expandDir = normalize(expandDir);
                }
                else
                {
                    // Simple normal expansion
                    expandDir = normalWS;
                }
                
                // Expand position outward
                float3 expandedPositionWS = positionWS + expandDir * _OutlineWidth;
                
                // Transform to clip space
                output.positionCS = TransformWorldToHClip(expandedPositionWS);
                output.normalWS = normalWS;
                output.viewDirWS = viewDirWS;
                
                // Pre-calculate fresnel for fragment shader
                float fresnel = 1.0 - saturate(dot(normalWS, viewDirWS));
                output.fresnel = pow(fresnel, 1.5);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Use fresnel to make edges brighter (optional - can remove for uniform glow)
                float glowMultiplier = lerp(0.8, 1.5, input.fresnel);
                
                float3 glow = _GlowColor.rgb * _GlowIntensity * glowMultiplier;
                
                // Alpha fades based on fresnel for smoother edges
                float alpha = lerp(0.3, 1.0, input.fresnel);
                
                return half4(glow, alpha * _GlowColor.a);
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

