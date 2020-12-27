using System;

namespace Drumpad_Machine.Models
{
    public class Audio
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public static bool operator !=(Audio obj1, Audio obj2) => obj1.ID != obj2.ID;

        public static bool operator ==(Audio obj1, Audio obj2) => obj1.ID == obj2.ID;
    }
}
