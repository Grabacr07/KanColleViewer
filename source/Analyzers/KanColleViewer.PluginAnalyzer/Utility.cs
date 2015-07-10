using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grabacr07.KanColleViewer.PluginAnalyzer
{
	public static class Utility
	{
		private static readonly string[] pluginFeatures = typeof(IPlugin).Assembly
			.DefinedTypes
			.Where(x => x.IsInterface)
			.Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == typeof(PluginFeatureAttribute).Name))
			.Select(x => x.Name)
			.ToArray();

		private static readonly string[] allPlugins = pluginFeatures.Concat(new[] { nameof(IPlugin) }).ToArray();

		private static readonly string[] metadataProperties = typeof(IPluginMetadata)
			.GetProperties()	//継承元は取得されないのでGuidは含まない
			.Select(x => x.Name)
			.ToArray();

		public static string[] GetMissingExports(this ClassDeclarationSyntax classDeclaration)
		{
			return classDeclaration.GetInterfaces()
				.Except(classDeclaration.GetExports())
				.ToArray();
		}

		private static string[] GetExports(this ClassDeclarationSyntax classDeclaration)
		{
			var expressions = classDeclaration.GetAttributes("Export")
				.Concat(classDeclaration.GetAttributes("InheritedExport"))
				.SelectMany(a => a.ArgumentList.Arguments)
				.Select(a => a.Expression)
				.OfType<TypeOfExpressionSyntax>();
			
			var found = new Queue<string>(allPlugins.Length);
			var typeNames = expressions.Select(e => e.Type.ToString());
			foreach (var t in typeNames)
			{
				foreach (var i in allPlugins)
				{
					if (t == i || t == $"Grabacr07.KanColleViewer.Composition.{i}")
						found.Enqueue(i);
				}
			}
			return found.ToArray();
		}

		public static string[] GetMissingInterfaces(this ClassDeclarationSyntax classDeclaration)
		{
			return classDeclaration.GetExports()
				.Except(classDeclaration.GetInterfaces())
				.ToArray();
		}

		private static string[] GetInterfaces(this ClassDeclarationSyntax classDeclaration)
		{
			var types = classDeclaration?.BaseList?.Types.ToArray();
			if (types == null)
				return new string[0];
			
			var found = new Queue<string>(allPlugins.Length);
			var typeNames = types.Select(x => x.ToString());
			foreach (var t in typeNames)
			{
				foreach (var i in allPlugins)
				{
					if (t == i || t == $"Grabacr07.KanColleViewer.Composition.{i}")
						found.Enqueue(i);
				}
			}
			return found.ToArray();
		}

		public static string[] GetMissingMetadatas(this ClassDeclarationSyntax classDeclaration)
		{
			var requireMetadata = classDeclaration.IsExportIPlugin();
			if (!requireMetadata) return new string[0];
			
			var defined = classDeclaration.GetMetadatas();
			return metadataProperties.Except(defined).ToArray();
		}

		public static bool IsExportIPlugin(this ClassDeclarationSyntax classDeclaration)
			=> classDeclaration.GetExports().Contains(nameof(IPlugin));

		private static string[] GetMetadatas(this ClassDeclarationSyntax classDeclaration)
		{
			return classDeclaration.GetAttributes("ExportMetadata")
				.Select(a => a.ArgumentList.Arguments.First().ToString().Replace("\"", ""))
				.ToArray();
		}
		
		public static bool IsPluginClass(this ClassDeclarationSyntax classDeclaration)
		{
			return classDeclaration.GetExports()
				.Intersect(allPlugins)
				.Any();
		}

		public static bool RequireGuidMetadata(this ClassDeclarationSyntax classDeclaration)
		{
			var isRequired = classDeclaration.IsPluginClass();
			var isDefined = classDeclaration.GetMetadatas().Any(x => x == nameof(IPluginGuid.Guid));
			return isRequired && !isDefined;
		}

		public static string GetGuidMetadataValue(this ClassDeclarationSyntax classDeclaration)
		{
			return classDeclaration.GetAttributes("ExportMetadata")
				.FirstOrDefault(a => a.ArgumentList.Arguments[0].ToString() == $"\"{nameof(IPluginGuid.Guid)}\"")
				?.ArgumentList.Arguments[1].ToString().Replace("\"", "");
		}

		public static IEnumerable<AttributeSyntax> GetAttributes(this ClassDeclarationSyntax classDeclaration, string attributeName)
		{
			return classDeclaration.AttributeLists
				.SelectMany(a => a.Attributes)
				.Where(a =>
				{
					var name = a.Name.ToString();
					return name == attributeName || name == $"{attributeName}Attribute";
				});
		}

		public static TSyntax[] FindSyntax<TSyntax>(this CompilationUnitSyntax root)
		{
			return root.DescendantNodes().OfType<TSyntax>().ToArray();
		}

		public static string NewGuidValueForTest { get; set; }

		public static string NewGuidValue()
		{
			return NewGuidValueForTest ?? Guid.NewGuid().ToString().ToUpper();
		}
	}
}
