﻿using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Base class for GPU geometry data.
	/// </summary>
	public abstract class Geometry : ContentData
	{
		protected Geometry(string contentName)
			: base(contentName) {}

		protected Geometry(GeometryCreationData creationData)
			: base("<GenerateGeometry>")
		{
			Format = creationData.Format;
			vertices = new byte[creationData.NumberOfVertices * Format.Stride];
			indices = new short[creationData.NumberOfIndices];
		}

		public VertexFormat Format { get; private set; }
		private byte[] vertices;
		private short[] indices;
		public int NumberOfVertices
		{
			get { return vertices.Length / Format.Stride; }
		}
		public int NumberOfIndices
		{
			get { return indices.Length; }
		}

		protected override void LoadData(Stream fileData)
		{
			if (fileData.Length == 0)
				throw new EmptyGeometryFileGiven();
			var loadedGeometry = new BinaryReader(fileData).Create() as GeometryData;
			Format = loadedGeometry.Format;
			vertices = loadedGeometry.VerticesData;
			indices = loadedGeometry.Indices;
			SetNativeData(vertices, indices);
		}

		public class EmptyGeometryFileGiven : Exception {}

		public class GeometryData
		{
			public String Name;
			public VertexFormat Format;
			public int NumberOfVertices;
			public byte[] VerticesData;
			public short[] Indices;
		}

		public abstract void Draw();

		public void SetData(Vertex[] setVertices, short[] setIndices)
		{
			SetData(BinaryDataExtensions.ToByteArray(setVertices), setIndices);
		}

		public void SetData(byte[] verticesData, short[] setIndices)
		{
			if (verticesData.Length != (NumberOfVertices * Format.Stride))
				throw new InvalidNumberOfVertices(verticesData.Length / Format.Stride, NumberOfVertices);
			if (setIndices.Length != NumberOfIndices)
				throw new InvalidNumberOfIndices(setIndices.Length, NumberOfIndices);
			vertices = verticesData;
			indices = setIndices;
			SetNativeData(vertices, indices);
		}

		public class InvalidNumberOfVertices : Exception
		{
			public InvalidNumberOfVertices(int actualVertices, int expectedVertices)
				: base("actualVertices=" + actualVertices + ", expectedVertices=" + expectedVertices) { }
		}

		public class InvalidNumberOfIndices : Exception
		{
			public InvalidNumberOfIndices(int actualIndices, int expectedIndices)
				: base("actualIndices=" + actualIndices + ", expectedIndices=" + expectedIndices) { }
		}

		public Matrix[] JointTranforms { get; set; }
		public bool HasAnimationData { get { return JointTranforms != null; } }

		protected abstract void SetNativeData(byte[] vertexData, short[] indices);
	}
}