using System;

namespace FlowDMApi.Models.Token
{
    public class TokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
