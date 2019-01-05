// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33559,y:32181,varname:node_4013,prsc:2|diff-5632-OUT;n:type:ShaderForge.SFN_Tex2d,id:5431,x:32062,y:32452,ptovrint:False,ptlb:baseColor,ptin:_baseColor,varname:node_5431,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8ddd91b52d2d3364e8b0cc90dd9ecb90,ntxv:0,isnm:False|MIP-9475-VFACE;n:type:ShaderForge.SFN_VertexColor,id:3407,x:31836,y:32596,varname:node_3407,prsc:2;n:type:ShaderForge.SFN_Slider,id:8951,x:32120,y:32131,ptovrint:False,ptlb:Hue,ptin:_Hue,varname:node_8951,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_HsvToRgb,id:9641,x:32439,y:32114,varname:node_9641,prsc:2|H-8951-OUT,S-2748-OUT,V-5848-OUT;n:type:ShaderForge.SFN_Slider,id:2748,x:32120,y:32227,ptovrint:False,ptlb:Power,ptin:_Power,varname:_Hue_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:5848,x:32120,y:32322,ptovrint:False,ptlb:Lightnes,ptin:_Lightnes,varname:_Hue_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:1303,x:32277,y:32399,varname:node_1303,prsc:2|A-9641-OUT,B-5431-RGB;n:type:ShaderForge.SFN_Slider,id:7753,x:31836,y:32859,ptovrint:False,ptlb:Effect,ptin:_Effect,varname:node_7753,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_VertexColor,id:7563,x:32417,y:32758,varname:node_7563,prsc:2;n:type:ShaderForge.SFN_Slider,id:3641,x:32427,y:33025,ptovrint:False,ptlb:AO,ptin:_AO,varname:node_3641,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:5632,x:33010,y:32458,varname:node_5632,prsc:2|A-5711-OUT,B-8736-OUT;n:type:ShaderForge.SFN_Lerp,id:8736,x:32689,y:32689,varname:node_8736,prsc:2|A-7563-A,B-4328-OUT,T-3641-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4328,x:32427,y:32929,ptovrint:False,ptlb:EmptyValue,ptin:_EmptyValue,varname:node_4328,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:3475,x:32098,y:32702,varname:node_3475,prsc:2|A-3407-RGB,B-4639-OUT,T-7753-OUT;n:type:ShaderForge.SFN_Multiply,id:5711,x:32673,y:32505,varname:node_5711,prsc:2|A-8260-OUT,B-3475-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4639,x:31836,y:32766,ptovrint:False,ptlb:EmptyValue1,ptin:_EmptyValue1,varname:node_4639,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Slider,id:3837,x:32661,y:32280,ptovrint:False,ptlb:Saturation,ptin:_Saturation,varname:node_3837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_RgbToHsv,id:5497,x:32740,y:32093,varname:node_5497,prsc:2|IN-1303-OUT;n:type:ShaderForge.SFN_Lerp,id:8260,x:32903,y:32093,varname:node_8260,prsc:2|A-5497-VOUT,B-1303-OUT,T-3837-OUT;n:type:ShaderForge.SFN_FaceSign,id:9475,x:31874,y:32424,varname:node_9475,prsc:2,fstp:0;proporder:5431-8951-2748-3837-5848-7753-3641-4328-4639;pass:END;sub:END;*/

