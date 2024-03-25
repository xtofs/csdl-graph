
namespace Csdl.Graph;

public record struct NodeDef(Property[] Properties, Association[] Associations, Element[] Elements)
{

    public readonly void Deconstruct(out Property[] properties, out Association[] associations, out Element[] elements)
    {
        properties = Properties ?? [];
        associations = Associations ?? [];
        elements = Elements ?? [];
    }
}


