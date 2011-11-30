using System;
using System.Collections.Generic;

namespace LoreSoft.Shared.Messaging
{
    public class GenericMessage : Message
    {
        public GenericMessage(string name)
            : this(null, name)
        { }

        public GenericMessage(object sender, string name)
            : base(sender)
        {
            Parameters = new Dictionary<string, object>();
            Name = name;
        }

        public string Name { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }
    }
}
