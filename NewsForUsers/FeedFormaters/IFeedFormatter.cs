using NewsForUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsForUsers.FeedFormaters
{
    interface IFeedHelper
    {
        IEnumerable<Entity> GetFeedData(string url);
    }
}
