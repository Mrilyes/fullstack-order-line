using Xunit;

public class ExampleTests
{
    [Fact]
    public void Test_Addition()
    {
        // Arrange
        int a = 1;
        int b = 1;

        // Act
        int result = a + b;

        // Assert
        Assert.Equal(2, result);
    }
}