using System;
namespace Caspian.common
{
    public class CaspianDataService
    {
        public CaspianDataService()
        {
            GUId = Guid.NewGuid().ToString();
        }

        public int UserId { get; set; }

        public string GUId { get; private set; }
    }
}
