using MessagePack;

namespace Boo.Common.Content;

[MessagePackObject]
public readonly struct ShaderData
{
    [Key(0)]
    public string Id { get; init; }

    [Key(1)]
    public byte[] VertexShader { get; init; }

    [Key(2)]
    public byte[] FragmentShader { get; init; }

    [Key(3)]
    public string[] Samplers { get; init; }

    [Key(4)]
    public string[] Params { get; init; }
}