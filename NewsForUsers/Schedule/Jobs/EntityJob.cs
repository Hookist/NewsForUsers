using NewsForUsers.FeedFormaters;
using NewsForUsers.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;

namespace NewsForUsers.Schedule.Jobs
{
    public class EntityJob : IJob
    {
        NewsForUsersModel db = new NewsForUsersModel();
        public void Execute(IJobExecutionContext context)
        {
            Dictionary<int, DateTimeOffset?> col = new Dictionary<int, DateTimeOffset?>();
            List<Source> sources = db.Sources.ToList();
            if (sources == null)
                return;

            foreach(var source in db.Sources)
            {
                DateTimeOffset? entityLastDateTime = db.Entities.Where(e => e.SourceId == source.Id).Max(e => e.PublicationDate);

                SyndicationFeed feed = FeedHelper.GetSyndicationFeedData(source.Link);
                foreach(SyndicationItem item in feed.Items)
                {
                    if(item.PublishDate > entityLastDateTime || entityLastDateTime == null)
                    {
                        db.Entities.Add(new Entity()
                        {
                            Title = item.Title.Text,
                            PublicationDate = item.PublishDate,
                            Link = item.Links[0].Uri.ToString(),
                            SourceId = source.Id
                        });
                    }
                }
            }

            db.SaveChangesAsync();
        }



    }
}