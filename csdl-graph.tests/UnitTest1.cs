namespace Csdl.Graph.Tests;

public class UnitTest1
{
    [Fact]
    public void Empty()
    {
        Assert.True(true);
    }

    [Theory]
    [InlineData("self.EntityContainer/MyEntitySet", new string[] { "self", "EntityContainer", "MyEntitySet" })]
    [InlineData("org.example.Manager", new string[] { "org.example", "Manager" })]
    [InlineData("org.example.Manager/name@Core.Description", new string[] { "org.example", "Manager", "name", "@Core.Description" })]
    [InlineData("org.example.Manager/@Core.Description", new string[] { "org.example", "Manager", "@Core.Description" })]
    [InlineData("org.example.EntityContainer/Addresses/Street", new string[] { "org.example", "EntityContainer", "Addresses", "Street" })]
    [InlineData("org.example.EntityContainer/Items@Capabilities.SortRestrictions", new string[] { "org.example", "EntityContainer", "Items", "@Capabilities.SortRestrictions" })]
    [InlineData("org.example.EntityContainer/Items@Capabilities.InsertRestrictions/Insertable", new string[] { "org.example", "EntityContainer", "Items", "@Capabilities.InsertRestrictions", "Insertable" })]

    public void TestSplit(string path, string[] expected)
    {
        var actual = Csdl.Graph.ModelPath.Split(path);
        Assert.Equal(expected, actual);
    }
}
