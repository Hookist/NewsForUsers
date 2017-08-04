using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewsForUsers.Models;
using System.Xml;
using System.ServiceModel.Syndication;

namespace NewsForUsers.FeedFormaters
{
    public class RSSFormater : IFeedable
    {
        public IEnumerable<Entity> GetEntities(string urlFeedLocation)
        {
            using (XmlReader reader = XmlReader.Create(urlFeedLocation))
            {
                Rss20FeedFormatter rss = new Rss20FeedFormatter();
                // try to read it as an atom feed
                if (rss.CanRead(reader))
                {
                    rss.ReadFrom(reader);
                    List<Entity> entities = new List<Entity>();
                    foreach (SyndicationItem item in rss.Feed.Items)
                    {
                        entities.Add(new Entity()
                        {
                            Title = item.Title.Text,
                            PublicationDate = item.PublishDate,
                            Link = item.Links[0].Uri.ToString(),
                        });
                    }
                    return entities;
                }
            }
            return null;
        }
    }
}