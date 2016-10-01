using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SphereGenerator : MonoBehaviour {

	[Tooltip("Approximate size of a tile in qubic meters.")]
	public float tileSize = 1f;
	public float radius = 40f;
	[Tooltip("Deformation of the whole planet.")]
	public float deformation = .25f;
	[Tooltip("Size independent deformation.")]
	public float ruggedness = .5f;

	public bool doubleSided = true;
	public float floorHeight = 1f;

	private Vector2[] uvPoints = new Vector2[3] {new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f)};
	
	private struct TriangleIndices {
		public int v1, v2, v3;

		public TriangleIndices(int v1, int v2, int v3)
		{
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}
	}

	// return index of point in the middle of p1 and p2. Create it if it doesn't exists.
	private int getMiddlePoint(int p1, int p2, ref Dictionary<long, int> middlePointIndexCache, ref List<Vector3> vertList, int currentRefineStep) {
		// first check if we have it already
		bool firstIsSmaller = p1 < p2;
		long smallerIndex = firstIsSmaller ? p1 : p2;
		long greaterIndex = firstIsSmaller ? p2 : p1;
		long key = (smallerIndex << 32) + greaterIndex;

		int ret;
		if (middlePointIndexCache.TryGetValue(key, out ret)) {
			return ret;
		}

		// not in cache, calculate it
		Vector3 point1 = vertList[p1];
		Vector3 point2 = vertList[p2];
		Vector3 middle = (point1 + point2) / 2;

		// add vertex makes sure point is on unit sphere
		int i = vertList.Count;
		float middleRadius = (point1.magnitude + point2.magnitude) / 2;
		float variance = Mathf.Abs (point1.magnitude - point2.magnitude);
		float rad = middleRadius;
		rad += (Random.value - .5f) * deformation*radius/Mathf.Pow(currentRefineStep+1,2); // deformation
		rad += (Random.value - .5f) * variance * ruggedness; // ruggedness
		vertList.Add( middle.normalized * rad);

		// store it, return index
		middlePointIndexCache.Add(key, i);

		return i;
	}
