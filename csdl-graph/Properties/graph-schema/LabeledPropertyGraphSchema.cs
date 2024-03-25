
namespace Csdl.Graph;

public class LabeledPropertyGraphSchema : IEnumerable
{
    private readonly Dictionary<String, NodeDef> dictionary = [];

    public LabeledPropertyGraphSchema()
    {
    }

    public NodeDef this[string key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        dictionary.GetEnumerator();

    public void Display(TextWriter w)
    {
        foreach (var (name, def) in dictionary)
        {
            w.WriteLine("model element {0} {{", name);

            foreach (var (i, p) in def.Properties.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine("    // properties (of primitive values)");
                }

                w.WriteLine("    {0}: {1};", p.Name, p.Type.Name());
            }

            foreach (var (i, assoc) in def.Associations.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("    // references (associations) to other nodes");
                }
                switch (assoc)
                {
                    case Reference reference:
                        w.WriteLine("    {0}: Reference<{1}>;", assoc.Name, string.Join("|", reference.TypeAlternatives));
                        break;

                    case PathReference path:
                        w.WriteLine("    {0}: PathReference<{1}>;", assoc.Name, string.Join(",", path.Types));
                        break;
                }
            }

            // var multi = def.Contained?.Length > 1;
            foreach (var (i, c) in def.Elements.WidthIndex())
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("    // child (contained) nodes");
                }

                w.WriteLine("    {0}: Collection<{1}>;",
                    c.Name,
                    string.Join("|", c.TypeAlternatives));
            }

            w.WriteLine("}");
            w.WriteLine();
        }

        // w.WriteLine(new string('\n', 10));
        // w.WriteLine("type Box<T> = any");
        // w.WriteLine("type Collection<T> = any");
    }

    public override string ToString()
    {
        var writer = new StringWriter();
        Display(writer);
        return writer.ToString();
    }
}

