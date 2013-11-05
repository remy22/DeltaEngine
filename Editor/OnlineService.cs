﻿using System;
using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Editor
{
	public class OnlineService : Service
	{
		public OnlineService(Settings editorSettings)
		{
			EditorSettings = editorSettings;
		}

		public Settings EditorSettings { get; private set; }

		public void Connect(string userName, OnlineServiceConnection connection)
		{
			UserName = userName;
			onlineServiceConnection = connection;
			connection.DataReceived += OnDataReceived;
			send = connection.Send;
			ContentLoader.DisposeIfInitialized();
			ContentLoader.Use<EditorContentLoader>();
			editorContent = new EditorContentLoader(onlineServiceConnection);
			editorContent.ContentUpdated += OnContentUpdated;
			editorContent.ContentDeleted += OnContentDeleted;
			editorContent.ContentReady += OnContentReady;
		}

		public string UserName { get; private set; }
		private OnlineServiceConnection onlineServiceConnection;

		private void OnDataReceived(object message)
		{
			var newProject = message as SetProject;
			if (newProject != null)
				ChangeProject(newProject);
			if (message.GetType() == typeof(ServerError))
				Logger.Warning(message.ToString());
			else if (DataReceived != null)
				DataReceived(message);
		}

		private void ChangeProject(SetProject project)
		{
			if (project.Permissions == ProjectPermissions.None)
			{
				if (StackTraceExtensions.IsStartedFromNunitConsole())
					throw new Exception("No access to project " + project.ProjectName);
				MessageBox.Show("No access to project " + project.ProjectName, "Fatal Error");
			}
			else
			{
				ProjectName = project.ProjectName;
				Permissions = project.Permissions;
				editorContent.SetProject(project);
				if (ProjectChanged != null)
					ProjectChanged();
			}
		}

		public string ProjectName { get; private set; }
		public ProjectPermissions Permissions { get; private set; }
		public event Action ProjectChanged;
		public event Action<object> DataReceived;
		private Action<object, bool> send;
		private EditorContentLoader editorContent;

		private void OnContentUpdated(ContentType type, string name)
		{
			editorContent.RefreshMetaData();
			if (ContentUpdated != null)
				ContentUpdated(type, name);
		}

		public event Action<ContentType, string> ContentUpdated;

		private void OnContentDeleted(string contentName)
		{
			editorContent.RefreshMetaData();
			if (ContentDeleted != null)
				ContentDeleted(contentName);
		}

		public event Action<string> ContentDeleted;

		private void OnContentReady()
		{
			if (ContentReady != null)
				ContentReady();
		}

		public event Action ContentReady;

		public void RequestChangeProject(string newProjectName)
		{
			if (newProjectName.Compare(ProjectName))
				return;
			send(new ChangeProjectRequest(newProjectName), true);
		}

		public void Send(object message, bool allowToCompressMessage = true)
		{
			send(message, allowToCompressMessage);
		}

		public IEnumerable<string> GetAllContentNames()
		{
			return editorContent.GetAllNames();
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			return editorContent.GetAllNamesByType(type);
		}

		public ContentType? GetTypeOfContent(string content)
		{
			return editorContent.GetTypeOfContent(content);
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null)
		{
			editorContent.Upload(metaData, optionalFileData);
		}

		public void DeleteContent(string contentName, bool deleteSubContent = false)
		{
			editorContent.Delete(contentName, deleteSubContent);
		}

		public void StartPlugin(Type plugin)
		{
			if (StartEditorPlugin != null)
				StartEditorPlugin(plugin);
		}

		public event Action<Type> StartEditorPlugin;

		public EditorOpenTkViewport Viewport { get; set; }
	}
}