//
//	private int createInsidePoint(int i) {
//		int ret;
//		if (insideIndexCache.TryGetValue(i, out ret)) {
//			return ret;
//		}
//		Vector3 point = vertList [i];
//		Vector3 insidePoint = point * (radius - floorHeight) / radius;
//		int index = vertList.Count;
//		vertList.Add (insidePoint);
//		insideIndexCache.Add (i, index);
//		return index;
//	}

	void Start() {
		MeshFilter filter = GetOrAddComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		if (mesh)
			mesh.Clear ();
		else {
			filter.mesh = new Mesh (); 
		}
			
		// create 12 vertices of a icosahedron
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		Vector3 v0 = new Vector3(-1f,  t,  0f).normalized * radius;
		Vector3 v1 = new Vector3( 1f,  t,  0f).normalized * radius;
		Vector3 v2 = new Vector3(-1f, -t,  0f).normalized * radius;
		Vector3 v3 = new Vector3( 1f, -t,  0f).normalized * radius;

		Vector3 v4 = new Vector3( 0f, -1f,  t).normalized * radius;
		Vector3 v5 = new Vector3( 0f,  1f,  t).normalized * radius;
		Vector3 v6 = new Vector3( 0f, -1f, -t).normalized * radius;
		Vector3 v7 = new Vector3( 0f,  1f, -t).normalized * radius;

		Vector3 v8 = new Vector3( t,  0f, -1f).normalized * radius;
		Vector3 v9 = new Vector3( t,  0f,  1f).normalized * radius;
		Vector3 v10 = new Vector3(-t,  0f, -1f).normalized * radius;
		Vector3 v11 = new Vector3(-t,  0f,  1f).normalized * radius;

		createSide (v0, v11, v5);
		createSide (v0, v5, v1);
		createSide (v0, v1, v7);
		createSide (v0, v7, v10);
		createSide (v0, v10, v11);

		createSide (v1, v5, v9);
		createSide (v5, v11, v4);
		createSide (v11, v10, v2);
		createSide (v10, v7, v6);
		createSide (v7, v1, v8);

		createSide (v3, v9, v4);
		createSide (v3, v4, v2);
		createSide (v3, v2, v6);
		createSide (v3, v6, v8);
		createSide (v3, v8, v9);

		createSide (v4, v9, v5);
		createSide (v2, v4, v11);
		createSide (v6, v2, v10);
		createSide (v8, v6, v7);
		createSide (v9, v8, v1);
		return;


//
//		// inside floor
//		if (doubleSided) {
//			int facesCount = faces.Count;
//			for (int i = 0; i < facesCount; i++) {
//				TriangleIndices face = faces [i];
//				int vert1 = createInsidePoint (face.v1);
//				int vert2 = createInsidePoint (face.v2);
//				int vert3 = createInsidePoint (face.v3);
//
//				faces.Add(new TriangleIndices(vert3, vert2, vert1));
//			}
//		}


	}

	void createSide(Vector3 v1, Vector3 v2, Vector3 v3) {
		// SETUP
		GameObject go = new GameObject ();
		go.name = "Side";
		go.transform.parent = this.transform;

		MeshRenderer meshRenderer = go.AddComponent<MeshRenderer> () as MeshRenderer;
		meshRenderer.material = (this.GetComponent<MeshRenderer> () as MeshRenderer).material;
		MeshFilter meshFilter = go.AddComponent<MeshFilter> () as MeshFilter;
		Mesh mesh = new Mesh ();
		meshFilter.mesh = mesh;

		List<TriangleIndices> faces = new List<TriangleIndices>();
		List<Vector3> vertList = new List<Vector3>();
		ArrayList<Vector2> uv = new ArrayList<Vector2> ();
		Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
		Dictionary<int, int> insideIndexCache = new Dictionary<int, int>();
		int currentRefineStep;

		// add first 3 vertices
		vertList.AddRange(new Vector3[] {v1, v2, v3});
		uv = new Vector2[] { uvPoints [0], uvPoints [1], uvPoints [2] };
		faces.Add (new TriangleIndices (0, 1, 2));

		// REFINE
		// Calculate how many steps have to be made until a face is approx small enough.
		float surfaceArea = 4 * Mathf.PI * radius * radius;
		float x = Mathf.Log(surfaceArea / (tileSize * 20), 4);
		int refineSteps = Mathf.CeilToInt (x);

		// refine triangles
		for (currentRefineStep = 0; currentRefineStep < refineSteps; currentRefineStep++) {

			List<TriangleIndices> faces2 = new List<TriangleIndices>();
			Vector2[] uv2 = new Vector2[currentRefineStep];
			uv.CopyTo (uv2, 0);
			uv = uv2;
			foreach (var tri in faces) {
				// replace triangle by 4 triangles
				int a = getMiddlePoint(tri.v1, tri.v2, ref middlePointIndexCache, ref vertList, currentRefineStep);
				int b = getMiddlePoint(tri.v2, tri.v3, ref middlePointIndexCache, ref vertList, currentRefineStep);
				int c = getMiddlePoint(tri.v3, tri.v1, ref middlePointIndexCache, ref vertList, currentRefineStep);
				uv[a] = getOppositeUV(new Vector2[] {uv[tri.v1], uv[tri.v2]});
				uv[b] = getOppositeUV(new Vector2[] {uv[tri.v2], uv[tri.v3]});
				uv[c] = getOppositeUV(new Vector2[] {uv[tri.v3], uv[tri.v1]});

				// split face in 4 new faces
				faces2.Add(new TriangleIndices(tri.v1, a, c));
				faces2.Add(new TriangleIndices(tri.v2, b, a));
				faces2.Add(new TriangleIndices(tri.v3, c, b));
				faces2.Add(new TriangleIndices(a, b, c));
			}
			faces = faces2;
		}

		// FINALIZE
		mesh.vertices = vertList.ToArray();

		List< int > triList = new List<int>();
		for( int i = 0; i < faces.Count; i++ ) {
			triList.Add( faces[i].v1 );
			triList.Add( faces[i].v2 );
			triList.Add( faces[i].v3 );
		}
		mesh.triangles = triList.ToArray();
		//mesh.uv = uvList.ToArray ();
//		mesh.uv = new Vector2[vertList.Count];
		mesh.uv = uv;

		Vector3[] normales = new Vector3[ vertList.Count];
		for (int i = 0; i < normales.Length; i++)
			normales [i] = vertList [i].normalized;
		mesh.normals = normales;

		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	Vector2 getOppositeUV(Vector2[] uvs) {
		if (!uvs.Contains(uvPoints[0]))
			return uvPoints[0];
		if (!uvs.Contains (uvPoints [1]))
			return uvPoints [1];
		if (!uvs.Contains (uvPoints [2]))
			return uvPoints [2];
		return Vector2.zero;
	}

	T GetOrAddComponent<T>() where T: Component {
		T component = gameObject.GetComponent<T> () as T;
		if (component == null)
			component = gameObject.AddComponent<T> () as T;
		return component;
	}
}