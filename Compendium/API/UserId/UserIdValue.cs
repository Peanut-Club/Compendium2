using Common.Values;

namespace Compendium.API
{
    public class UserIdValue : IGetValue<string>
    {
        public string Value { get; }

        public UserIdValue(string value)
        {
            Value = value;
        }
    }
}