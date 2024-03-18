
public static class Path
{
    public static IEnumerable<string> Split(string path)
    {
        var fields = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var segments = fields.SelectMany(field => field.IndexOf('@') > 0 ? field.Split('@', 2).Select((s, i) => i == 0 ? s : "@" + s) : [field]);

        // Console.WriteLine("{0} => {1}", path, string.Join("; ", segments));

        return segments;
    }


    // [Theory]
    // [InlineData("/self.EntityContainer/MyEntitySet", new string[] { "", "self.EntityContainer", "MyEntitySet" })]
    // [InlineData("/org.example.Manager", new string[] { "", "org.example.Manager" })]
    // [InlineData("/org.example.Manager/name@Core.Description", new string[] { "", "org.example.Manager", "name", "@Core.Description" })]
    // [InlineData("/org.example.Manager/@Core.Description", new string[] { "", "org.example.Manager", "@Core.Description" })]
    // [InlineData("/org.example.EntityContainer/Addresses/Street", new string[] { "", "org.example.EntityContainer", "Addresses", "Street" })]
    // [InlineData("/org.example.EntityContainer/Items@Capabilities.SortRestrictions", new string[] { "", "org.example.EntityContainer", "Items", "@Capabilities.SortRestrictions" })]
    // [InlineData("/org.example.EntityContainer/Items@Capabilities.InsertRestrictions/Insertable", new string[] { "", "org.example.EntityContainer", "Items", "@Capabilities.InsertRestrictions", "Insertable" })]

    // public void TestSplit(string path, string[] expected)
    // {
    //     var actual = csdlGraph.Path.Split(path);
    //     Assert.Equal(actual, expected);
    // }
}