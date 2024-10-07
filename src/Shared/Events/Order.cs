namespace Shared.Events;

public record Order(int Id, List<Line> Lines);
