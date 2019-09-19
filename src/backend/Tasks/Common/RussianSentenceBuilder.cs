using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace Tasks
{
    public class RussianSentenceBuilder : SentenceBuilder
    {
        public override Func<string> RequiredWord { get; }
            = () => "Обязательный.";

        public override Func<string> ErrorsHeadingText { get; }
            = () => "Ошибка(-и):";

        public override Func<string> UsageHeadingText { get; }
            = () => "Использование:";

        public override Func<bool, string> HelpCommandText { get; }
            = isOption => isOption
                ? "Отображает эту справку"
                : "Отображает подробную информацию о конкретной команде";

        public override Func<bool, string> VersionCommandText { get; }
            = _ => "Отображает информацию о версии";

        public override Func<Error, string> FormatError { get; }
            = e => "";

        public override Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors { get; }
            = e => "";
    }
}
