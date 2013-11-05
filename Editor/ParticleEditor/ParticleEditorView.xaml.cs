﻿using System;
using System.Windows;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.MaterialEditor;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Interaction logic for ParticleEditorView.xaml
	/// </summary>
	public partial class ParticleEditorView : EditorPluginView
	{
		//ncrunch: no coverage start, relating arguments only
		public ParticleEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service setService)
		{
			service = setService;
			viewModel = new ParticleEditorViewModel(service);
			DataContext = viewModel;
			AttachToUpdateEvents();
			Messenger.Default.Send("ParticleEditor", "SetSelectedEditorPlugin");
		}

		public void Activate()
		{
			viewModel.Activate();
			Messenger.Default.Send("ParticleEditor", "SetSelectedEditorPlugin");
		}

		private void AttachToUpdateEvents()
		{
			service.ContentUpdated += (type, name) =>
			{
				Action updateAction = () => { viewModel.UpdateOnContentChange(type, name); };
				Dispatcher.Invoke(updateAction);
			};
			service.ContentDeleted += (name) =>
			{
				Action updateAction = () => { viewModel.UpdateOnContentDeletion(name); };
				Dispatcher.Invoke(updateAction);
			};
			service.ProjectChanged +=
				() => Dispatcher.Invoke(new Action(viewModel.ResetOnProjectChange));
		}

		private ParticleEditorViewModel viewModel;
		private Service service;

		public string ShortName
		{
			get { return "Particle Effects"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/ParticleEffect.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			viewModel.Save();
		}

		private void OpenMaterialEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(MaterialEditorView));
		}

		private void LoadEffect(object sender, RoutedEventArgs e)
		{
			viewModel.LoadEffect();
		}

		private void AddDefaultEmitter(object sender, RoutedEventArgs e)
		{
			viewModel.AddEmitterToSystem();
		}

		private void RemoveSelectedEmitter(object sender, RoutedEventArgs e)
		{
			viewModel.RemoveCurrentEmitterFromSystem();
		}

		private void ResetDefaultEffect(object sender, RoutedEventArgs e)
		{
			viewModel.ResetDefaultEffect();
		}

		private void AddEmitterFromContent(object sender, RoutedEventArgs e)
		{
			viewModel.ToggleLookingForExistingEmitters();
		}

		private void TryChooseTemplate(object sender, RoutedEventArgs e)
		{
			viewModel.ToggleLookingForTemplateEffect();
		}
	}
}