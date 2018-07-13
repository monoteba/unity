Shader "mortentoo/Simple Vertex Fragment"
{
    Properties
    {
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _MainTex("Main Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            //#include "UnityCG.cginc"
            
            // properties
            half4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST; // texture tiling and offset
            
            // structs
            struct vertInput // appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct vertOutput // v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            vertOutput vert(vertInput input)
            {
                vertOutput o;
                
                // transfer vertex from world space to screen space
                o.pos = UnityObjectToClipPos(input.pos);
                
                // maps uv's to texture
                o.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                
                return o;
            }
            
            half4 frag(vertOutput output) : SV_TARGET
            {
                half4 tex = tex2D(_MainTex, output.uv);
                return _Color * tex;
            }
            
            ENDCG
        }
    }
}
