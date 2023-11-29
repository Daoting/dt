using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DtChineseAssist
{
    class ChineseCompletionSource : IAsyncCompletionSource
    {
        static ImmutableArray<CompletionFilter> _filters = ImmutableArray.Create(
            new CompletionFilter(
                "简拼",
                "j",
                new ImageElement(new ImageId(new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"), 2436), "简拼")));

        readonly ITextStructureNavigatorSelectorService _textNavigator;
        readonly IAsyncCompletionSource _sharpSource;

        public ChineseCompletionSource(ITextStructureNavigatorSelectorService p_textNavigator, IAsyncCompletionSource p_sharpSource)
        {
            _textNavigator = p_textNavigator;
            _sharpSource = p_sharpSource;
        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
        {
            var tokenSpan = FindTokenSpanAtPosition(triggerLocation);
            return new CompletionStartData(CompletionParticipation.ProvidesItems, tokenSpan);
        }

        SnapshotSpan FindTokenSpanAtPosition(SnapshotPoint triggerLocation)
        {
            ITextStructureNavigator navigator = _textNavigator.GetTextStructureNavigator(triggerLocation.Snapshot.TextBuffer);
            TextExtent extent = navigator.GetExtentOfWord(triggerLocation);
            if (triggerLocation.Position > 0 && (!extent.IsSignificant || !extent.Span.GetText().Any(c => char.IsLetterOrDigit(c))))
            {
                extent = navigator.GetExtentOfWord(triggerLocation - 1);
            }
            var tokenSpan = triggerLocation.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
            var snapshot = triggerLocation.Snapshot;
            var tokenText = tokenSpan.GetText(snapshot);
            if (string.IsNullOrWhiteSpace(tokenText))
            {
                return new SnapshotSpan(triggerLocation, 0);
            }
            int startOffset = 0;
            int endOffset = 0;
            if (tokenText.Length > 0)
            {
                if (tokenText.StartsWith("\""))
                    startOffset = 1;
            }
            if (tokenText.Length - startOffset > 0)
            {
                if (tokenText.EndsWith("\"\r\n"))
                    endOffset = 3;
                else if (tokenText.EndsWith("\r\n"))
                    endOffset = 2;
                else if (tokenText.EndsWith("\"\n"))
                    endOffset = 2;
                else if (tokenText.EndsWith("\n"))
                    endOffset = 1;
                else if (tokenText.EndsWith("\""))
                    endOffset = 1;
            }
            return new SnapshotSpan(tokenSpan.GetStartPoint(snapshot) + startOffset, tokenSpan.GetEndPoint(snapshot) - endOffset);
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            if (_sharpSource == null || Kit.RecursionLock.Value)
                return null;

            try
            {
                Kit.RecursionLock.Value = true;
                CompletionContext context = await _sharpSource.GetCompletionContextAsync(session, trigger, triggerLocation, applicableToSpan, token);
                if (context.Items.Length == 0)
                    return null;

                var items = new ConcurrentBag<CompletionItem>();
                foreach (var ci in context.Items)
                {
                    // 其他命名空间的不处理，否则出现两遍相同的选项
                    if (!string.IsNullOrEmpty(ci.Suffix))
                        continue;

                    string rawStr = ci.InsertText;
                    var pinyin = Kit.GetPingYin(rawStr);
                    if (pinyin != null)
                    {
                        var item = new CompletionItem(
                                displayText: rawStr,
                                source: this,
                                icon: ci.Icon,
                                filters: _filters,
                                // 不加[]会显示两遍名称
                                suffix: $"[{pinyin}]",
                                insertText: rawStr,
                                sortText: pinyin,
                                filterText: pinyin,
                                automationText: ci.AutomationText,
                                attributeIcons: ci.AttributeIcons);
                        items.Add(item);
                    }
                }

                //Debug.WriteLine($"触发：{trigger.Reason}；共：{context.Items.Length}；中文：{items.Count}\r\n");
                return new CompletionContext(items.ToImmutableArray());
            }
            finally
            {
                Kit.RecursionLock.Value = false;
            }
        }

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
            return Task.FromResult((object)null);
        }
    }
}
