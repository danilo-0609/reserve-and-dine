namespace BuildingBlocks.Application;

public sealed record BlobObject(Stream? Content, string? ContentType);