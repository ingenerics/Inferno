using Inferno.Testing;
using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace Inferno.Reactive.Tests
{
    public class RxphNameOfTestFixture : TestFixture
    {
        [IgnoreDataMember]
        private readonly RxPropertyHelper<string> _firstThreeLettersOfOneWord;

        [IgnoreDataMember]
        private readonly RxPropertyHelper<string> _lastThreeLettersOfOneWord;

        public RxphNameOfTestFixture()
        {
            this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x).ToProperty(this, nameof(FirstThreeLettersOfOneWord), out _firstThreeLettersOfOneWord);

            _lastThreeLettersOfOneWord = this.WhenAnyValue(x => x.IsOnlyOneWord).Select(x => x ?? string.Empty).Select(x => x.Length >= 3 ? x.Substring(x.Length - 3, 3) : x).ToProperty(this, nameof(LastThreeLettersOfOneWord));
        }

        [IgnoreDataMember]
        public string FirstThreeLettersOfOneWord => _firstThreeLettersOfOneWord.Value;

        [IgnoreDataMember]
        public string LastThreeLettersOfOneWord => _lastThreeLettersOfOneWord.Value;
    }
}
