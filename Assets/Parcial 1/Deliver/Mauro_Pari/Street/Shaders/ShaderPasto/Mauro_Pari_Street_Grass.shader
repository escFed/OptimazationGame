// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mauro_Pari_Shader_Grass"
{
	Properties
	{
		_AlbedoPasto("AlbedoPasto", 2D) = "white" {}
		_NormalPasto("NormalPasto", 2D) = "white" {}
		_GrassHeight("GrassHeight", Float) = 1
		_PastoHeight("PastoHeight", 2D) = "white" {}
		_GrassBladeTexture("GrassBladeTexture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _PastoHeight;
		uniform float4 _PastoHeight_ST;
		uniform float _GrassHeight;
		uniform sampler2D _NormalPasto;
		uniform float4 _NormalPasto_ST;
		uniform sampler2D _GrassBladeTexture;
		uniform float4 _GrassBladeTexture_ST;
		uniform sampler2D _AlbedoPasto;
		uniform float4 _AlbedoPasto_ST;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, 0.0);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 uv_PastoHeight = v.texcoord * _PastoHeight_ST.xy + _PastoHeight_ST.zw;
			float4 tex2DNode61 = tex2Dlod( _PastoHeight, float4( uv_PastoHeight, 0, 0.0) );
			float4 PastoHeightRef67 = ( ( 1.0 - tex2DNode61 ) * float4( float3(0,1,0) , 0.0 ) * _GrassHeight );
			v.vertex.xyz += PastoHeightRef67.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalPasto = i.uv_texcoord * _NormalPasto_ST.xy + _NormalPasto_ST.zw;
			float3 localUnpackNormal55 = UnpackScaleNormal( tex2D( _NormalPasto, uv_NormalPasto ), 1.4 );
			float3 NormalPastoReferencia80 = localUnpackNormal55;
			o.Normal = NormalPastoReferencia80;
			float GrassNormalZChannel113 = pow( localUnpackNormal55.z , 3.0 );
			float4 color47 = IsGammaSpace() ? float4(0.1225652,0.3773585,0.08009969,1) : float4(0.01388842,0.1175492,0.007207164,1);
			float2 uv_GrassBladeTexture = i.uv_texcoord * _GrassBladeTexture_ST.xy + _GrassBladeTexture_ST.zw;
			float4 TexturaArena105 = ( color47 * tex2D( _GrassBladeTexture, uv_GrassBladeTexture ) );
			float4 color93 = IsGammaSpace() ? float4(0.1215686,0.3764706,0.07843138,1) : float4(0.01370208,0.1169707,0.00699541,1);
			float2 uv_AlbedoPasto = i.uv_texcoord * _AlbedoPasto_ST.xy + _AlbedoPasto_ST.zw;
			float3 hsvTorgb34 = RGBToHSV( CalculateContrast(1.5,tex2D( _AlbedoPasto, uv_AlbedoPasto )).rgb );
			float3 hsvTorgb31 = HSVToRGB( float3(hsvTorgb34.x,( hsvTorgb34.y * 1.1 ),( hsvTorgb34.z * 0.85 )) );
			float4 AlbedoBaseRef102 = ( color93 * float4( hsvTorgb31 , 0.0 ) );
			float4 lerpResult46 = lerp( TexturaArena105 , AlbedoBaseRef102 , float4( 0,0,0,0 ));
			float4 AlbedoPastoReferencia79 = lerpResult46;
			o.Albedo = ( GrassNormalZChannel113 * AlbedoPastoReferencia79 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
202;73;1717;918;1623.488;451.3163;1.862724;False;False
Node;AmplifyShaderEditor.CommentaryNode;115;-1717.276,-1457.61;Inherit;False;2270.153;1347.291;Albedo;4;108;106;110;104;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;108;-1667.276,-1407.61;Inherit;False;2170.153;589.7882;Textura Base del Albedo. editada;13;1;19;34;41;36;35;32;37;40;31;93;92;102;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-1617.276,-1260.693;Inherit;True;Property;_AlbedoPasto;AlbedoPasto;0;0;Create;True;0;0;0;False;0;False;-1;c68296334e691ed45b62266cbc716628;c68296334e691ed45b62266cbc716628;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleContrastOpNode;19;-1262.458,-1220.107;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;1.5;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;34;-943.1402,-1200.392;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;36;-720.5356,-1271.69;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-808.8642,-933.823;Inherit;False;Constant;_ValueMultiplier;ValueMultiplier;1;0;Create;True;0;0;0;False;0;False;0.85;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-656.8387,-1089.685;Inherit;False;Constant;_SaturationAmount;SaturationAmount;1;0;Create;True;0;0;0;False;0;False;1.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-597.8638,-995.823;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;106;-1548.037,-705.2465;Inherit;False;833.9158;463.7677;Textura de Arena con color, para simular más hojas;4;94;47;95;105;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;37;-340.6215,-1289.797;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-377.6436,-1155.85;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;94;-1498.037,-471.4786;Inherit;True;Property;_GrassBladeTexture;GrassBladeTexture;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;47;-1419.257,-655.2465;Inherit;False;Constant;_Color2;Color 2;1;0;Create;True;0;0;0;False;0;False;0.1225652,0.3773585,0.08009969,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.HSVToRGBNode;31;-209.7702,-1174.255;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;93;-212.636,-1357.61;Inherit;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;0;False;0;False;0.1215686,0.3764706,0.07843138,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1155.038,-560.4788;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;33.36347,-1212.609;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;116;-1676.48,-36.9312;Inherit;False;1008.162;331.2904;Tomamos el valor Z del normal para oscurecer las zonas sombreadas;5;53;55;113;80;56;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;275.8771,-1078.246;Inherit;False;AlbedoBaseRef;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;53;-1626.48,38.85269;Inherit;True;Property;_NormalPasto;NormalPasto;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;76;-1618.983,348.6799;Inherit;False;949.8789;559.0673;El vector toma la dirección hacia arriba (Y) del heightMap, el float permite ajustar que tan alto va el pasto.;7;61;75;67;85;96;65;63;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;104;-573.1794,-414.9374;Inherit;False;820.704;268.1722;Combinación textura base, con textura de arena con step para teñir solo la parte superior = Albedo;4;103;46;79;107;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-938.1219,-476.6949;Inherit;False;TexturaArena;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;55;-1316.762,29.47979;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1.4;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;103;-529.1005,-272.2395;Inherit;False;102;AlbedoBaseRef;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-522.377,-366.4962;Inherit;False;105;TexturaArena;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;61;-1603.663,400.5051;Inherit;True;Property;_PastoHeight;PastoHeight;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;63;-1409.906,637.3962;Inherit;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;96;-1278.06,522.899;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;56;-1084.95,136.338;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;46;-253.1715,-343.4008;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;126;-587.6537,-23.81937;Inherit;False;1067.948;933.1813;Results;5;125;124;122;0;120;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1419.189,794.3466;Inherit;False;Property;_GrassHeight;GrassHeight;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;125;-537.6537,26.18063;Inherit;False;541.0906;264.5953;Multiplicamos el albedo terminado del pasto con el Canal Z del normal del pasto;3;114;57;82;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-932.3167,178.3593;Inherit;False;GrassNormalZChannel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-1097.94,530.2527;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-23.47515,-364.9374;Inherit;False;AlbedoPastoReferencia;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-949.6873,450.0319;Inherit;False;PastoHeightRef;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-980.4202,13.06881;Inherit;False;NormalPastoReferencia;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;122;-394.4888,529.5635;Inherit;False;572.6695;166;El offset vertical dictado por el heightmap y sus cálculos;1;71;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;124;-404.1319,741.6429;Inherit;False;595.2004;166.4018;Mantiene los picos fijos para que no se muevan con la cámara;1;68;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;110;-493.1596,-778.2025;Inherit;False;662.9934;342.4016;Step para teñir solo la parte superior de los picos;4;88;89;91;109;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-487.6537,174.7758;Inherit;False;79;AlbedoPastoReferencia;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;120;-266.4379,337.9384;Inherit;False;320;166;El normal de la textura del pasto, sin más;1;81;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-466.5704,76.18065;Inherit;False;113;GrassNormalZChannel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-214.2081,387.9384;Inherit;False;80;NormalPastoReferencia;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;-57.16616,-650.4216;Inherit;False;StepReferencia;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;89;-443.1596,-728.2023;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;88;-225.4792,-669.4512;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-425.0406,-551.8007;Inherit;False;Constant;_Step;Step;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-1185.36,412.5364;Inherit;False;HeightMapRef;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-158.5628,142.6462;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-221.3366,592.9167;Inherit;False;67;PastoHeightRef;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.EdgeLengthTessNode;68;-206.5234,798.3627;Inherit;False;1;0;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;225.2945,252.3141;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Mauro_Pari_Shader_Grass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;1;1;0
WireConnection;34;0;19;0
WireConnection;36;0;34;1
WireConnection;40;0;34;3
WireConnection;40;1;41;0
WireConnection;37;0;36;0
WireConnection;32;0;34;2
WireConnection;32;1;35;0
WireConnection;31;0;37;0
WireConnection;31;1;32;0
WireConnection;31;2;40;0
WireConnection;95;0;47;0
WireConnection;95;1;94;0
WireConnection;92;0;93;0
WireConnection;92;1;31;0
WireConnection;102;0;92;0
WireConnection;105;0;95;0
WireConnection;55;0;53;0
WireConnection;96;0;61;0
WireConnection;56;0;55;3
WireConnection;46;0;107;0
WireConnection;46;1;103;0
WireConnection;113;0;56;0
WireConnection;75;0;96;0
WireConnection;75;1;63;0
WireConnection;75;2;65;0
WireConnection;79;0;46;0
WireConnection;67;0;75;0
WireConnection;80;0;55;0
WireConnection;109;0;88;0
WireConnection;88;0;89;2
WireConnection;88;1;91;0
WireConnection;85;0;61;0
WireConnection;57;0;114;0
WireConnection;57;1;82;0
WireConnection;0;0;57;0
WireConnection;0;1;81;0
WireConnection;0;11;71;0
WireConnection;0;14;68;0
ASEEND*/
//CHKSM=A804A447FA5B928ABE997B6FAFB995A521ACB6F5