using Inferno.Testing;
using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace Inferno.Reactive.Tests
{
    public class RxphTestFixture : TestFixture
    {
        [IgnoreDataMember]
        private readonly RxPropertyHelper<string> _firstThreeLettersOfOneWord;

        public RxphTestFixture()
        {
            this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(this, x => x.FirstThreeLettersOfOneWord, out _firstThreeLettersOfOneWord);
        }

        [IgnoreDataMember]
        public string FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;
    }
}
