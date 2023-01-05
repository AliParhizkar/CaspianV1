using System;
namespace Caspian.Common
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
