using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
	Dictionary<int, int> insideIndexCache = new Dictionary<int, int>();
	List<Vector3> vertList = new List<Vector3>();
	int currentRefineStep;
	
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
	private int getMiddlePoint(int p1, int p2) {
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

	private int createInsidePoint(int i) {
		int ret;
		if (insideIndexCache.TryGetValue(i, out ret)) {
			return ret;
		}
		Vector3 point = vertList [i];
		Vector3 insidePoint = point * (radius - floorHeight) / radius;
		int index = vertList.Count;
		vertList.Add (insidePoint);
		insideIndexCache.Add (i, index);
		return index;
	}

	void Start() {
		MeshFilter filter = GetOrAddComponent< MeshFilter >();
		Mesh mesh = filter.sharedMesh;
		mesh.Clear();

		// create 12 vertices of a icosahedron
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		vertList.Add(new Vector3(-1f,  t,  0f).normalized * radius);
		vertList.Add(new Vector3( 1f,  t,  0f).normalized * radius);
		vertList.Add(new Vector3(-1f, -t,  0f).normalized * radius);
		vertList.Add(new Vector3( 1f, -t,  0f).normalized * radius);

		vertList.Add(new Vector3( 0f, -1f,  t).normalized * radius);
		vertList.Add(new Vector3( 0f,  1f,  t).normalized * radius);
		vertList.Add(new Vector3( 0f, -1f, -t).normalized * radius);
		vertList.Add(new Vector3( 0f,  1f, -t).normalized * radius);

		vertList.Add(new Vector3( t,  0f, -1f).normalized * radius);
		vertList.Add(new Vector3( t,  0f,  1f).normalized * radius);
		vertList.Add(new Vector3(-t,  0f, -1f).normalized * radius);
		vertList.Add(new Vector3(-t,  0f,  1f).normalized * radius);


		// create 20 triangles of the icosahedron
		List<TriangleIndices> faces = new List<TriangleIndices>();

		// 5 faces around point 0
		faces.Add(new TriangleIndices(0, 11, 5));
		faces.Add(new TriangleIndices(0, 5, 1));
		faces.Add(new TriangleIndices(0, 1, 7));
		faces.Add(new TriangleIndices(0, 7, 10));
		faces.Add(new TriangleIndices(0, 10, 11));

		// 5 adjacent faces 
		faces.Add(new TriangleIndices(1, 5, 9));
		faces.Add(new TriangleIndices(5, 11, 4));
		faces.Add(new TriangleIndices(11, 10, 2));
		faces.Add(new TriangleIndices(10, 7, 6));
		faces.Add(new TriangleIndices(7, 1, 8));

		// 5 faces around point 3
		faces.Add(new TriangleIndices(3, 9, 4));
		faces.Add(new TriangleIndices(3, 4, 2));
		faces.Add(new TriangleIndices(3, 2, 6));
		faces.Add(new TriangleIndices(3, 6, 8));
		faces.Add(new TriangleIndices(3, 8, 9));

		// 5 adjacent faces 
		faces.Add(new TriangleIndices(4, 9, 5));
		faces.Add(new TriangleIndices(2, 4, 11));
		faces.Add(new TriangleIndices(6, 2, 10));
		faces.Add(new TriangleIndices(8, 6, 7));
		faces.Add(new TriangleIndices(9, 8, 1));


		// Calculate how many steps have to be made until a face is approx small enough.
		float surfaceArea = 4 * Mathf.PI * radius * radius;
		float x = Mathf.Log(surfaceArea / (tileSize * 20), 4);
		int refineSteps = Mathf.CeilToInt (x);

		// refine triangles
		for (currentRefineStep = 0; currentRefineStep < refineSteps; currentRefineStep++) {
			
			List<TriangleIndices> faces2 = new List<TriangleIndices>();
			foreach (var tri in faces) {
				// replace triangle by 4 triangles
				int a = getMiddlePoint(tri.v1, tri.v2);
				int b = getMiddlePoint(tri.v2, tri.v3);
				int c = getMiddlePoint(tri.v3, tri.v1);

				faces2.Add(new TriangleIndices(tri.v1, a, c));
				faces2.Add(new TriangleIndices(tri.v2, b, a));
				faces2.Add(new TriangleIndices(tri.v3, c, b));
				faces2.Add(new TriangleIndices(a, b, c));
			}
			faces = faces2;
		}

		// inside floor
		if (doubleSided) {
			int facesCount = faces.Count;
			for (int i = 0; i < facesCount; i++) {
				TriangleIndices face = faces [i];
				int v1 = createInsidePoint (face.v1);
				int v2 = createInsidePoint (face.v2);
				int v3 = createInsidePoint (face.v3);

				faces.Add(new TriangleIndices(v3, v2, v1));
			}
		}

		mesh.vertices = vertList.ToArray();

		List< int > triList = new List<int>();
		for( int i = 0; i < faces.Count; i++ ) {
			triList.Add( faces[i].v1 );
			triList.Add( faces[i].v2 );
			triList.Add( faces[i].v3 );
		}
		mesh.triangles = triList.ToArray();
		mesh.uv = new Vector2[ mesh.vertices.Length ];

		Vector3[] normales = new Vector3[ vertList.Count];
		for( int i = 0; i < normales.Length; i++ )
			normales[i] = vertList[i].normalized;


		mesh.normals = normales;

		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	T GetOrAddComponent<T>() where T: Component {
		T component = gameObject.GetComponent<T> () as T;
		if (component == null)
			component = gameObject.AddComponent<T> () as T;
		return component;
	}
}