namespace CompanyEmployees;

public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type? type)
    {
        if (typeof(CompanyResponseDto).IsAssignableFrom(type) ||
            typeof(IEnumerable<CompanyResponseDto>).IsAssignableFrom(type))
        {
            return base.CanWriteType(type);
        }
        return false;
    }

    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        HttpResponse response = context.HttpContext.Response;
        var buffer = new StringBuilder();
        if (context.Object is IEnumerable<CompanyResponseDto> companies)
        {
            foreach (var company in companies)
            {
                FormatCsv(buffer, company);
            }
        }
        else
        {
            FormatCsv(buffer, (CompanyResponseDto)context.Object!);
        }
        await response.WriteAsync(buffer.ToString());
    }

    private static void FormatCsv(StringBuilder buffer, CompanyResponseDto company)
    {
        buffer.AppendLine($"\"Company ID: {company.Id}\", \"Company Name: {company.Name}, " +
                          $"\"Company Address: {company.FullAddress}\"");
    }
}