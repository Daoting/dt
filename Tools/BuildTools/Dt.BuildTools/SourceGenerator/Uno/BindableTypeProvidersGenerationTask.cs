#nullable enable

using Uno.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;
using Uno.Roslyn;
using Uno.UI.SourceGenerators.XamlGenerator;
using Uno.UI.SourceGenerators.Helpers;
using System.Xml;
using System.Threading;
using System.Collections.Concurrent;





#if NETFRAMEWORK
using Uno.SourceGeneration;
#endif

namespace Uno.UI.SourceGenerators.BindableTypeProviders
{
#if NETFRAMEWORK
	[GenerateAfter("Uno.ImmutableGenerator")]
#endif
	[Generator]
	public class BindableTypeProvidersSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			DependenciesInitializer.Init();
		}

		public void Execute(GeneratorExecutionContext context)
		{
			new Generator().Generate(context);
		}

		class Generator
		{
			private string? _defaultNamespace;

			private readonly Dictionary<INamedTypeSymbol, GeneratedTypeInfo> _typeMap = new Dictionary<INamedTypeSymbol, GeneratedTypeInfo>();
			private readonly Dictionary<string, (string type, List<string> members)> _substitutions = new Dictionary<string, (string type, List<string> members)>();
			private INamedTypeSymbol[]? _bindableAttributeSymbol;
			private ITypeSymbol? _dependencyPropertySymbol;
			private INamedTypeSymbol? _dependencyObjectSymbol;
			private INamedTypeSymbol? _javaObjectSymbol;
			private INamedTypeSymbol? _nsObjectSymbol;
			private INamedTypeSymbol? _nonBindableSymbol;
			private INamedTypeSymbol? _resourceDictionarySymbol;
			private IModuleSymbol? _currentModule;
			private IReadOnlyDictionary<string, INamedTypeSymbol[]>? _namedSymbolsLookup;
			private string? _projectFullPath;
			private string? _projectDirectory;
			private string? _baseIntermediateOutputPath;
			private string? _intermediatePath;
			private string? _assemblyName;
			private bool _xamlResourcesTrimming;
			private CancellationToken _cancellationToken;

			public string[] AnalyzerSuppressions { get; set; } = Array.Empty<string>();

			internal void Generate(GeneratorExecutionContext context)
			{
				try
				{
					var validPlatform = PlatformHelper.IsValidPlatform(context);
					var isDesignTime = DesignTimeHelper.IsDesignTime(context);
					var isApplication = Helpers.IsApplication(context);

					if (validPlatform
						&& !isDesignTime)
					{
						if (isApplication)
						{
							_cancellationToken = context.CancellationToken;

							_projectFullPath = context.GetMSBuildPropertyValue("MSBuildProjectFullPath");
							_projectDirectory = Path.GetDirectoryName(_projectFullPath)
								?? throw new InvalidOperationException($"MSBuild property MSBuildProjectFullPath value {_projectFullPath} is not valid");

							// Defaults to 'false'
							_ = bool.TryParse(context.GetMSBuildPropertyValue("UnoXamlResourcesTrimming"), out _xamlResourcesTrimming);

							_baseIntermediateOutputPath = context.GetMSBuildPropertyValue("BaseIntermediateOutputPath");
							_intermediatePath = Path.Combine(
								_projectDirectory,
								_baseIntermediateOutputPath
							);
							_assemblyName = context.GetMSBuildPropertyValue("AssemblyName");

							_defaultNamespace = context.GetMSBuildPropertyValue("RootNamespace");
							_namedSymbolsLookup = context.Compilation.GetSymbolNameLookup();

							_bindableAttributeSymbol = FindBindableAttributes();
							_dependencyPropertySymbol = context.Compilation.GetTypeByMetadataName(XamlConstants.Types.DependencyProperty);
							_dependencyObjectSymbol = context.Compilation.GetTypeByMetadataName(XamlConstants.Types.DependencyObject);

							_javaObjectSymbol = context.Compilation.GetTypeByMetadataName("Java.Lang.Object");
							_nsObjectSymbol = context.Compilation.GetTypeByMetadataName("Foundation.NSObject");
							_nonBindableSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.Data.NonBindableAttribute");
							_resourceDictionarySymbol = context.Compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.ResourceDictionary");
							_currentModule = context.Compilation.SourceModule;

							var modules = from ext in context.Compilation.ExternalReferences
										  let sym = context.Compilation.GetAssemblyOrModuleSymbol(ext) as IAssemblySymbol
										  where sym != null
										  from module in sym.Modules
										  select module;

							modules = modules.Concat(context.Compilation.SourceModule);

							context.AddSource("BindableMetadata", GenerateTypeProviders(modules));

							GenerateLinkerSubstitutionDefinition();
						}
						else
						{
							context.AddSource("BindableMetadata", $"// validPlatform: {validPlatform} designTime:{isDesignTime} isApplication:{isApplication}");
						}
					}
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception e)
				{
					string? message = e.Message + e.StackTrace;

					if (e is AggregateException)
					{
						message = (e as AggregateException)?.InnerExceptions.Select(ex => ex.Message + e.StackTrace).JoinBy("\r\n");
					}

#if NETSTANDARD
					var diagnostic = Diagnostic.Create(
						XamlCodeGenerationDiagnostics.GenericXamlErrorRule,
						null,
						$"Failed to generate type providers. ({e.Message})");

					context.ReportDiagnostic(diagnostic);
#else
					Console.WriteLine("Failed to generate type providers.", new Exception("Failed to generate type providers." + message, e));
#endif
				}
			}

			private INamedTypeSymbol[] FindBindableAttributes() =>
				_namedSymbolsLookup!.TryGetValue("BindableAttribute", out var types) ? types : Array.Empty<INamedTypeSymbol>();

			private string GenerateTypeProviders(IEnumerable<IModuleSymbol> modules)
			{
				var q = from module in modules
						from type in module.GlobalNamespace.GetNamespaceTypes()
						where (
							(_bindableAttributeSymbol?.Any(s => type.FindAttributeFlattened(s) != null) ?? false)
							&& !type.IsGenericType
							&& !type.IsAbstract
							&& IsValidProvider(type)
						)
						select type;

				q = q.ToArray();

				var writer = new IndentedStringBuilder();

				writer.AppendLineInvariant("// <auto-generated>");
				writer.AppendLineInvariant("// *****************************************************************************");
				writer.AppendLineInvariant("// This file has been generated by Uno.UI (BindableTypeProvidersSourceGenerator)");
				writer.AppendLineInvariant("// *****************************************************************************");
				writer.AppendLineInvariant("// </auto-generated>");
				writer.AppendLine();
				writer.AppendLineInvariant("#pragma warning disable 618  // Ignore obsolete members warnings");
				writer.AppendLineInvariant("#pragma warning disable 1591 // Ignore missing XML comment warnings");
				writer.AppendLineInvariant("#pragma warning disable Uno0001 // Ignore not implemented members");
				writer.AppendLineInvariant("using System;");
				writer.AppendLineInvariant("using System.Linq;");
				writer.AppendLineInvariant("using System.Diagnostics;");

				using (writer.BlockInvariant("namespace {0}", _defaultNamespace))
				{
					GenerateMetadata(writer, q);
					GenerateProviderTable(q, writer);
				}

				return writer.ToString();
			}

			private bool IsValidProvider(INamedTypeSymbol type)
				=> type.IsLocallyPublic(_currentModule!)

				// Exclude resource dictionaries for linking constraints (XamlControlsResources in particular)
				// Those are not databound, so there's no need to generate providers for them.
				&& !type.Is(_resourceDictionarySymbol);

			private void GenerateProviderTable(IEnumerable<INamedTypeSymbol> q, IndentedStringBuilder writer)
			{
				writer.AppendLineInvariant("[System.Runtime.CompilerServices.CompilerGeneratedAttribute]");
				writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1502:AvoidExcessiveComplexity\", Justification=\"Must be ignored even if generated code is checked.\")]");
				writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1506:AvoidExcessiveClassCoupling\", Justification = \"Must be ignored even if generated code is checked.\")]");
				AnalyzerSuppressionsGenerator.Generate(writer, AnalyzerSuppressions);
				using (writer.BlockInvariant("public class BindableMetadataProvider : global::Uno.UI.DataBinding.IBindableMetadataProvider"))
				{
					writer.AppendLineInvariant(@"static global::System.Collections.Hashtable _bindableTypeCacheByFullName = new global::System.Collections.Hashtable({0});", q.Count());

					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1502:AvoidExcessiveComplexity\", Justification=\"Must be ignored even if generated code is checked.\")]");
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1506:AvoidExcessiveClassCoupling\", Justification = \"Must be ignored even if generated code is checked.\")]");
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1505:AvoidUnmaintainableCode\", Justification = \"Must be ignored even if generated code is checked.\")]");

					writer.AppendLineInvariant("private delegate global::Uno.UI.DataBinding.IBindableType TypeBuilderDelegate();");

					using (writer.BlockInvariant("private static TypeBuilderDelegate CreateMemoized(TypeBuilderDelegate builder)"))
					{
						writer.AppendLineInvariant(@"global::Uno.UI.DataBinding.IBindableType value = null;
						return () => {{
							if (value == null)
							{{
								value = builder();
							}}

							return value;
						}};"
						);
					}

					GenerateTypeTable(writer);

					writer.AppendLineInvariant(@"#if DEBUG");
					writer.AppendLineInvariant(@"private static global::System.Collections.Generic.List<global::System.Type> _knownMissingTypes = new global::System.Collections.Generic.List<global::System.Type>();");
					writer.AppendLineInvariant(@"#endif");

					//writer.AppendLineInvariant(@"#if DEBUG");
					//writer.AppendLineInvariant(@"private static global::System.Collections.Concurrent.ConcurrentDictionary<global::System.Type, string> _knownMissingTypes = new global::System.Collections.Concurrent.ConcurrentDictionary<global::System.Type, string>();");
					//writer.AppendLineInvariant(@"#endif");

					using (writer.BlockInvariant("public void ForceInitialize()"))
					{
						using (writer.BlockInvariant(@"foreach (TypeBuilderDelegate item in _bindableTypeCacheByFullName.Values)"))
						{
							writer.AppendLineInvariant(@"item();");
						}
					}
				}
			}

			private class PropertyNameEqualityComparer : IEqualityComparer<IPropertySymbol>
			{
				public bool Equals(IPropertySymbol? x, IPropertySymbol? y)
				{
					return x?.Name == y?.Name;
				}

				public int GetHashCode(IPropertySymbol obj)
				{
					return obj.Name.GetHashCode();
				}
			}

			private void GenerateMetadata(IndentedStringBuilder writer, IEnumerable<INamedTypeSymbol> types)
			{
				foreach (var type in types)
				{
					GenerateType(writer, type);
				}
			}

			class GeneratedTypeInfo
			{
				public GeneratedTypeInfo(int index, bool hasProperties)
				{
					Index = index;
					HasProperties = hasProperties;
				}

				public int Index { get; }

				public bool HasProperties { get;  }
			}

			private void GenerateType(IndentedStringBuilder writer, INamedTypeSymbol ownerType)
			{
				_cancellationToken.ThrowIfCancellationRequested();

				if (_typeMap.ContainsKey(ownerType))
				{
					return;
				}

				var ownerTypeName = GetGlobalQualifier(ownerType) + ownerType.ToString();

				var flattenedProperties =
					from property in ownerType.GetAllProperties()
					where !property.IsStatic
						&& !(property.ContainingSymbol is INamedTypeSymbol containing && IsIgnoredType(containing))
						&& !IsNonBindable(property)
						&& !IsOverride(property.GetMethod)
					select property;

				var properties =
					from property in ownerType.GetProperties()
					where !property.IsStatic
						&& !IsNonBindable(property)
						&& HasPublicGetter(property)
						&& !IsOverride(property.GetMethod)
					select property;

				var propertyDependencyProperties =
					from property in ownerType.GetProperties()
					where property.IsStatic
						&& SymbolEqualityComparer.Default.Equals(property.Type, _dependencyPropertySymbol)
					select property.Name;

				var fieldDependencyProperties =
					from field in ownerType.GetFields()
					where field.IsStatic
						&& SymbolEqualityComparer.Default.Equals(field.Type, _dependencyPropertySymbol)
					select field.Name;

				var dependencyProperties = fieldDependencyProperties
					.Concat(propertyDependencyProperties)
					.ToArray();

				properties = from prop in properties.Distinct(new PropertyNameEqualityComparer())
							 where !dependencyProperties.Contains(prop.Name + "Property")
							 select prop;

				properties = properties
					.ToArray();

				var typeInfo = new GeneratedTypeInfo(
					index: _typeMap.Count,
					hasProperties: properties.Any() || dependencyProperties.Any()
				);

				_typeMap.Add(ownerType, typeInfo);

				var baseType = GetBaseType(ownerType);

				// Call the builders for the base types
				if (baseType != null)
				{
					GenerateType(writer, baseType);
				}

				writer.AppendLineInvariant("/// <summary>");
				writer.AppendLineInvariant("/// Builder for {0}", ownerType.GetFullName());
				writer.AppendLineInvariant("/// </summary>");
				writer.AppendLineInvariant("[System.Runtime.CompilerServices.CompilerGeneratedAttribute]");
				writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1502:AvoidExcessiveComplexity\", Justification=\"Must be ignored even if generated code is checked.\")]");
				writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1506:AvoidExcessiveClassCoupling\", Justification = \"Must be ignored even if generated code is checked.\")]");
				AnalyzerSuppressionsGenerator.Generate(writer, AnalyzerSuppressions);
				using (writer.BlockInvariant("static class MetadataBuilder_{0:000}", typeInfo.Index))
				{
					var postWriter = new IndentedStringBuilder();
					postWriter.Indent(writer.CurrentLevel);

					// Generate a parameter-less build to avoid generating a lambda during registration (avoids creating a caching backing field)
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1502:AvoidExcessiveComplexity\", Justification=\"Must be ignored even if generated code is checked.\")]");
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1506:AvoidExcessiveClassCoupling\", Justification = \"Must be ignored even if generated code is checked.\")]");
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1505:AvoidUnmaintainableCode\", Justification = \"Must be ignored even if generated code is checked.\")]");
					using (writer.BlockInvariant("internal static global::Uno.UI.DataBinding.IBindableType Build()"))
					{
						writer.AppendLineInvariant("return Build(null);");
					}

					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1502:AvoidExcessiveComplexity\", Justification=\"Must be ignored even if generated code is checked.\")]");
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1506:AvoidExcessiveClassCoupling\", Justification = \"Must be ignored even if generated code is checked.\")]");
					writer.AppendLineInvariant("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Microsoft.Maintainability\", \"CA1505:AvoidUnmaintainableCode\", Justification = \"Must be ignored even if generated code is checked.\")]");
					using (writer.BlockInvariant("internal static global::Uno.UI.DataBinding.IBindableType Build(global::Uno.UI.DataBinding.BindableType parent)"))
					{
						RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, "Uno.UI.DataBinding.IBindableType Build(Uno.UI.DataBinding.BindableType)");

						writer.AppendLineInvariant(
							@"var bindableType = parent ?? new global::Uno.UI.DataBinding.BindableType({0}, typeof({1}));",
							flattenedProperties
								.Where(p => !IsStringIndexer(p) && HasPublicGetter(p))
								.Count()
							, ExpandType(ownerType)
						);

						// Call the builders for the base types
						if (baseType != null)
						{
							var baseTypeMapped = _typeMap.UnoGetValueOrDefault(baseType);

							writer.AppendLineInvariant(@"MetadataBuilder_{0:000}.Build(bindableType); // {1}", baseTypeMapped.Index, ExpandType(baseType));
						}

						var ctor = ownerType.GetMethods().FirstOrDefault(m => m.MethodKind == MethodKind.Constructor && !m.Parameters.Any() && m.IsLocallyPublic(_currentModule!));

						if (ctor != null && IsCreateable(ownerType))
						{
							using (writer.BlockInvariant("if(parent == null)"))
							{
								writer.AppendLineInvariant(@"bindableType.AddActivator(CreateInstance);");
								postWriter.AppendLineInvariant($@"private static object CreateInstance() => new {ownerTypeName}();");

								RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, "System.Object CreateInstance()");
							}
						}

						foreach (var property in properties)
						{
							var propertyTypeName = property.Type.GetFullyQualifiedType();
							var propertyName = property.Name;

							if (IsStringIndexer(property))
							{
								writer.AppendLineInvariant("bindableType.AddIndexer(GetIndexer, SetIndexer);");

								postWriter.AppendLineInvariant($@"private static object GetIndexer(object instance, string name) => (({ownerTypeName})instance)[name];");
								RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, "System.Object GetIndexer(System.Object, System.String)");

								if (property.SetMethod != null)
								{
									postWriter.AppendLineInvariant($@"private static void SetIndexer(object instance, string name, object value) => (({ownerTypeName})instance)[name] = ({propertyTypeName})value;");
									RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, "System.Void SetIndexer(System.Object,System.String,System.Object)");
								}
								else
								{
									postWriter.AppendLineInvariant("private static void SetIndexer(object instance, string name, object value) {{}}");
								}

								continue;
							}

							if (property.IsIndexer)
							{
								// Other types of indexers are currently not supported.
								continue;
							}

							if (
								property.SetMethod != null
								&& property.SetMethod != null
								&& property.SetMethod.IsLocallyPublic(_currentModule!)
								)
							{
								if (property.Type.IsValueType)
								{
									writer.AppendLineInvariant($@"bindableType.AddProperty(""{propertyName}"", typeof({propertyTypeName}), Get{propertyName}, Set{propertyName});");

									postWriter.AppendLineInvariant($@"private static object Get{propertyName}(object instance, Microsoft.UI.Xaml.DependencyPropertyValuePrecedences? precedence) => (({ownerTypeName})instance).{propertyName};");

									using (postWriter.BlockInvariant($@"private static void Set{propertyName}(object instance, object value, Microsoft.UI.Xaml.DependencyPropertyValuePrecedences? precedence)"))
									{
										using (postWriter.BlockInvariant($"if(value != null)"))
										{
											postWriter.AppendLineInvariant($"(({ownerTypeName})instance).{propertyName} = ({propertyTypeName})value;");
										}
									}
								}
								else
								{
									writer.AppendLineInvariant($@"bindableType.AddProperty(""{propertyName}"", typeof({propertyTypeName}), Get{propertyName}, Set{propertyName});");

									postWriter.AppendLineInvariant($@"private static object Get{propertyName}(object instance,  Microsoft.UI.Xaml.DependencyPropertyValuePrecedences? precedence) => (({ownerTypeName})instance).{propertyName};");
									postWriter.AppendLineInvariant($@"private static void Set{propertyName}(object instance, object value, Microsoft.UI.Xaml.DependencyPropertyValuePrecedences? precedence) => (({ownerTypeName})instance).{propertyName} = ({propertyTypeName})value;");
								}

								RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, $"System.Object Get{propertyName}(System.Object,System.Nullable`1<Microsoft.UI.Xaml.DependencyPropertyValuePrecedences>)");
								RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, $"System.Void Set{propertyName}(System.Object,System.Object,System.Nullable`1<Microsoft.UI.Xaml.DependencyPropertyValuePrecedences>)");

							}
							else if (HasPublicGetter(property))
							{
								writer.AppendLineInvariant($@"bindableType.AddProperty(""{propertyName}"", typeof({propertyTypeName}), Get{propertyName});");

								postWriter.AppendLineInvariant($@"private static object Get{propertyName}(object instance, Microsoft.UI.Xaml.DependencyPropertyValuePrecedences? precedence) => (({ownerTypeName})instance).{propertyName};");

								RegisterHintMethod($"MetadataBuilder_{typeInfo.Index:000}", ownerType, $"System.Object Get{propertyName}(System.Object,System.Nullable`1<Microsoft.UI.Xaml.DependencyPropertyValuePrecedences>)");
							}
						}

						foreach (var dependencyProperty in dependencyProperties)
						{
							var propertyName = dependencyProperty.TrimEnd("Property");

							var getMethod = ownerType.GetMethods().FirstOrDefault(m => m.Name == "Get" + propertyName && m.Parameters.Length == 1 && m.IsLocallyPublic(_currentModule!));

							if (getMethod == null)
							{
								getMethod = ownerType
									.GetProperties()
									.FirstOrDefault(p => p.Name == propertyName && (p.GetMethod?.IsLocallyPublic(_currentModule!) ?? false))
									?.GetMethod;
							}

							if (getMethod != null)
							{
								writer.AppendLineInvariant($@"bindableType.AddProperty({ownerType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{dependencyProperty});");
							}
						}

						writer.AppendLineInvariant(@"return bindableType;");
					}

					writer.Append(postWriter.ToString());
				}

				writer.AppendLine();
			}

			private void RegisterHintMethod(string type, INamedTypeSymbol targetType, string signature)
			{
				type = _defaultNamespace + "." + type;

				if (!_substitutions.TryGetValue(type, out var hint))
				{
					_substitutions[type] = hint = (LinkerHintsHelpers.GetPropertyAvailableName(targetType.ToDisplayString()), new List<string>());
				}

				hint.members.Add(signature);
			}

			private static string ExpandType(INamedTypeSymbol ownerType)
			{
				if (ownerType.TypeKind == TypeKind.Error)
				{
					return ownerType.ToString() + "/* Type is error */";
				}
				else
				{
					return GetGlobalQualifier(ownerType) + ownerType.GetFullName();
				}
			}

			private static string GetGlobalQualifier(ITypeSymbol ownerType)
			{
				if (ownerType is IArrayTypeSymbol arrayType)
				{
					return GetGlobalQualifier(arrayType.ElementType);
				}

				if (ownerType.IsNullable(out var nullType))
				{
					return GetGlobalQualifier(nullType!);
				}

				var needsGlobal = ownerType.SpecialType == SpecialType.None && !ownerType.IsTupleType;
				return (needsGlobal ? "global::" : "");
			}

			private bool IsCreateable(INamedTypeSymbol type)
			{
				return !type.IsAbstract
					&& type
						.GetMethods()
						.Safe()
						.Any(m =>
							m.MethodKind == MethodKind.Constructor
							&& m.IsLocallyPublic(_currentModule!)
							&& m.Parameters.Safe().None()
						);
			}

			private INamedTypeSymbol? GetBaseType(INamedTypeSymbol type)
			{
				if (type.BaseType != null)
				{
					var ignoredByConfig = IsIgnoredType(type.BaseType);

					// These types are know to not be bindable, so ignore them by default.
					var isKnownBaseType = type.BaseType.SpecialType == SpecialType.System_Object
						|| SymbolEqualityComparer.Default.Equals(type.BaseType, _javaObjectSymbol)
						|| SymbolEqualityComparer.Default.Equals(type.BaseType, _nsObjectSymbol);

					if (!ignoredByConfig && !isKnownBaseType)
					{
						return type.BaseType;
					}
					else
					{
						return GetBaseType(type.BaseType);
					}
				}

				return null;
			}

			private bool IsIgnoredType(INamedTypeSymbol typeSymbol)
			{
				return typeSymbol.IsGenericType;
			}

			private bool HasPublicGetter(IPropertySymbol property) => property.GetMethod?.IsLocallyPublic(_currentModule!) ?? false;

			private bool IsStringIndexer(IPropertySymbol property)
			{
				return property.IsIndexer
					&& property.GetMethod!.IsLocallyPublic(_currentModule!)
					&& property.Parameters.Length == 1
					&& property.Parameters.Any(p => p.Type.SpecialType == SpecialType.System_String);
			}

			private bool IsNonBindable(IPropertySymbol property) => property.FindAttributeFlattened(_nonBindableSymbol!) != null;

			private bool IsOverride(IMethodSymbol? methodDefinition)
			{
				return methodDefinition != null
					&& methodDefinition.IsOverride
					&& !methodDefinition.IsVirtual;
			}

			private void GenerateTypeTable(IndentedStringBuilder writer)
			{
				foreach (var type in _typeMap.Where(k => !k.Key.IsGenericType))
				{
					writer.AppendLineInvariant($"private global::Uno.UI.DataBinding.IBindableType _bindableType{type.Value.Index:000};");
				}

				using (writer.BlockInvariant("public global::Uno.UI.DataBinding.IBindableType GetBindableTypeByFullName(string fullName)"))
				{
					using (writer.BlockInvariant(@"switch(fullName)"))
					{
						foreach (var type in _typeMap.Where(k => !k.Key.IsGenericType))
						{
							_cancellationToken.ThrowIfCancellationRequested();

							var typeIndexString = $"{type.Value.Index:000}";

							writer.AppendLineInvariant($"case \"{type.Key}\":");
							using (writer.BlockInvariant($"if(_bindableType{typeIndexString} == null)"))
							{
								if (_xamlResourcesTrimming && type.Key.GetAllInterfaces().Any(i => SymbolEqualityComparer.Default.Equals(i, _dependencyObjectSymbol)))
								{
									var linkerHintsClassName = LinkerHintsHelpers.GetLinkerHintsClassName(_defaultNamespace);
									var safeTypeName = LinkerHintsHelpers.GetPropertyAvailableName(type.Key.GetFullMetadataName());

									writer.AppendLineInvariant($"if(global::{linkerHintsClassName}.{safeTypeName})");
								}

								writer.AppendLineInvariant($"_bindableType{typeIndexString} = MetadataBuilder_{typeIndexString}.Build();");
							}

							writer.AppendLineInvariant($"return _bindableType{typeIndexString};");
						}

						writer.AppendLineInvariant("default:");
						writer.AppendLineInvariant(@"return null;");
					}
				}
				
				using (writer.BlockInvariant("public global::Uno.UI.DataBinding.IBindableType GetBindableTypeByType(Type type)"))
				{
					writer.AppendLineInvariant(@"var bindableType = GetBindableTypeByFullName(type.FullName);");

					writer.AppendLineInvariant(@"#if DEBUG");
					using (writer.BlockInvariant(@"lock(_knownMissingTypes)"))
					{
						using (writer.BlockInvariant(@"if(bindableType == null && !_knownMissingTypes.Contains(type) && !type.IsGenericType && !type.IsAbstract)"))
						{
							writer.AppendLineInvariant(@"_knownMissingTypes.Add(type);");
							writer.AppendLineInvariant(@"Debug.WriteLine($""The Bindable attribute is missing and the type [{{type.FullName}}] is not known by the MetadataProvider. Reflection was used instead of the binding engine and generated static metadata. Add the Bindable attribute to prevent this message and performance issues."");");
						}
					}
					writer.AppendLineInvariant(@"#endif");

					//writer.AppendLineInvariant(@"#if DEBUG");
					//using (writer.BlockInvariant(@"if(bindableType == null && !_knownMissingTypes.ContainsKey(type) && !type.IsGenericType && !type.IsAbstract)"))
					//{
					//	writer.AppendLineInvariant(@"_knownMissingTypes.TryAdd(type, null);");
					//	writer.AppendLineInvariant(@"Debug.WriteLine($""The Bindable attribute is missing and the type [{{type.FullName}}] is not known by the MetadataProvider. Reflection was used instead of the binding engine and generated static metadata. Add the Bindable attribute to prevent this message and performance issues."");");
					//}
					//writer.AppendLineInvariant(@"#endif");

					writer.AppendLineInvariant(@"return bindableType;");
				}

				using (writer.BlockInvariant("public BindableMetadataProvider()"))
				{
				}
			}

			private void GenerateLinkerSubstitutionDefinition()
			{
				if (!_xamlResourcesTrimming)
				{
					return;
				}

				// <linker>
				//   <assembly fullname="Uno.UI">
				// 	<type fullname="Uno.UI.GlobalStaticResources">
				// 	  <method signature="System.Void Initialize()" body="remove" />
				// 	  <method signature="System.Void RegisterDefaultStyles()" body="remove" />
				// 	</type>
				//   </assembly>
				// </linker>
				var doc = new XmlDocument();

				var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

				var root = doc.DocumentElement;
				doc.InsertBefore(xmlDeclaration, root);

				var linkerNode = doc.CreateElement(string.Empty, "linker", string.Empty);
				doc.AppendChild(linkerNode);

				var assemblyNode = doc.CreateElement(string.Empty, "assembly", string.Empty);
				assemblyNode.SetAttribute("fullname", _assemblyName);
				linkerNode.AppendChild(assemblyNode);


				foreach(var substitution in _substitutions)
				{
					var typeNode = doc.CreateElement(string.Empty, "type", string.Empty);
					typeNode.SetAttribute("fullname", substitution.Key);
					typeNode.SetAttribute("feature", substitution.Value.type);
					typeNode.SetAttribute("featurevalue", "false");
					assemblyNode.AppendChild(typeNode);

					foreach(var method in substitution.Value.members)
					{
						var methodNode = doc.CreateElement(string.Empty, "method", string.Empty);
						methodNode.SetAttribute("signature", method);
						methodNode.SetAttribute("body", "remove");
						typeNode.AppendChild(methodNode);
					}
				}

				var fileName = Path.Combine(_intermediatePath, "Substitutions", "BindableMetadata.Substitutions.xml");
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));

				doc.Save(fileName);
			}

		}
	}
}
