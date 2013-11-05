﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class GeometryTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void SetInvalidDataThrows()
		{
			var geometry =
				ContentLoader.Create<Geometry>(new GeometryCreationData(VertexFormat.Position3DColor, 1, 1));
			Assert.Throws<Geometry.InvalidNumberOfVertices>(
				() => geometry.SetData(new Vertex[] { }, new short[] { 0 }));
			Assert.Throws<Geometry.InvalidNumberOfIndices>(
				() => geometry.SetData(new Vertex[] { new VertexPosition3DColor(Vector3D.Zero, Color.Red) },
					new short[] { }));
		}

		[Test, CloseAfterFirstFrame]
		public void LoadInvalidDataThrows()
		{
			var creationData = new GeometryCreationData(VertexFormat.Position3DColor, 1, 1);
			var geometry = ContentLoader.Create<TestGeometry>(creationData);
			Assert.Throws<Geometry.EmptyGeometryFileGiven>(geometry.LoadInvalidData);
		}

		private class TestGeometry : Geometry
		{
			protected TestGeometry(string contentName)
				: base(contentName) { }

			private TestGeometry(GeometryCreationData creationData)
				: base(creationData) {}

			public override void Draw() {} //ncrunch: no coverage
			protected override void SetNativeData(byte[] vertexData, short[] indices) {}
			protected override void DisposeData() {}

			public void LoadInvalidData()
			{
				LoadData(new MemoryStream());
			} //ncrunch: no coverage

			public void LoadValidData()
			{
				var geometryData = new GeometryData
				{
					Format = VertexFormat.Position3DColor,
					Indices = new short[6]
				};
				LoadData(new MemoryStream(BinaryDataExtensions.ToByteArrayWithTypeInformation(geometryData)));
			}
		}

		[Test, CloseAfterFirstFrame]
		public void LoadGeometryWithoutDataFails()
		{
			Assert.Throws<Geometry.EmptyGeometryFileGiven>(
				() => ContentLoader.Load<TestGeometry>("TestGeometry"));
		}

		[Test, CloseAfterFirstFrame]
		public void CreateGeometry()
		{
			var creationData = new GeometryCreationData(VertexFormat.Position3DColor, 1, 1);
			var geometry = ContentLoader.Create<TestGeometry>(creationData);
			geometry.LoadValidData();
			Assert.AreEqual(6, geometry.NumberOfIndices);
		}

		[Test]
		public void ShowTriangle()
		{
			CreateTriangle(new Vertex[]
			{
				new VertexPosition3DColor(new Vector3D(-3.0f, 0.0f, 0.0f), Color.Red),
				new VertexPosition3DColor(new Vector3D(3.0f, 0.0f, 0.0f), Color.Yellow),
				new VertexPosition3DColor(new Vector3D(1.5f, 3.0f, 0.0f), Color.Teal)
			}, "");
		}

		private static void CreateTriangle(Vertex[] vertices, string imageName)
		{
			var geometry =
				ContentLoader.Create<Geometry>(new GeometryCreationData(VertexFormat.Position3DColor, 3, 3));
			new GeometryCreationData(VertexFormat.Position3DColor, 3, 3);
			geometry.SetData(vertices, new short[] { 0, 1, 2 });
			new Triangle(geometry, new Material(Shader.Position3DColor, imageName));
		}

		private class Triangle : DrawableEntity
		{
			public Triangle(Geometry geometry, Material material)
			{
				this.geometry = geometry;
				this.material = material;
				OnDraw<DrawTriangle>();
			}

			private readonly Geometry geometry;
			private readonly Material material;

			private class DrawTriangle : DrawBehavior
			{
				public DrawTriangle(Drawing drawing)
				{
					this.drawing = drawing;
				}

				private readonly Drawing drawing;

				public void Draw(List<DrawableEntity> visibleEntities)
				{
					foreach (var triangle in visibleEntities.OfType<Triangle>())
						drawing.AddGeometry(triangle.geometry, triangle.material, Matrix.Identity);
				}
			}
		}

		[Test]
		public void ShowSquareIn3D()
		{
			CreateTriangle(new Vertex[]
			{
				new VertexPosition3DColor(new Vector3D(-3.0f, 3.0f, 0.0f), Color.Red),
				new VertexPosition3DColor(new Vector3D(-3.0f, -3.0f, 0.0f), Color.Red),
				new VertexPosition3DColor(new Vector3D(3.0f, -3.0f, 0.0f), Color.Red)
			}, "DeltaEngineLogo");
			CreateTriangle(new Vertex[]
			{
				new VertexPosition3DColor(new Vector3D(3.0f, 3.0f, 0.0f), Color.Red),
				new VertexPosition3DColor(new Vector3D(-3.0f, 3.0f, 0.0f), Color.Red),
				new VertexPosition3DColor(new Vector3D(3.0f, -3.0f, 0.0f), Color.Red)
			}, "DeltaEngineLogo");
		}

		[Test]
		public void ShowLineIn3D()
		{
			var drawing = Resolve<Drawing>();
			var lineMaterial = new Material(Shader.Position3DColor, "");
			drawing.AddLines(lineMaterial,
				new[]
				{
					new VertexPosition3DColor(Vector3D.Zero, Color.Red),
					new VertexPosition3DColor(Vector3D.One, Color.Red)
				});
		}

		[Test]
		public void ShowBillboardSpriteIn3D()
		{
			var drawing = Resolve<Drawing>();
			var billboardMaterial = new Material(Shader.Position3DUV, "DeltaEngineLogo");
			drawing.Add(billboardMaterial,
				new[]
				{
					new VertexPosition3DUV(Vector3D.Zero, Vector2D.Zero),
					new VertexPosition3DUV(Vector3D.UnitX, Vector2D.UnitX),
					new VertexPosition3DUV(Vector3D.UnitY, Vector2D.UnitY)
				});
		}
	}
}