using Logistica.Application.Dtos;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Logistica.Infrastructure.Parsers.Serialization;

[JsonSerializable(typeof(JsonDto))]
[JsonSerializable(typeof(IAsyncEnumerable<JsonDto>))]
public partial class JsonContext : JsonSerializerContext
{
}
