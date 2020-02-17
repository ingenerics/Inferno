using Xunit;

namespace Inferno.Core.Tests
{
    public class Class1Tests
    {
        [Fact]
        public void PassingTest()
        {
            var c = new Class1();

            Assert.Equal(4, c.AddOne(3));
        }

        [Fact(Skip = "Don't break the build")]
        public void FailingTest()
        {
            var c = new Class1();

            Assert.Equal(1, c.AddOne(3));
        }
    }
}
