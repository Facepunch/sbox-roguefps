using Sandbox.Engine;
using Editor;
using Editor.MapEditor;
using Editor.MapDoc;

namespace Editor;

public partial class GridMapTool
{
	static Material GridMaterial = Material.Load( "materials/grid/grid_material.vmat" );

	List<Vertex> gridVertices = new();
	private SceneModel so;

	public void Grid( Vector2 size, float spacing = 32.0f, float opacity = 1.0f, float minorLineWidth = 0.01f, float majorLineWidth = 0.02f )
	{
		if ( so is null )
		{
			so = new SceneModel( Scene.SceneWorld, "models/grid/grid.vmdl", new Transform( new Vector3( 0, 0, floors ) ) );
			so.SetMaterialOverride( GridMaterial );
			so.RenderLayer = SceneRenderLayer.OverlayWithDepth;
			so.Bounds = new BBox( new Vector3( -size.x / 2, -size.y / 2, 0 ), new Vector3( size.x / 2, size.y / 2, 0 ) );
		}
		so.Attributes.Set( "GridScale", spacing );
		so.Attributes.Set( "MinorLineWidth", 0.0125f );
		so.Attributes.Set( "MajorLineWidth", 0.025f );
		so.Attributes.Set( "AxisLineWidth", 0.03f  );
		so.Attributes.Set( "MinorLineColor", new Vector4( 1, 0.5f, 0, 0.75f ) );
		so.Attributes.Set( "MajorLineColor", new Vector4( 1, 0.5f, 0, 1f ) );
		so.Attributes.Set( "XAxisColor", new Vector4( 1, 0.5f, 0, 0.0f ) );
		so.Attributes.Set( "YAxisColor", new Vector4( 1, 0.5f, 0, 0.0f ) );
		so.Attributes.Set( "ZAxisColor", new Vector4( 1, 0.5f, 0, 0.0f ) );
		so.Attributes.Set( "CenterColor", new Vector4( 1, 0.5f, 0, 1.0f ) );
		so.Attributes.Set( "MajorGridDivisions", 16.0f );

		/*
		// Tessellating helps with depth bias precision
		// Generally 1x1 is enough for 8k x 8k, 2x2 for 16k x 16k and so on.
		// Obvious optimization here is to generate this mesh only once and to use a simpler Vertex format
		// But it barely matters
		int tessellationLevel = 4;
		for ( int i = 0; i < tessellationLevel; i++ )
		{
			for ( int j = 0; j < tessellationLevel; j++ )
			{
				float x0 = i / (float)tessellationLevel;
				float x1 = (i + 1) / (float)tessellationLevel;
				float y0 = j / (float)tessellationLevel;
				float y1 = (j + 1) / (float)tessellationLevel;

				so.Vertices.Add( new Vertex( new Vector3( x0, y0, 0 ) ) );
				so.Vertices.Add( new Vertex( new Vector3( x1, y0, 0 ) ) );
				so.Vertices.Add( new Vertex( new Vector3( x0, y1, 0 ) ) );

				so.Vertices.Add( new Vertex( new Vector3( x0, y1, 0 ) ) );
				so.Vertices.Add( new Vertex( new Vector3( x1, y0, 0 ) ) );
				so.Vertices.Add( new Vertex( new Vector3( x1, y1, 0 ) ) );
			}
		}
		*/
	}

	static Mesh CreatePlane()
	{
		var material = GridMaterial;
		var mesh = new Mesh( material );
		mesh.CreateVertexBuffer<Vertex>( 4, Vertex.Layout, new[]
		{
			new Vertex( new Vector3( -200, -200, 0 ), Vector3.Up, Vector3.Forward, new Vector4( 0, 0, 0, 0 ) ),
			new Vertex( new Vector3( 200, -200, 0 ), Vector3.Up, Vector3.Forward, new Vector4( 2, 0, 0, 0 ) ),
			new Vertex( new Vector3( 200, 200, 0 ), Vector3.Up, Vector3.Forward, new Vector4( 2, 2, 0, 0 ) ),
			new Vertex( new Vector3( -200, 200, 0 ), Vector3.Up, Vector3.Forward, new Vector4( 0, 2, 0, 0 ) ),
		} );
		mesh.CreateIndexBuffer( 6, new[] { 0, 1, 2, 2, 3, 0 } );
		mesh.Bounds = BBox.FromPositionAndSize( 0, 100 );

		return mesh;
	}
}
