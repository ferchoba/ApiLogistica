
namespace Logistica.Infrastructure.Parsers;

public class CsvParser : DelimitedTextParser
{
    public CsvParser(ILogger<CsvParser> logger) : base(logger)
    {
    }

    public override string FormatId => "CSV";
    protected override char Delimiter => ',';
    protected override string DateFormat => "yyyy-MM-dd";
    protected override bool HasHeader => true;
}
