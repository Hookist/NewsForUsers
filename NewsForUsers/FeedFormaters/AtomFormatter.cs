using NewsForUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace NewsForUsers.FeedFormaters
{
    public class AtomFormatter : IFeedable
    {
        public IEnumerable<Entity> GetEntities(string urlFeedLocation)
        {
            using (XmlReader reader = XmlReader.Create(urlFeedLocation))
            {
                Atom10FeedFormatter atom = new Atom10FeedFormatter();
                // try to read it as an atom feed
                if (atom.CanRead(reader))
                {
                    atom.ReadFrom(reader);
                    List<Entity> entities = new List<Entity>();
                    foreach (SyndicationItem item in atom.Feed.Items)
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