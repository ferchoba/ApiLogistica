
namespace Logistica.Infrastructure.Parsers
{
    public class TxtParser : DelimitedTextParser
    {
        public TxtParser(ILogger<TxtParser> logger) : base(logger)
        {
        }

        public override string FormatId => "TXT";
        protected override char Delimiter => '|';
        protected override string DateFormat => "yyyyMMdd";
        protected override bool HasHeader => false;
    }
}
