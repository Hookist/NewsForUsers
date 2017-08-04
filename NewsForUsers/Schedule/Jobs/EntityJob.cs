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
            List<Source> sources = db.Sources.ToList();
            if (sources == null)
                return;

            foreach(var source in db.Sources)
            {
                DateTimeOffset? entityLastDateTime = db.Entities.Where(e => e.SourceId == source.Id).Max(e => e.PublicationDate);

                List<Entity> entities = FeedHelper.GetEntitiesFromFeed(source.Link).ToList();
                foreach(Entity item in entities)
                {
                    if(item.PublicationDate > entityLastDateTime || entityLastDateTime == null)
                    {
                        db.Entities.Add(new Entity()
                        {
                            Title = item.Text,
                            PublicationDate = item.PublicationDate,
                            Link = item.Link,
                            SourceId = source.Id
                        });
                    }
                }
            }

            db.SaveChangesAsync();
        }



    }
}