using UnityEngine;
using System;

namespace GameDataEditor
{
    [Serializable]
    public struct GDEV2
    {
        public float x;
        public float y;

        public GDEV2(float px, float py)
        {
            x = px;
            y = py;
        }

		static public implicit operator GDEV2(Vector2 pV2)
        {
			return new GDEV2(pV2.x, pV2.y);
        }

		static public implicit operator Vector2(GDEV2 pV2)
		{
			return new Vector2(pV2.x, pV2.y);
		}
    }

    [Serializable]
    public struct GDEV3
    {
		public float x;
		public float y;
        public float z;

		public GDEV3(float px, float py, float pz)
		{
			x = px;
			y = py;
			z = pz;
		}
		
		public GDEV3(Vector3 pV3)
		{
			x = pV3.x;
			y = pV3.y;
			z = pV3.z;
		}

		static public implicit operator GDEV3(Vector3 pV3)
		{
			return new GDEV3(pV3.x, pV3.y, pV3.z);
		}
		
		static public implicit operator Vector3(GDEV3 pV3)
		{
			return new Vector3(pV3.x, pV3.y, pV3.z);
		}
    }

    [Serializable]
    public struct GDEV4
    {
		public float x;
		public float y;
		public float z;
        public float w;

		public GDEV4(float px, float py, float pz, float pw)
		{
			x = px;
			y = py;
			z = pz;
			w = pw;
		}
		
		public GDEV4(Vector4 pV4)
		{
			x = pV4.x;
			y = pV4.y;
			z = pV4.z;
			w = pV4.w;
		}

		static public implicit operator GDEV4(Vector4 pV4)
		{
			return new GDEV4(pV4.x, pV4.y, pV4.z, pV4.w);
		}
		
		static public implicit operator Vector4(GDEV4 pV4)
		{
			return new Vector4(pV4.x, pV4.y, pV4.z, pV4.w);
		}
    }

	[Serializable]
	public struct GDEColor
	{
		public float r;
		public float g;
		public float b;
		public float a;

		public GDEColor(float pr, float pg, float pb, float pa)
		{
			r = pr;
			g = pg;
			b = pb;
			a = pa;
		}

		static public implicit operator GDEColor(Color pCol)
		{
			return new GDEColor(pCol.r, pCol.g, pCol.b, pCol.a);
		}
		
		static public implicit operator Color(GDEColor pCol)
		{
			return new Color(pCol.r, pCol.g, pCol.b, pCol.a);
		}
	}
}

