namespace GestaoPedidos.Application.Common;

public class NotFoundException(string recurso, Guid id) : Exception($"{recurso} '{id}' não encontrado.");