Shader "Creativemood/DoubleshadedVertexColor" {
    Properties {
        _baseColor ("baseColor", 2D) = "white" {}
        _Hue ("Hue", Range(0, 1)) = 0.5
        _Power ("Power", Range(0, 1)) = 0
        _Saturation ("Saturation", Range(0, 1)) = 1
        _Lightnes ("Lightnes", Range(0, 2)) = 1
        _Effect ("Effect", Range(0, 1)) = 0.5
        _AO ("AO", Range(0, 1)) = 0.5
        [HideInInspector]_EmptyValue ("EmptyValue", Float ) = 1
        [HideInInspector]_EmptyValue1 ("EmptyValue1", Float ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _baseColor; uniform float4 _baseColor_ST;
            uniform float _Hue;
            uniform float _Power;
            uniform float _Lightnes;
            uniform float _Effect;
            uniform float _AO;
            uniform float _EmptyValue;
            uniform float _EmptyValue1;
            uniform float _Saturation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _baseColor_var = tex2Dlod(_baseColor,float4(TRANSFORM_TEX(i.uv0, _baseColor),0.0,isFrontFace));
                float3 node_1303 = ((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(_Hue+float3(0.0,-1.0/3.0,1.0/3.0)))-1),_Power)*_Lightnes)*_baseColor_var.rgb);
                float4 node_5497_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_5497_p = lerp(float4(float4(node_1303,0.0).zy, node_5497_k.wz), float4(float4(node_1303,0.0).yz, node_5497_k.xy), step(float4(node_1303,0.0).z, float4(node_1303,0.0).y));
                float4 node_5497_q = lerp(float4(node_5497_p.xyw, float4(node_1303,0.0).x), float4(float4(node_1303,0.0).x, node_5497_p.yzx), step(node_5497_p.x, float4(node_1303,0.0).x));
                float node_5497_d = node_5497_q.x - min(node_5497_q.w, node_5497_q.y);
                float node_5497_e = 1.0e-10;
                float3 node_5497 = float3(abs(node_5497_q.z + (node_5497_q.w - node_5497_q.y) / (6.0 * node_5497_d + node_5497_e)), node_5497_d / (node_5497_q.x + node_5497_e), node_5497_q.x);;
                float3 node_5632 = ((lerp(float3(node_5497.b,node_5497.b,node_5497.b),node_1303,_Saturation)*lerp(i.vertexColor.rgb,float3(_EmptyValue1,_EmptyValue1,_EmptyValue1),_Effect))*lerp(i.vertexColor.a,_EmptyValue,_AO));
                float3 diffuseColor = node_5632;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _baseColor; uniform float4 _baseColor_ST;
            uniform float _Hue;
            uniform float _Power;
            uniform float _Lightnes;
            uniform float _Effect;
            uniform float _AO;
            uniform float _EmptyValue;
            uniform float _EmptyValue1;
            uniform float _Saturation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _baseColor_var = tex2Dlod(_baseColor,float4(TRANSFORM_TEX(i.uv0, _baseColor),0.0,isFrontFace));
                float3 node_1303 = ((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(_Hue+float3(0.0,-1.0/3.0,1.0/3.0)))-1),_Power)*_Lightnes)*_baseColor_var.rgb);
                float4 node_5497_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_5497_p = lerp(float4(float4(node_1303,0.0).zy, node_5497_k.wz), float4(float4(node_1303,0.0).yz, node_5497_k.xy), step(float4(node_1303,0.0).z, float4(node_1303,0.0).y));
                float4 node_5497_q = lerp(float4(node_5497_p.xyw, float4(node_1303,0.0).x), float4(float4(node_1303,0.0).x, node_5497_p.yzx), step(node_5497_p.x, float4(node_1303,0.0).x));
                float node_5497_d = node_5497_q.x - min(node_5497_q.w, node_5497_q.y);
                float node_5497_e = 1.0e-10;
                float3 node_5497 = float3(abs(node_5497_q.z + (node_5497_q.w - node_5497_q.y) / (6.0 * node_5497_d + node_5497_e)), node_5497_d / (node_5497_q.x + node_5497_e), node_5497_q.x);;
                float3 node_5632 = ((lerp(float3(node_5497.b,node_5497.b,node_5497.b),node_1303,_Saturation)*lerp(i.vertexColor.rgb,float3(_EmptyValue1,_EmptyValue1,_EmptyValue1),_Effect))*lerp(i.vertexColor.a,_EmptyValue,_AO));
                float3 diffuseColor = node_5632;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
