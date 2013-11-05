﻿namespace DeltaEngine.Editor.SampleBrowser.Helpers
{
	public class DesignSampleItem : Sample
	{
		public DesignSampleItem()
			: base(Name, SampleCategory.Test, Project, Exe, "Class", "Method") {}

		private const string Name =
			"TestMethodWithTheMaximumAmountOfLettersInItsNameToFitIntoTheDeltaEngineLineWidth";
		private const string Project = @"C:\Code\DeltaEngine\Samples\Asteroids\Asteroids.csproj";
		private const string Exe = @"C:\Code\DeltaEngine\Samples\Asteroids\bin\Debug\Asteroids.exe";
	}
}