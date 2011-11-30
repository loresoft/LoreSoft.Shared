using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Security
{
    public class SaltedHashBuilder : HashBuilder
    {
        public SaltedHashBuilder(string salt)
        {
            if (salt.IsNullOrEmpty())
                return;

            Writer.Write(salt);
        }
    }
}
