using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GestaoPedidos.Infrastructure.Persistence;

public class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    valor => valor,
    valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc));
