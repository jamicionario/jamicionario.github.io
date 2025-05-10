
using NodaTime;

namespace ScoresProcessor.Model;

public record class VersionInfo(int Version, Instant GenerationDate);
