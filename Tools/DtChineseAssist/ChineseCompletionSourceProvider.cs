using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DtChineseAssist
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name("中文代码助手")]
    [ContentType("CSharp")]
    internal class ChineseCompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        IDictionary<ITextView, IAsyncCompletionSource> _cache = new Dictionary<ITextView, IAsyncCompletionSource>();

        [Import]
        ITextStructureNavigatorSelectorService _textNavigator;

        [ImportMany]
        Lazy<IAsyncCompletionSourceProvider>[] _lazyProviders;

        [Import]
        IAsyncCompletionBroker _broker;

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            if (_cache.TryGetValue(textView, out var itemSource))
                return itemSource;

            IAsyncCompletionSource sharpSource = GetSharpSource(textView);
            var source = new ChineseCompletionSource(_textNavigator, sharpSource);
            textView.Closed += (o, e) => _cache.Remove(textView);
            _cache.Add(textView, source);
            return source;
        }

        IAsyncCompletionSource GetSharpSource(ITextView textView)
        {
            if (_lazyProviders != null && _lazyProviders.Length > 0)
            {
                for (int i = 0; i < _lazyProviders.Length; i++)
                {
                    var provider = _lazyProviders[i];
                    if (provider.IsValueCreated)
                    {
                        Type type = provider.Value.GetType();
                        if (type.FullName == _providerType)
                        {
                            return provider.Value.GetOrCreate(textView);
                        }
                    }
                }
            }
            
            if (_broker != null)
            {
                var prvs = _broker.GetType().GetField("orderedCompletionSourceProviders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (prvs != null)
                {
                    IList list = prvs.GetValue(_broker) as IList;
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Lazy<IAsyncCompletionSourceProvider> provider = list[i] as Lazy<IAsyncCompletionSourceProvider>;
                            if (provider.IsValueCreated)
                            {
                                Type type = provider.Value.GetType();
                                if (type.FullName == _providerType)
                                {
                                    return provider.Value.GetOrCreate(textView);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        const string _providerType = "Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.AsyncCompletion.CompletionSourceProvider";
    }
}
