﻿using System;
using System.IO;
using System.Reflection;

namespace DeltaEngine.Extensions
{
	public static class PathExtensions
	{
		//ncrunch: no coverage start
		public static void CreateDirectoryIfNotExists(string fullFilePath)
		{
			string directory = Path.GetDirectoryName(fullFilePath);
			if (directory.Length > 0 && !Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}

		/// <summary>
		/// Reference: http://msdn.microsoft.com/en-us/library/y549e41e.aspx
		/// </summary>
		public static string GetDotNetFrameworkPath(Version runtimeVersion)
		{
			string frameworkVersion = runtimeVersion.Major + "." + runtimeVersion.Minor + "." +
				runtimeVersion.Build;
			return Path.Combine(GetDotNetPath(), "Framework", "v" + frameworkVersion);
		}

		private static string GetDotNetPath()
		{
			string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			return Path.Combine(windowsDirectory, "Microsoft.NET");
		}

		public static string GetFallbackEngineSourceCodeDirectory()
		{
			if (Directory.Exists(DefaultCodePath))
				return DefaultCodePath;
			const string DefaultDevelopmentPath = @"C:\Development\DeltaEngine";
			if (Directory.Exists(DefaultDevelopmentPath))
				return DefaultDevelopmentPath;
			throw new NoDeltaEngineFoundInFallbackPaths();
		}

		public const string DefaultCodePath = @"C:\Code\DeltaEngine";

		public class NoDeltaEngineFoundInFallbackPaths : Exception {}

		public static string GetDeltaEngineSolutionFilePath()
		{
			return Path.Combine(GetFallbackEngineSourceCodeDirectory(), "DeltaEngine.sln");
		}

		public static string GetExecutableDirectory()
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		public static bool IsDeltaEnginePathEnvironmentVariableAvailable()
		{
			return !String.IsNullOrEmpty(GetDeltaEngineInstalledDirectory());
		}

		public static string GetDeltaEngineInstalledDirectory()
		{
			return Environment.GetEnvironmentVariable(EnginePathEnvironmentVariableName);
		}

		public const string EnginePathEnvironmentVariableName = "DeltaEnginePath";

		public static string GetAbsolutePath(string directoryOrFilePath)
		{
			if (String.IsNullOrEmpty(directoryOrFilePath))
				return Environment.CurrentDirectory;
			return Path.GetFullPath(directoryOrFilePath);
		}
	}
}